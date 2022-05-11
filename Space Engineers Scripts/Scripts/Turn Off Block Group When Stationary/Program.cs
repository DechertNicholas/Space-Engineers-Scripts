using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;

namespace TurnOffBlockGroupWhenStationary
{
    partial class Program : MyGridProgram
    {
        public const int delayCount = 3;

        public List<IMyShipController> ShipControllers = new List<IMyShipController>();
        public string blockGroupName;
        public int StartupDelay = delayCount;
        public int ShutdownDelay = delayCount;

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.None;
            GridTerminalSystem.GetBlocksOfType(ShipControllers, block => block.CubeGrid.EntityId == Me.CubeGrid.EntityId);
            Echo($"ShipController: {ShipControllers[0].CustomName}");
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Echo($"ShipController: {ShipControllers[0].CustomName}");
            if(StartupDelay <= 0 && ShutdownDelay <= 0)
            {
                Echo("Finished.");
                Runtime.UpdateFrequency = UpdateFrequency.None;
            }
            else
            {
                Echo("Running...");
            }

            if (updateSource != UpdateType.Update100) 
            {
                blockGroupName = argument;
                StartupDelay = delayCount;
                ShutdownDelay = delayCount;
                Runtime.UpdateFrequency = UpdateFrequency.Update100; 
                return; 
            }

            if (StartupDelay > 0)
            {
                Echo($"{nameof(StartupDelay)}: {StartupDelay--}");
                return;
            }

            if (ShipControllers[0].GetShipSpeed() < 0.5 && ShutdownDelay > 0)
            {
                Echo($"{nameof(ShutdownDelay)}: {ShutdownDelay--}");
                return;
            }
            else if (ShutdownDelay > 0)
            {
                ShutdownDelay = delayCount;
                return;
            }

            var blocks = new List<IMyFunctionalBlock>();
            GridTerminalSystem.GetBlockGroupWithName(blockGroupName).GetBlocksOfType(blocks);
            foreach (var block in blocks)
            {
                block.Enabled = false;
            }
        }
    }
}
