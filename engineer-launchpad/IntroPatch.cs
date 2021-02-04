using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace engineer_launchpad
{
    [HarmonyPatch(typeof(IntroCutscene.CoBegin__d), nameof(IntroCutscene.CoBegin__d.MoveNext))]
    class IntroPatch
    {
        static void Postfix(IntroCutscene.CoBegin__d __instance)
        {
            foreach (EngineerPlayer engineer in EngineerPlayer.allEngineers)
            {
                engineer.lastRepair = DateTime.UtcNow;
            }
            if (EngineerPlayer.IsPlayerEngineer(PlayerControl.LocalPlayer))
            {
                __instance.__this.Title.Text = "Engineer";
                __instance.__this.Title.Color = EngineerConfig.engineerColor;
                __instance.__this.ImpostorText.Text = "Maintain important systems on the ship";
                __instance.__this.BackgroundBar.material.color = EngineerConfig.engineerColor;
            }
        }
    }
}
