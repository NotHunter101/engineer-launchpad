using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Hazel;

namespace engineer_launchpad
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.RpcSetInfected))]
    class RpcSetInfectedPatch
    {
        static void Postfix()
        {
            List<PlayerControl> nonInfectedPlayers = new List<PlayerControl>();
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (!player.Data.IsImpostor)
                    nonInfectedPlayers.Add(player);
            }
            Random roleRng = new Random();

            if (EngineerConfig.engineerEnabled)
            {
                EngineerPlayer.allEngineers.Clear();
                for (var i = 0; i < EngineerConfig.engineerCount; i++)
                {
                    if (nonInfectedPlayers.Count < 1)
                        break;
                    int engineerRandom = roleRng.Next(0, nonInfectedPlayers.Count);
                    if (nonInfectedPlayers[engineerRandom] == null)
                        break;
                    new EngineerPlayer(nonInfectedPlayers[engineerRandom]);
                    nonInfectedPlayers.RemoveAt(engineerRandom);
                }

                if (EngineerPlayer.allEngineers.Count > 0)
                {
                    foreach (EngineerPlayer engineer in EngineerPlayer.allEngineers)
                    {
                        MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RPC.CustomRPC.SetEngineer, Hazel.SendOption.None, -1);
                        writer.Write(engineer.engineerControl.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
        }
    }
}
