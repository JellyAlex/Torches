using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Torches.Entities;
using System.Drawing;
using System.Diagnostics;

namespace Torches
{
    public class World
    {
        Player player;
        List<Zone> zones;
        int currentZone = -1;

        public World()
        {
            player = new Player(7, 3, "No Name", 100, 100);
            zones = new List<Zone>();

            Zone zone = new Zone(0, 0);
            //for(int yi = 0; yi < Zone.Height; yi++)
            //{
            //    for(int xi = 0; xi < Zone.Width; xi++)
            //    {
            //        Tile tile = new Tile('.', false);
            //        zone.AddTile(xi, yi, tile);
            //    }
            //}

            zones.Add(zone);
            currentZone = 0;
            zones[currentZone].Render();
            player.Render();

            string name;
            do
            {
                Renderer.PrintGameOutput("Enter a character name in the text input!");
                name = Game.InputCommand();
                player.name = name;
            } while (name.Length > 20);

            Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY, name, Color.LightGray);

            Renderer.PrintGameOutput("Enter a command... (type 'help' or 'h' for additional information)");
        }

        public World(List<Zone> zones)
        {
            this.zones = zones;
        }

        public void Update(string command)
        {
            string[] segments = command.Split(' ');
            if (segments.First() == "info")
            {

            }
            else if (segments.First() == "move" || segments.First() == "m")
            {
                if (segments.Length >= 2)
                {
                    string direction = segments[1].ToLower();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        if (!zones[currentZone].IsSolidAt(player.x + 1, player.y))
                        {
                            player.x += 1;
                            zones[currentZone].Render();
                            player.Render();
                            Trace.WriteLine("Moving player right");
                            Renderer.PrintGameOutput("Moved character east");
                        }
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        if (!zones[currentZone].IsSolidAt(player.x - 1, player.y))
                        {
                            player.x -= 1;
                            zones[currentZone].Render();
                            player.Render();
                            Trace.WriteLine("Moving player left");
                            Renderer.PrintGameOutput("Moved character west");
                        }
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        if (!zones[currentZone].IsSolidAt(player.x, player.y + 1))
                        {
                            player.y += 1;
                            zones[currentZone].Render();
                            player.Render();
                            Trace.WriteLine("Moving player up");
                            Renderer.PrintGameOutput("Moved character north");
                        }
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        if (!zones[currentZone].IsSolidAt(player.x, player.y - 1))
                        {
                            player.y -= 1;
                            zones[currentZone].Render();
                            player.Render();
                            Trace.WriteLine("Moving player down");
                            Renderer.PrintGameOutput("Moved character south");
                        }
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
            }
            else if (segments.First() == "interact")
            {

            }
            else
            {
                if(command.Length <=  30)
                {
                    Renderer.PrintGameOutput("(Error) Unknown command: " + command);
                }
                else
                {
                    Renderer.PrintGameOutput("(Error) Unknown command");
                }
            }
        }
    }
}
