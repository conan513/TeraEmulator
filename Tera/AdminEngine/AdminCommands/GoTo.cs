using System;
using Communication;
using Data.Enums;
using Data.Interfaces;
using Data.Structures.Player;
using Data.Structures.World;
using Network.Server;
using Tera.Services;
using Utils;

namespace Tera.AdminEngine.AdminCommands
{
    internal class GoTo : ACommand
    {
        public override void Process(IConnection connection, string msg)
        {
            try
            {
                if (msg.Length == 0)
                {
                    new SpChatMessage("Maps:", ChatType.System).Send(connection);
                    foreach (var map in MapService.Maps)
                        new SpChatMessage("" + map.Key, ChatType.System).Send(connection);
                    return;
                }

                Player player = connection.Player;

                string[] options = msg.Split(' ');

                switch (options[0].ToLower())
                {
                    case "pos":
                        Global.TeleportService.ForceTeleport(player,
                                                             new WorldPosition
                                                                 {
                                                                     Heading = player.Position.Heading,
                                                                     MapId = player.Position.MapId,
                                                                     X = float.Parse(options[1]),
                                                                     Y = float.Parse(options[2]),
                                                                     Z = float.Parse(options[3])
                                                                 });
                        break;

                    case "IslandOfDawn":
                        Global.TeleportService.ForceTeleport(player,
                                                             new WorldPosition
                                                                 {
                                                                     Heading = 32767,
                                                                     MapId = 13,
                                                                     X = 93492,
                                                                     Y = -88216,
                                                                     Z = -4523
                                                                 });
                        break;
                    case "all":
                        //Global.PlayerService.TeleportPlayer(PlayerService.GetPlayerByName(options[1]), player.Position);
                        break;
                    default:
                        int mapId = int.Parse(msg);
                        Global.TeleportService.ForceTeleport(connection.Player, MapService.Maps[mapId][0].Npcs[0].Position);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorException("AdminEngine: Speed:", ex);
            }
        }
    }
}
