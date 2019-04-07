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
            player = new Entity();
            player.AddComponent(new ZonePosition(8, 3))
                .AddComponent(new Symbol('@'))
                .AddComponent(new Colour(Color.DarkRed));

            Trace.WriteLine("Player pos: " + player.GetComponent<ZonePosition>().x + ", " + player.GetComponent<ZonePosition>().y);

            zones = new List<Zone>();

            Zone zone = new Zone(0, 0);

            zones.Add(zone);
            currentZone = 0;
            zones[currentZone].Render();
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
    }
}
