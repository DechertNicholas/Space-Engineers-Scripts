using Sandbox.ModAPI.Ingame;
using System.Collections.Generic;
using System;

namespace BatteryMonitor
{
    partial class Program : MyGridProgram
    {
        IMyBlockGroup BatteryGroup;
        IMyTextSurface CockpitScreen;
        float MaxCharge;
        float CurrentCharge;

        // Set these
        string CockpitName = "Your Cockpit";
        string BatteryGroupName = "Your Battery Group";

        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update100;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Echo("Running");

            CockpitScreen = (GridTerminalSystem.GetBlockWithName(CockpitName)
                as IMyTextSurfaceProvider).GetSurface(1);
            CockpitScreen.Font = "Monospace";
            CockpitScreen.FontSize = 1.69f; // Sex number
            CockpitScreen.BackgroundColor = new VRageMath.Color(0, 88, 151);

            BatteryGroup = GridTerminalSystem.GetBlockGroupWithName(BatteryGroupName);

            float groupMaxCharge = 0f;
            float groupCurrentCharge = 0f;

            var blocks = new List<IMyTerminalBlock>();
            BatteryGroup.GetBlocks(blocks);

            foreach (var block in blocks)
            {
                var currentBattery = block as IMyBatteryBlock;
                groupMaxCharge += currentBattery.MaxStoredPower;
                groupCurrentCharge += currentBattery.CurrentStoredPower;
            }

            MaxCharge = groupMaxCharge;
            CurrentCharge = groupCurrentCharge;
            
            WriteScreenStatus();
        }

        public void WriteScreenStatus()
        {
            int chargePercent = (int)(Math.Round(CurrentCharge / MaxCharge, 2) * 100);
            string chargeString = CurrentCharge.ToString("0.00");
            string batteryBar = "";
            int eachSide = 6;
            // 100 / 13 = 7.96
            int barsToWrite = (int)Math.Round(chargePercent / 7.96f, 0);

            for (var i = 0; i < barsToWrite; i++)
            {
                if (i < eachSide)
                {
                    batteryBar += "/";
                }
                if (i == 6)
                {
                    batteryBar += "|";
                }
                if (i >= (eachSide + 1))
                {
                    batteryBar += "\\";
                }
            }

            if (batteryBar.Length < 13)
            {
                batteryBar = batteryBar.PadRight(13);
            }

            CockpitScreen.WriteText(
                $"Battery Status:\n" +
                $"Charge: {chargePercent}%\n" +
                $"{chargeString} MW / {MaxCharge} MW\n" +
                $"[{batteryBar}]"
            );
        }
    }
}
