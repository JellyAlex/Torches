using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;
using System.Drawing;
using Torches.ECS;

namespace Torches
{
    public class Zone
    {
        public const int Width = 16;
        public const int Height = 8;

        public int x { get; protected set; }
        public int y { get; protected set; }

        // List of tiles, indexed with tiles[y, x].
        public Tile[,] tiles { get; set; }
        protected List<Entity> entities { get; set; }

        // Represents if a door is open in a zone, start from the right and go counterclockwise
        // (ie. right, up, left, down).
        public bool[] doors { get; protected set; }

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

            // Load zone from file.
            try
            {
                // Load tiles from file
                using (StreamReader sr = new StreamReader("Resources/BaseZones/" + x.ToString() + "," + y.ToString() + "/tiles.dat"))
                {
                    float index = -1;
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] segments = line.Split(' ');

                        if (index == -1 )
                        {
                            if(segments.Length == 4)
                            {
                                bool right, up, left, down;

                                if(bool.TryParse(segments[0], out right) &&
                                    bool.TryParse(segments[1], out up) &&
                                    bool.TryParse(segments[2], out left) &&
                                    bool.TryParse(segments[3], out down)
                                    )
                                {
                                    doors = new bool[4] { right, up, left, down };
                                }
                                else
                                {
                                    Trace.WriteLine($"Error: Error while parsing zone line {index + 2}: {line}");
                                    doors = new bool[4] { false, false, false, false };
                                }
                            }
                            else
                            {
                                Trace.WriteLine($"Error: Incorrect number of parameters in zone tiles, line {index + 2}: {line}");
                            }
                        }
                        else
                        {
                            if (index < Width * Height)
                            {
                                if (segments.Length >= 2)
                                {
                                    // Create tile, input data from text file
                                    tiles[(int)Math.Floor(index / Width), (int)index % Width] = new Tile();
                                    tiles[(int)Math.Floor(index / Width), (int)index % Width].symbol = segments[0].First();
                                    tiles[(int)Math.Floor(index / Width), (int)index % Width].isSolid = Convert.ToBoolean(segments[1]);
                                }
                                else
                                {
                                    Trace.WriteLine("Error: Invalid entity tile \"" + line + "\"");
                                }
                            }
                            else
                            {
                                Trace.WriteLine("Error: Invalid tile file, too many lines");
                                break;
                            }
                        }

                        index++;
                    }
                }

                // Load entities from file
                using (StreamReader sr = new StreamReader("Resources/BaseZones/" + x.ToString() + "," + y.ToString() + "/entities.dat"))
                {
                    // Stores the index of the entity whose components are being filled.
                    int currentIndex = -1;

                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Skip commented or blank lines.
                        if(line.Length >= 2)
                        {
                            if (line.TrimStart().Substring(0, 2) == "//" || line.Trim() == "")
                            {
                                continue;
                            }

                            // Get strings left and right side of '='.
                            string[] segments = line.Split('=');
                            if (segments.Length == 1 || segments.Length == 2)
                            {
                                if (line == "Entity")
                                {
                                    currentIndex++;
                                    entities.Add(new Entity());
                                }
                                else if (currentIndex >= 0 && entities.Count == currentIndex + 1)
                                {
                                    string component = segments[0].Trim();
                                    string[] componentData = segments[1].Split(',');

                                    // Remove leading and trailing whitespace from component data.
                                    for (int i = 0; i < componentData.Length; i++)
                                    {
                                        componentData[i] = componentData[i].Trim();
                                    }

                                    if (component == "Flags")
                                    {
                                        foreach (string flag in componentData)
                                        {
                                            switch (flag)
                                            {
                                                case "Solid":
                                                    entities[currentIndex].flags |= EntityFlags.Solid;
                                                    break;
                                                case "Player":
                                                    entities[currentIndex].flags |= EntityFlags.Player;
                                                    break;
                                                case "Tribesman":
                                                    entities[currentIndex].flags |= EntityFlags.Tribesman;
                                                    break;
                                                default:
                                                    Trace.WriteLine("Error: Unknown flag '" + flag + "'");
                                                    break;
                                            }

                                        }
                                    }
                                    else if (component == "ZonePosition")
                                    {
                                        // Ensure there are only 2 parameters.
                                        if (componentData.Length == 2)
                                        {
                                            if (int.TryParse(componentData[0], out int tx) &&
                                                int.TryParse(componentData[1], out int ty))
                                            {
                                                // Add the ZonePosition component to the current entity
                                                entities[currentIndex].AddComponent(new ZonePosition(tx, ty));
                                            }
                                            else
                                            {
                                                Trace.WriteLine("Error: Error while converting ZonePosition parameters.");
                                            }
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: ZonePosition takes 2 parameters.");
                                        }
                                    }
                                    else if (component == "Symbol")
                                    {
                                        if (componentData.Length == 1)
                                        {
                                            entities[currentIndex].AddComponent(new Symbol(componentData[0].First()));
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Symbol takes 1 parameters.");
                                        }
                                    }
                                    else if (component == "Health")
                                    {
                                        if (componentData.Length == 2)
                                        {
                                            if (int.TryParse(componentData[0], out int health) &&
                                                int.TryParse(componentData[1], out int maxHealth))
                                            {
                                                entities[currentIndex].AddComponent(new Health(health, maxHealth));
                                            }
                                            else
                                            {
                                                Trace.WriteLine("Error: Error while converting Health parameters");
                                            }
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Health takes 2 parameters.");
                                        }
                                    }
                                    else if (component == "Character")
                                    {
                                        if (componentData.Length == 1)
                                        {
                                            entities[currentIndex].AddComponent(new Character(componentData[0]));
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Character takes 1 parameter.");
                                        }
                                    }
                                    else if (component == "Inventory")
                                    {
                                        entities[currentIndex].AddComponent(new Inventory());
                                    }
                                    else if (component == "Colour")
                                    {
                                        if (componentData.Length == 1)
                                        {
                                            entities[currentIndex].AddComponent(
                                                new Colour(Color.FromName(componentData.First()))
                                                );
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Colour takes 1 parameter.");
                                        }
                                    }
                                    else if (component == "Enemy")
                                    {
                                        if(componentData.Length == 3)
                                        {
                                            string[] playerAttackPatternRaw = componentData[0].Split('|');
                                            string[] playerDefencePatternRaw = componentData[1].Split('|');

                                            int[] playerAttackPattern = new int[playerAttackPatternRaw.Length];
                                            int[] playerDefencePattern = new int[playerDefencePatternRaw.Length];

                                            for (int i = 0; i < playerAttackPatternRaw.Length; i++)
                                            {
                                                if(!int.TryParse(playerAttackPatternRaw[i], out playerAttackPattern[i]))
                                                    Trace.WriteLine("Error: Error while parsing int for attack pattern.");

                                                if (!int.TryParse(playerDefencePatternRaw[i], out playerDefencePattern[i]))
                                                    Trace.WriteLine("Error: Error while parsing int for defence pattern.");
                                            }

                                            int delay;
                                            if(!int.TryParse(componentData[2], out delay))
                                            {
                                                Trace.WriteLine("Error: Error while parsing int for delay.");
                                            }

                                            entities[currentIndex].AddComponent(
                                                new Enemy(playerAttackPattern, playerDefencePattern, delay));
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Enemy takes 2 parameters.");
                                        }
                                    }
                                }
                                else
                                {
                                    Trace.WriteLine("Error: Invalid Entity File (Resources/BaseZones/" + x.ToString() + ", " + y.ToString() + " /entities.dat)");
                                }

                            } // end if (segments.Length == 1 || segments.Length == 2)

                        } // end if(line.Length >= 2)

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
            // Make sure tile is within the bounds of the zone
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
                if (e.HasFlag(EntityFlags.Solid))
                {
                    if (e.GetComponent<ECS.ZonePosition>().x == x && e.GetComponent<ECS.ZonePosition>().y == y)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public Entity GetEntityAt(int x, int y)
        {
            // Check if coordinate is within zone
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return null;
            }

            // Check if an entity exists at the coord
            foreach (Entity e in entities)
            {
                if(e.HasComponent<ECS.ZonePosition>())
                {
                    if (e.GetComponent<ECS.ZonePosition>().x == x && e.GetComponent<ECS.ZonePosition>().y == y)
                    {
                        return e;
                    }
                }
            }

            return null;
        }

        public void Render()
        {
            // Render tiles.
            Console.CursorVisible = false;
            for (int yi = 0; yi < Height; yi++)
            {
                for (int xi = 0; xi < Width; xi++)
                {
                    RenderTile(xi, yi);
                }
            }

            // Render entities.
            foreach(Entity e in entities)
            {
                Renderer.RenderEntity(e);
            }
            Console.CursorVisible = true;
            
            RenderDoors();
        }

        public void RenderTile(int x, int y)
        {
            Renderer.PrintAt(Constants.MapX + x, Constants.MapY + (Height - y - 1), tiles[y, x].symbol, Color.LightGray);
        }

        public void RenderDoors()
        {
            // Right door.
            if (doors[0])
            {
                Renderer.PrintAt(Constants.MapX + Width, Constants.MapY + Height / 2 - 1, ':', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + Width, Constants.MapY + Height / 2 - 1, '|', Color.LightGray);
            }

            // Top door.
            if (doors[1])
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY - 1, '.', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY - 1, '.', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY - 1, '-', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY - 1, '-', Color.LightGray);
            }

            // Left door.
            if (doors[2])
            {
                Renderer.PrintAt(Constants.MapX - 1, Constants.MapY + Height / 2 - 1, ':', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX - 1, Constants.MapY + Height / 2 - 1, '|', Color.LightGray);
            }

            // Bottom door.
            if (doors[3])
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY + Height, '.', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY + Height, '.', Color.LightGray);
            }
            else
            {
                Renderer.PrintAt(Constants.MapX + Width / 2 - 1, Constants.MapY + Height, '-', Color.LightGray);
                Renderer.PrintAt(Constants.MapX + Width / 2    , Constants.MapY + Height, '-', Color.LightGray);
            }
        }

    }
}
