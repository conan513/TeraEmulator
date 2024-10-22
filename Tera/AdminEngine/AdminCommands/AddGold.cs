﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Communication;
using Data.Enums;
using Data.Interfaces;
using Data.Structures.Player;
using Network.Server;
using Utils;

namespace Tera.AdminEngine.AdminCommands
{
    class AddGold : ACommand
    {
        public override void Process(IConnection connection, string msg)
        {
            try
            {
                var args = msg.Split(' ');
                int goldAmount = 0;

                // Do have a target ?
                if (int.TryParse(args[0], out goldAmount))
                {
                    // We are Singular!
                    Global.StorageService.AddMoneys(connection.Player, connection.Player.Inventory,
                        int.Parse(args[0]));
                }
                else
                {
                    var target = Communication.Global.PlayerService.GetPlayerByName(args[0]);
                    goldAmount = int.Parse(args[1]);
                    Global.StorageService.AddMoneys(target, target.Inventory,
                        goldAmount);
                }

            }
            catch (Exception e)
            {
                new SpChatMessage("Wrong Syntax!\n Type `addgold {player} {number}", ChatType.Notice).Send(connection);
                Log.Warn(e.ToString());
            }
        }
    }
}
