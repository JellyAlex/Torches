using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Diagnostics;
using Torches.ECS;
using System.IO;

namespace Torches
{
    public class World
    {
        private Entity player;
        private readonly List<Zone> zones;
        int currentZone = -1;

        public World()
        {
            // Create the player
            player = new Entity(EntityFlags.Player);
            player
                .AddComponent(new Position(8, 3))
                .AddComponent(new Symbol('@'))
                .AddComponent(new Colour(Color.DarkOrange))
                .AddComponent(new Health(200, 200))
                .AddComponent(new Damager(10))
                .AddComponent(new Inventory())
                .AddComponent(new Coins())
                .AddComponent(new Weapon());

            zones = new List<Zone>();

            LoadZonesFromRes();

            // Set the current zone to the index of zone 0,0
            currentZone = zones.FindIndex(z => z.X == 0 && z.Y == 0);

            // Render the current zone and player.
            GetCurrentZone().RenderAll();
            Renderer.RenderEntity(player);
            
            // Enter the player's name.
            string name;
            do
            {
                Renderer.PrintGameOutput("Enter a character name in the text input!");
                name = Game.InputCommand();
                player.AddComponent(new Character(name));
            } while (name.Length > 20);

            Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY, name, Color.White);

            // Render player's stats and inventory
            Renderer.RenderPlayerInfo(player);

            Renderer.PrintGameOutput("Enter a command... (type 'help' or 'h' for additional information)");
        }

        public Zone GetCurrentZone()
        {
            return zones[currentZone];
        }

        public ref Entity GetPlayer()
        {
            return ref player;
        }

        public void ChangeZone(int zx, int zy, int px, int py)
        {
            for (int i = 0; i < zones.Count; i++)
            {
                if(zones[i].X == zx && zones[i].Y == zy)
                {
                    currentZone = i;

                    player.GetComponent<Position>().x = px;
                    player.GetComponent<Position>().y = py;

                    zones[currentZone].RenderAll();
                    Renderer.RenderEntity(player);

                    return;
                }
            }

            Trace.WriteLine($"Error: Zone {zx}, {zy} not found.");
        }

        public void Update()
        {
            foreach(Zone zone in zones)
            {
                zone.Update();
            }
        }

        public void LoadZonesFromRes()
        {
            // Load all zones in the BaseZones directory.
            string[] directories = Directory.GetDirectories("Resources\\BaseZones\\", "*", SearchOption.TopDirectoryOnly);
            foreach (string dir in directories)
            {
                // Only look at the inner most folder's name, split the x and y.
                string[] dirSplit = dir.Split('\\').Last().Split(',');
                if (dirSplit.Length == 2)
                {
                    if (int.TryParse(dirSplit[0], out int x) && int.TryParse(dirSplit[1], out int y))
                    {
                        // Make a new zone with the coordinates and add it to the list.
                        zones.Add(ResourceManager.LoadZoneFromRes(x, y, this));
                    }
                    else
                    {
                        Trace.WriteLine($"Error loading zone: {dir} ({dirSplit[0]}, {dirSplit[1]})");
                    }
                }
            }
        }

        public Zone GetZone(int x, int y)
        {
            foreach (Zone z in zones)
            {
                if(z.X == x && z.Y == y)
                {
                    return z;
                }
            }

            return null;
        }
    }
}
