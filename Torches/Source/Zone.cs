using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Torches
{
    public class Zone
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
            x = 0;
            y = 0;
        }

        public Zone(int x, int y)
        {
            tiles = new Tile[Height, Width];
            entities = new List<Entity>();
            this.x = x;
            this.y = y;

            try
            {
                // Load tiles from file
                using (StreamReader sr = new StreamReader("Resources/BaseZones/" + x.ToString() + "," + y.ToString() + "/tiles.dat"))
                {
                    float index = 0;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] segments = line.Split(' ');

                        if (index < 128)
                        {
                            if (segments.Length >= 2)
                            {
                                Trace.WriteLine("Added tile " + (int)index % Width + ", " + (int)Math.Floor(index / Width));
                                tiles[(int)Math.Floor(index / Width), (int)index % Width] = new Tile();
                                tiles[(int)Math.Floor(index / Width), (int)index % Width].symbol = segments[0].First();
                                tiles[(int)Math.Floor(index / Width), (int)index % Width].isSolid = Convert.ToBoolean(segments[1]);
                            }
                            else
                            {
                                Trace.WriteLine("Invalid entity tile \"" + line + "\"");
                            }
                        }
                        else
                        {
                            Trace.WriteLine("Invalid tile file, too many lines");
                            break;
                        }

                        index++;
                    }
                }

                // Load entities from file
                using (StreamReader sr = new StreamReader("Resources/BaseZones/" + x.ToString() + "," + y.ToString() + "/entities.dat"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] segments = line.Split(' ');
                        if (segments.Length >= 5)
                        {
                            try
                            {
                                Entity entity = new Entity(segments[0].First(), Convert.ToBoolean(segments[1]), 
                                    Convert.ToInt32(segments[2]), Convert.ToInt32(segments[3]));

                                Trace.WriteLine("Added entity");
                            }
                            catch (Exception e)
                            {
                                Trace.WriteLine("ERROR: Invalid entity line: " + e.Message);
                                Game.Stop();
                            }
                        }
                        else
                        {
                            Trace.WriteLine("Invalid entity entry \"" + line + "\"");
                        }
                    }
                }
            }
            catch (IOException e)
            {
                Trace.WriteLine("The file could not be read:");
                Trace.WriteLine(e.Message);

                Game.Stop();
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

        public bool IsSolidAt(int x, int y)
        {
            // Check if coordinate is within zone
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return true;
            }

            // Check if the tile at coord is solid
            if (tiles[y, x].isSolid)
            {
                return true;
            }
            
            // Check if a solid entity exists at the coord
            foreach (Entity e in entities)
            {
                if (e.isSolid)
                {
                    if (e.x == x && e.y == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public void Render()
        {
            for (int yi = 0; yi < Height; yi++)
            {
                for (int xi = 0; xi < Width; xi++)
                {
                    Renderer.PrintAt(Constants.MapX + xi, Constants.MapY + (Height - yi - 1), tiles[yi, xi].symbol, Color.LightGray);
                }
            }

            foreach(Entity e in entities)
            {
                e.Render();
            }
        }
    }
}
