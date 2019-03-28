using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace Torches
{
    class Zone
    {
        public const int Width = 16;
        public const int Height = 8;

        protected int x { get; set; }
        protected int y { get; set; }

        public Tile[,] tiles { get; set; }
        protected List<Entity> entities { get; set; }

        public Zone()
        {
            tiles = new Tile[Height, Width];
            entities = new List<Entity>();
            this.x = 0;
            this.y = 0;
        }

        public Zone(int x, int y)
        {
            tiles = new Tile[Height, Width];
            entities = new List<Entity>();
            this.x = x;
            this.y = y;

            try
            {
                using (StreamReader sr = new StreamReader("Resources/BaseZones/" + x.ToString() + "," + y.ToString() + "/tiles.dat"))
                {
                    float index = 0;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] segments = line.Split(' ');
                        if(segments.Length >= 2)
                        {
                            if(index < 128)
                            {
                                Trace.WriteLine("Added tile " + (int)index % Width + ", " + (int)Math.Floor(index / Width));
                                tiles[(int)Math.Floor(index / Width), (int)index % Width] = new Tile();
                                tiles[(int)Math.Floor(index / Width), (int)index % Width].symbol = segments[0].First();
                                tiles[(int)Math.Floor(index / Width), (int)index % Width].isSolid = Convert.ToBoolean(segments[1]);
                            }
                        }

                        index++;
                    }
                }
            }
            catch (IOException e)
            {
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);
            }
        }

        public Zone(int x, int y, Tile[,] tiles, List<Entity> entities)
        {
            this.x = x;
            this.y = y;
            this.tiles = tiles;
            this.entities = entities;
        }

        public void AddTile(int x, int y, Tile tile)
        {
            if(x >= 0 && x < Width && y >= 0 && y < Height)
            {
                tiles[y, x] = tile;
            }
        }
    }
}
