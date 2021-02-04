using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace engineer_launchpad
{
    class EngineerPlayer
    {
        public PlayerControl engineerControl { get; set; }
        public DateTime lastRepair { get; set; }
        public bool canRepair { get; set; }
        public static List<EngineerPlayer> allEngineers = new List<EngineerPlayer>();
        public static EngineerPlayer GetEngineerFromPlayer(PlayerControl player)
        {
            foreach (EngineerPlayer player2 in allEngineers)
                if (player2.engineerControl.PlayerId == player.PlayerId)
                    return player2;
            return null;
        }
        public static bool IsPlayerEngineer(PlayerControl player)
        {
            foreach (EngineerPlayer player2 in allEngineers)
                if (player2.engineerControl.PlayerId == player.PlayerId)
                    return true;
            return false;
        }
        public EngineerPlayer(PlayerControl engineer)
        {
            this.engineerControl = engineer;
            allEngineers.Add(this);
        }
    }
    class EngineerConfig
    {
        public static Color engineerColor = new Color(151f / 255f, 46f / 255f, 0, 1);
        public static bool engineerEnabled = true;
        public static float engineerCooldown = 80f;
        public static int engineerCount = 1;
        public static bool showEngineers = false;
    }
}
