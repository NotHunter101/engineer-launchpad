using HarmonyLib;
using Reactor.Extensions;
using Reactor.Unstrip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace engineer_launchpad
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    class HudUpdateManager
    {
        static AssetBundle bundle = AssetBundle.LoadFromFile(Directory.GetCurrentDirectory() + "\\BepInEx\\assets\\bundle");
        static Sprite repairIco = bundle.LoadAsset<Sprite>("RE").DontUnload();

        static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState == InnerNetClient.GameStates.Started)
            {
                bool sabotageActive = false;
                foreach (PlayerTask task in PlayerControl.LocalPlayer.myTasks)
                {
                    if (task.TaskType == TaskTypes.FixLights || task.TaskType == TaskTypes.RestoreOxy || task.TaskType == TaskTypes.ResetReactor || task.TaskType == TaskTypes.ResetSeismic || task.TaskType == TaskTypes.FixComms)
                        sabotageActive = true;
                }
                EngineerRole.sabotageActive = sabotageActive;

                if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) || EngineerConfig.showEngineers)
                {
                    foreach (EngineerPlayer engineer in EngineerPlayer.allEngineers)
                        engineer.engineerControl.nameText.Color = EngineerConfig.engineerColor;
                    if (MeetingHud.Instance != null)
                        foreach (PlayerVoteArea player in MeetingHud.Instance.playerStates)
                            if (EngineerPlayer.IsPlayerEngineer(PlayerTools.getPlayerFromId((byte)player.TargetPlayerId)))
                                player.NameText.Color = EngineerConfig.engineerColor;
                }

                if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer) && __instance.UseButton.isActiveAndEnabled)
                {
                    __instance.KillButton.gameObject.SetActive(true);
                    __instance.KillButton.isActive = true;
                    __instance.KillButton.SetCoolDown(0f, 30f);
                    __instance.KillButton.renderer.sprite = repairIco;
                    __instance.KillButton.renderer.color = Palette.EnabledColor;
                    __instance.KillButton.renderer.material.SetFloat("_Desat", 0f);
                }
            }
        }
    }
}
