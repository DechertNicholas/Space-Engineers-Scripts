#if DEBUG
using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System;
using VRage.Collections;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ObjectBuilders.Definitions;
using VRage.Game;
using VRage;
using VRageMath;

namespace PistonWelderController
{
    partial class Program : MyGridProgram
    {
#endif
        const string PistonName = "Pistons";
        const string SensorName = "Sensor";

        List<IMyPistonBase> Pistons;
        List<IMySensorBlock> Sensors;
        Dictionary<IMySensorBlock, List<IMyPistonBase>> SensorPistonMap = new Dictionary<IMySensorBlock, List<IMyPistonBase>>();
        Dictionary<IMyCubeGrid, IMyPistonBase> PistonHeadGridReverseLookup = new Dictionary<IMyCubeGrid, IMyPistonBase>();

        public Program()
        {
            Pistons = GetBlockList<IMyPistonBase>(PistonName);
            Sensors = GetBlockList<IMySensorBlock>(SensorName);
            MapPistonHeadGridReverseLookup();
            MapSensorsToPistons();
            ConfigureSensors();

            Runtime.UpdateFrequency = UpdateFrequency.Update1;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Sensors.ForEach(s =>
            {
                var detectedEntities = new List<MyDetectedEntityInfo>(); ;
                s.DetectedEntities(detectedEntities);
                var isExtending = detectedEntities.Count == 0;
                var piston = GetNextPistonBase(s, isExtending);
                var velocity = Math.Abs(piston.Velocity);

                piston.Velocity = isExtending ? velocity : -velocity;
            });
        }

        public void MapSensorsToPistons()
        {
            foreach (var sensor in Sensors)
            {
                SensorPistonMap[sensor] = new List<IMyPistonBase>();
                IMyPistonBase nextPiston;
                IMyCubeGrid currentGrid = sensor.CubeGrid;

                while(PistonHeadGridReverseLookup.TryGetValue(currentGrid, out nextPiston))
                {
                    SensorPistonMap[sensor].Add(nextPiston);
                    currentGrid = nextPiston.CubeGrid;
                }

                if (SensorPistonMap[sensor].Count == 0) throw new Exception($"Sensor '{sensor.CustomName}' is not attached to a piston.");
            }
        }

        public void MapPistonHeadGridReverseLookup()
        {
            foreach(var piston in Pistons)
            {
                PistonHeadGridReverseLookup[piston.TopGrid] = piston;
            }
        }

        public IMyPistonBase GetNextPistonBase(IMySensorBlock sensor, bool isExtending)
        {
            const float tolerance = 0.05f;

            for (int i = 0; isExtending == true && i < SensorPistonMap[sensor].Count; i++)
            {
                if (SensorPistonMap[sensor][i].CurrentPosition < SensorPistonMap[sensor][i].MaxLimit - tolerance)
                {
                    return SensorPistonMap[sensor][i];
                }
            }

            for (int i = SensorPistonMap[sensor].Count - 1; isExtending == false && i >= 0; i--)
            {
                if (SensorPistonMap[sensor][i].CurrentPosition > SensorPistonMap[sensor][i].MinLimit + tolerance)
                {
                    return SensorPistonMap[sensor][i];
                }
            }

            if (isExtending)
            {
                return SensorPistonMap[sensor].Last();
            }
            else
            {
                return SensorPistonMap[sensor].First();
            }
        }

        public void ConfigureSensors()
        {
            foreach(IMySensorBlock sensor in Sensors)
            {
                var welder = sensor.CubeGrid.GetCubeBlock(sensor.Position - Base6Directions.GetIntVector(sensor.Orientation.Forward)).FatBlock as IMyShipWelder;
                var sensorConfigurationMap = new Dictionary<Base6Directions.Direction, float>()
                {
                    { welder.Orientation.Forward, 3.8f },
                    { Base6Directions.GetOppositeDirection(welder.Orientation.Forward), 0.1f },
                    { welder.Orientation.Left, 0.3f },
                    { Base6Directions.GetOppositeDirection(welder.Orientation.Left), 0.3f },
                    { welder.Orientation.Up, welder.Orientation.Up == sensor.Orientation.Forward ? 0.1f : 2.5f },
                    { Base6Directions.GetOppositeDirection(welder.Orientation.Up), welder.Orientation.Up == sensor.Orientation.Forward ? 2.5f : 0.1f }
                };

                sensor.FrontExtend   = sensorConfigurationMap[sensor.Orientation.Forward];
                sensor.BackExtend  = sensorConfigurationMap[Base6Directions.GetOppositeDirection(sensor.Orientation.Forward)];
                sensor.LeftExtend = sensorConfigurationMap[sensor.Orientation.Left];
                sensor.RightExtend    = sensorConfigurationMap[Base6Directions.GetOppositeDirection(sensor.Orientation.Left)];
                sensor.TopExtend   = sensorConfigurationMap[sensor.Orientation.Up];
                sensor.BottomExtend  = sensorConfigurationMap[Base6Directions.GetOppositeDirection(sensor.Orientation.Up)];

                sensor.PlayProximitySound = false;
                sensor.DetectPlayers = false;
                sensor.DetectFloatingObjects = false;
                sensor.DetectSmallShips = false;
                sensor.DetectLargeShips = false;
                sensor.DetectStations = false;
                sensor.DetectSubgrids = true;
                sensor.DetectAsteroids = false;

                sensor.DetectOwner = true;
                sensor.DetectFriendly = true;
                sensor.DetectNeutral = true;
                sensor.DetectEnemy = true;
            }
        }

        public List<T> GetBlockList<T>(string name) where T : IMyTerminalBlock
        {
            var result = new List<T>();
            var blocks = new List<IMyTerminalBlock>();
            GridTerminalSystem.GetBlockGroupWithName(name)?.GetBlocks(blocks);

            if (blocks.Count > 0)
            {
                foreach (var block in blocks) result.Add((T)block);
            }
            else
            {
                var block = (T)GridTerminalSystem.GetBlockWithName(name);
                if (block == null) throw new Exception($"Block or block group named \"{name}\" not found.");
                result = new List<T>() { block };
            }

            return result;
        }

        public List<MyDetectedEntityInfo> AggregateDetectedEntities(List<IMySensorBlock> sensorBlocks)
        {
            var result = new List<MyDetectedEntityInfo>();
            var entities = new List<MyDetectedEntityInfo>();

            foreach (var sensor in sensorBlocks)
            {
                if (sensor == null) continue;

                sensor.DetectedEntities(entities);
                result.AddRange(entities);
            }

            return result;
        }
#if DEBUG
    }
}
#endif