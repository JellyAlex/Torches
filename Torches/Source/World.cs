using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Torches.Entities;
using System.Drawing;

namespace Torches
{
    class World
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
            RenderCurrentZone();
            player.Render();

            string name;
            do
            {
                Renderer.PrintGameOutput("Enter a character name in the text input!");
                name = Game.InputCommand();
                player.name = name;
            } while (name.Length > 20);

            Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY, name, Color.LightGray);
        }

        public World(List<Zone> zones)
        {
            this.zones = zones;
        }

        public void Update(string command)
        {
            string[] commandSegments = command.Split(' ');
            if (commandSegments.First() == "info")
            {

            }
            else if (commandSegments.First() == "move")
            {

            }
            else if (commandSegments.First() == "interact")
            {

            }
        }

        protected void RenderCurrentZone()
        {
            Zone zone = zones.ElementAt(currentZone);

            for (int yi = 0; yi < Zone.Height; yi++)
            {
                for (int xi = 0; xi < Zone.Width; xi++)
                {
                    Renderer.PrintAt(Constants.MapX + xi, Constants.MapY + (Zone.Height - yi) , zone.tiles[yi, xi].symbol, Color.LightGray);
                }
            }
        }
    }
}
