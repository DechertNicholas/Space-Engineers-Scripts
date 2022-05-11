using Sandbox.ModAPI.Ingame;
using System;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        public void Main(string argument, UpdateType updateSource)
        {
            var substringLength = argument[0] == '"' ? argument.IndexOf('"', 1) + 1 : argument.IndexOf(' ');
            var nameRaw = argument.Substring(0, substringLength);
            var argumentSegments = argument.Substring(nameRaw.Length).Trim().Split(' ');
            var action = argumentSegments[0];
            var property = argumentSegments[1];

            var name = nameRaw[0] == '"' ? nameRaw.Substring(1, nameRaw.Length - 2) : nameRaw;
            var block = GridTerminalSystem.GetBlockWithName(name);

            if(action.Equals("get", StringComparison.OrdinalIgnoreCase))
            {

                Echo($"{property}: {GetFloatValue(block, property)}");
            }
            else if (action.Equals("set", StringComparison.OrdinalIgnoreCase))
            {
                var value = float.Parse(argumentSegments[2]);
                SetFloatValue(block, property, value);
                Echo($"{property} set to: {value}");
            }
            else if (action.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                var original = GetFloatValue(block, property);
                var value = original + float.Parse(argumentSegments[2]);
                SetFloatValue(block, property, value);
                Echo($"{property} set to: {value}");
            }
            else if (action.Equals("subtract", StringComparison.OrdinalIgnoreCase))
            {
                var original = GetFloatValue(block, property);
                var value = original - float.Parse(argumentSegments[2]);
                SetFloatValue(block, property, value);
                Echo($"{property} set to: {value}");
            }
            else
            {
                throw new Exception("Invalid action. Action must be 'get', 'set', 'add', or 'subtract'.");
            }
        }

        public float GetFloatValue(IMyTerminalBlock block, string property)
        {
            try
            {
                if (property.Equals("Velocity", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).Velocity;
                if (property.Equals("MaxVelocity", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).MaxVelocity;
                if (property.Equals("MinLimit", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).MinLimit;
                if (property.Equals("MaxLimit", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).MaxLimit;
                if (property.Equals("LowestPosition", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).LowestPosition;
                if (property.Equals("HighestPosition", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).HighestPosition;
                if (property.Equals("CurrentPosition", StringComparison.OrdinalIgnoreCase)) return ((IMyPistonBase)block).CurrentPosition;
            }
            catch { }
            finally { }

            try
            {
                if (property.Equals("Angle", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).Angle;
                if (property.Equals("CurrentPosition", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).Angle;
                if (property.Equals("Torque", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).Torque;
                if (property.Equals("BrakingTorque", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).BrakingTorque;
                if (property.Equals("TargetVelocityRad", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).TargetVelocityRad;
                if (property.Equals("TargetVelocityRPM", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).TargetVelocityRPM;
                if (property.Equals("Velocity", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).TargetVelocityRPM;
                if (property.Equals("LowerLimitRad", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).LowerLimitRad;
                if (property.Equals("LowerLimitDeg", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).LowerLimitDeg;
                if (property.Equals("UpperLimitRad", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).UpperLimitRad;
                if (property.Equals("UpperLimitDeg", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).UpperLimitDeg;
                if (property.Equals("MinLimit", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).LowerLimitDeg;
                if (property.Equals("MaxLimit", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).UpperLimitDeg;
                if (property.Equals("Displacement", StringComparison.OrdinalIgnoreCase)) return ((IMyMotorStator)block).Displacement;
            }
            catch { }
            finally { }

            throw new Exception($"Property '{property}' not found.");
        }

        public void SetFloatValue(IMyTerminalBlock block, string property, float value)
        {
            try
            {
                if (property.Equals("Velocity", StringComparison.OrdinalIgnoreCase)) { ((IMyPistonBase)block).Velocity = value; return; }
                if (property.Equals("MinLimit", StringComparison.OrdinalIgnoreCase)) { ((IMyPistonBase)block).MinLimit = value; return; }
                if (property.Equals("MaxLimit", StringComparison.OrdinalIgnoreCase)) { ((IMyPistonBase)block).MaxLimit = value; return; }
            }
            catch { }
            finally { }

            try
            {
                if (property.Equals("Torque", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).Torque = value; return; }
                if (property.Equals("BrakingTorque", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).BrakingTorque = value; return; }
                if (property.Equals("TargetVelocityRad", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).TargetVelocityRad = value; return; }
                if (property.Equals("TargetVelocityRPM", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).TargetVelocityRPM = value; return; }
                if (property.Equals("Velocity", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).TargetVelocityRPM = value; return; }
                if (property.Equals("LowerLimitRad", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).LowerLimitRad = value; return; }
                if (property.Equals("LowerLimitDeg", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).LowerLimitDeg = value; return; }
                if (property.Equals("UpperLimitRad", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).UpperLimitRad = value; return; }
                if (property.Equals("UpperLimitDeg", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).UpperLimitDeg = value; return; }
                if (property.Equals("MinLimit", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).LowerLimitDeg = value; return; }
                if (property.Equals("MaxLimit", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).UpperLimitDeg = value; return; }
                if (property.Equals("Displacement", StringComparison.OrdinalIgnoreCase)) { ((IMyMotorStator)block).Displacement = value; return; }
            }
            catch { }
            finally { }

            throw new Exception($"Property '{property}' is read-only or not found.");
        }
    }
}
