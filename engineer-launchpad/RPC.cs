using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Text;

namespace engineer_launchpad
{
    class RPC
    {
        public enum CustomRPC
        {
            SetEngineer = 42,
            FixLights = 43
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
    class HandleRpcPatch
    {
        static void Postfix(byte HKHMBLJFLMC, MessageReader ALMCIJKELCP)
        {
            byte packetId = HKHMBLJFLMC;
            MessageReader reader = ALMCIJKELCP;
            switch (packetId)
            {
                case (byte)RPC.CustomRPC.SetEngineer:
                    byte engineerId = reader.ReadByte();
                    PlayerControl engineer = PlayerTools.getPlayerFromId(engineerId);
                    if (engineer != null)
                        new EngineerPlayer(engineer);
                    else
                        System.Console.WriteLine("Invalid engineer id provided inside HandleRpc...");
                    break;
                case (byte)RPC.CustomRPC.FixLights:
                    SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                    switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
                    break;
            }
        }
    }
}
