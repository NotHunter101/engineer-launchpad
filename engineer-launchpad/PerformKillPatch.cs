using HarmonyLib;
using Hazel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace engineer_launchpad
{
    [HarmonyPatch(typeof(KillButtonManager), nameof(KillButtonManager.PerformKill))]
    class PerformKillPatch
    {
        static void Prefix()
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
            {
                DestroyableSingleton<HudManager>.Instance.ShowMap((Action<MapBehaviour>)delegate (MapBehaviour m)
                {
                    m.ShowInfectedMap();
                    m.ColorControl.baseColor = EngineerRole.sabotageActive ? Color.gray : EngineerConfig.engineerColor;
                });
            }
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.ShowInfectedMap))]
    class EngineerMapOpen
    {
        static void Postfix(MapBehaviour __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
            {
                if (__instance.IsOpen)
                {
                    __instance.ColorControl.baseColor = EngineerConfig.engineerColor;
                    foreach (MapRoom room in __instance.infectedOverlay.rooms)
                    {
                        if (room.door != null)
                        {
                            room.door.enabled = false;
                            room.door.gameObject.SetActive(false);
                            room.door.gameObject.active = false;
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MapBehaviour), nameof(MapBehaviour.FixedUpdate))]
    class EngineerMapUpdate
    {
        static void Postfix(MapBehaviour __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
            {
                if (__instance.IsOpen && __instance.infectedOverlay.gameObject.active)
                {
                    if (!EngineerRole.sabotageActive)
                        __instance.ColorControl.baseColor = Color.gray;
                    else
                        __instance.ColorControl.baseColor = EngineerConfig.engineerColor;
                    long lastRepairTicks = EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).lastRepair.Ticks;
                    long dateTimeNowTicks = DateTime.UtcNow.Ticks;
                    float perc = (dateTimeNowTicks - lastRepairTicks) / (TimeSpan.TicksPerSecond * EngineerConfig.engineerCooldown);
                    if (perc > 1f)
                        EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair = true;
                    foreach (MapRoom room in __instance.infectedOverlay.rooms)
                    {
                        if (room.special != null)
                        {
                            if (!EngineerRole.sabotageActive)
                                room.special.material.SetFloat("_Desat", 1f);
                            else
                                room.special.material.SetFloat("_Desat", 0f);
                            room.special.enabled = true;
                            room.special.gameObject.SetActive(true);
                            room.special.gameObject.active = true;
                            if (!PlayerControl.LocalPlayer.Data.IsDead)
                                room.special.material.SetFloat("_Percent", 1f - perc);
                            else
                                room.special.material.SetFloat("_Percent", 1f);
                        }
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.Method_41))]
    class SabotageButtonDeactivatePatch
    {
        static bool Prefix(MapRoom __instance, float DCEFKAOFGOG)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageReactor))]
    class SabotageReactorPatch
    {
        static bool Prefix(MapRoom __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) && EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair && EngineerRole.sabotageActive)
            {
                EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).lastRepair = DateTime.UtcNow;
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                return false;
            }
            else if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageLights))]
    class SabotageLightsPatch
    {
        static bool Prefix(MapRoom __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) && EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair && EngineerRole.sabotageActive)
            {
                System.Console.WriteLine(EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair);
                EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).lastRepair = DateTime.UtcNow;
                SwitchSystem switchSystem = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                switchSystem.ActualSwitches = switchSystem.ExpectedSwitches;
                MessageWriter writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)RPC.CustomRPC.FixLights, Hazel.SendOption.None, -1);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return false;
            }
            else if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageComms))]
    class SabotageCommsPatch
    {
        static bool Prefix(MapRoom __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) && EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair && EngineerRole.sabotageActive)
            {
                System.Console.WriteLine(EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair);
                EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).lastRepair = DateTime.UtcNow;
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                return false;
            }
            else if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageOxygen))]
    class SabotageOxyPatch
    {
        static bool Prefix(MapRoom __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) && EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair && EngineerRole.sabotageActive)
            {
                System.Console.WriteLine(EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair);
                EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).lastRepair = DateTime.UtcNow;
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 0 | 64);
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 1 | 64);
                return false;
            }
            else if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
                return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(MapRoom), nameof(MapRoom.SabotageSeismic))]
    class SabotageSeismicPatch
    {
        static bool Prefix(MapRoom __instance)
        {
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) && EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair && EngineerRole.sabotageActive)
            {
                System.Console.WriteLine(EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).canRepair);
                EngineerPlayer.GetEngineerFromPlayer(PlayerControl.LocalPlayer).lastRepair = DateTime.UtcNow;
                ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
                return false;
            }
            else if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
                return false;
            return true;
        }
    }
}
