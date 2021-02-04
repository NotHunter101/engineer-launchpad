using System;
using System.Collections.Generic;
using System.Text;

namespace engineer_launchpad
{
    class PlayerTools
    {
        public static PlayerControl getPlayerFromId(byte id)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;
            return null;
        }
    }
}
