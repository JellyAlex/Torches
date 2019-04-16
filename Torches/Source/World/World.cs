using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Diagnostics;
using Torches.ECS;

namespace Torches
{
    public class World
    {
        Entity player;
        List<Zone> zones;
        int currentZone = -1;

        public World()
        {
            player = new Entity(EntityFlags.Player);
            player
                .AddComponent(new ZonePosition(8, 3))
                .AddComponent(new Symbol('@'))
                .AddComponent(new Colour(Color.DarkRed))
                .AddComponent(new Health(100, 100))
                .AddComponent(new Damager(10));


            Trace.WriteLine("Player pos: " + player.GetComponent<ZonePosition>().x + ", " + player.GetComponent<ZonePosition>().y);

            zones = new List<Zone>();

            // TODO: Automatically load all zones found in base zones.
            zones.Add(new Zone(0, 0));
            zones.Add(new Zone(1, 0));

            currentZone = 0;
            zones[currentZone].RenderAll();
            Renderer.RenderEntity(player);
            
            string name;
            do
            {
                Renderer.PrintGameOutput("Enter a character name in the text input!");
                name = Game.InputCommand();
                player.AddComponent(new Character(name));
            } while (name.Length > 20);

            Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY, name, Color.LightGray);

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
                if(zones[i].x == zx && zones[i].y == zy)
                {
                    currentZone = i;

                    player.GetComponent<ZonePosition>().x = px;
                    player.GetComponent<ZonePosition>().y = py;

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
    }
}
