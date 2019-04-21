using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Torches.ECS;
using System.IO;
using System.Diagnostics;
using System.Drawing;

namespace Torches
{
    class ResourceManager
    {
        public static Zone LoadZoneFromRes(int x, int y, World world)
        {
            Zone zone = new Zone(x, y, world);

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

                        if (index == -1)
                        {
                            if (segments.Length == 4)
                            {
                                if (bool.TryParse(segments[0], out bool right) &&
                                    bool.TryParse(segments[1], out bool up) &&
                                    bool.TryParse(segments[2], out bool left) &&
                                    bool.TryParse(segments[3], out bool down)
                                    )
                                {
                                    zone.Doors = new bool[4] { right, up, left, down };
                                }
                                else
                                {
                                    Trace.WriteLine($"Error: Error while parsing zone line {index + 2}: {line}");
                                    zone.Doors = new bool[4] { false, false, false, false };
                                }
                            }
                            else
                            {
                                Trace.WriteLine($"Error: Incorrect number of parameters in zone tiles, line {index + 2}: {line}");
                            }
                        }
                        else
                        {
                            if (index < Zone.Width * Zone.Height)
                            {
                                if (segments.Length >= 2)
                                {
                                    // Create tile, input data from text file
                                    zone.Tiles[(int)Math.Floor(index / Zone.Width), (int)index % Zone.Width] = new Tile
                                    {
                                        Symbol = segments[0].First(),
                                        IsSolid = Convert.ToBoolean(segments[1])
                                    };
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
                        if (line.Length >= 2)
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
                                    zone.Entities.Add(new Entity());
                                }
                                else if (currentIndex >= 0 && zone.Entities.Count == currentIndex + 1)
                                {
                                    string component = segments[0].Trim();
                                    string[] componentData = segments[1].Split(',');

                                    // Remove leading and trailing whitespace from component data.
                                    for (int i = 0; i < componentData.Length; i++)
                                    {
                                        componentData[i] = componentData[i].Trim();
                                    }

                                    Trace.WriteLine($"Adding component {component} to entity {currentIndex} zone {x} {y}");

                                    if (component == "Flags")
                                    {
                                        foreach (string flag in componentData)
                                        {
                                            switch (flag)
                                            {
                                                case "Solid":
                                                    zone.Entities[currentIndex].flags |= EntityFlags.Solid;
                                                    break;
                                                case "Player":
                                                    zone.Entities[currentIndex].flags |= EntityFlags.Player;
                                                    break;
                                                case "Tribesman":
                                                    zone.Entities[currentIndex].flags |= EntityFlags.Tribesman;
                                                    break;
                                                case "Loot":
                                                    zone.Entities[currentIndex].flags |= EntityFlags.Loot;
                                                    break;
                                                case "DigSpot":
                                                    zone.Entities[currentIndex].flags |= EntityFlags.DigSpot;
                                                    break;
                                                default:
                                                    Trace.WriteLine("Error: Unknown flag '" + flag + "'");
                                                    break;
                                            }

                                        }
                                    }
                                    else if (component == "Position")
                                    {
                                        // Ensure there are only 2 parameters.
                                        if (componentData.Length == 2)
                                        {
                                            if (int.TryParse(componentData[0], out int tx) &&
                                                int.TryParse(componentData[1], out int ty))
                                            {
                                                // Add the Position component to the current entity
                                                zone.Entities[currentIndex].AddComponent(new Position(tx, ty));
                                            }
                                            else
                                            {
                                                Trace.WriteLine("Error: Error while converting Position parameters.");
                                            }
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Position takes 2 parameters.");
                                        }
                                    }
                                    else if (component == "Symbol")
                                    {
                                        if (componentData.Length == 1)
                                        {
                                            zone.Entities[currentIndex].AddComponent(new Symbol(componentData[0].First()));
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
                                                zone.Entities[currentIndex].AddComponent(new Health(health, maxHealth));
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
                                            zone.Entities[currentIndex].AddComponent(new Character(componentData[0]));
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Character takes 1 parameter.");
                                        }
                                    }
                                    else if (component == "Colour")
                                    {
                                        if (componentData.Length == 1)
                                        {
                                            zone.Entities[currentIndex].AddComponent(
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
                                        if (componentData.Length == 3)
                                        {
                                            // Load attack/defend patterns from file
                                            string[] playerAttackPatternRaw = componentData[0].Split('|');
                                            string[] playerDefendPatternRaw = componentData[1].Split('|');

                                            int[] playerAttackPattern = new int[playerAttackPatternRaw.Length];
                                            int[] playerDefendPattern = new int[playerDefendPatternRaw.Length];

                                            // Convert arrays from string[] to int[].
                                            for (int i = 0; i < playerAttackPatternRaw.Length; i++)
                                            {
                                                if (!int.TryParse(playerAttackPatternRaw[i], out playerAttackPattern[i]))
                                                    Trace.WriteLine("Error: Error while parsing int for attack pattern.");
                                            }

                                            for (int i = 0; i < playerDefendPatternRaw.Length; i++)
                                            {
                                                if (!int.TryParse(playerDefendPatternRaw[i], out playerDefendPattern[i]))
                                                    Trace.WriteLine("Error: Error while parsing int for defend pattern.");
                                            }

                                            // Get delay from file.
                                            if (!int.TryParse(componentData[2], out int delay))
                                            {
                                                Trace.WriteLine("Error: Error while parsing int for delay.");
                                            }

                                            zone.Entities[currentIndex].AddComponent(
                                                new Enemy(playerAttackPattern, playerDefendPattern, delay));
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Enemy takes 2 parameters.");
                                        }
                                    }
                                    else if (component == "Damager")
                                    {
                                        if (componentData.Length == 1)
                                        {
                                            if (int.TryParse(componentData[0], out int damage))
                                            {
                                                zone.Entities[currentIndex].AddComponent(new Damager(damage));
                                            }
                                            else
                                            {
                                                Trace.WriteLine("Error: Error while converting Damager parameters");
                                            }
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Damager takes 1 parameter.");
                                        }
                                    }
                                    else if (component == "Inventory")
                                    {
                                        Inventory inventory = new Inventory();

                                        if (componentData.Length == 0)
                                        {
                                            Trace.WriteLine("Inventory Component data length 0");
                                        }

                                        foreach (string rawItemStack in componentData)
                                        {
                                            string[] rawItemStackSplit = rawItemStack.Split(' ');
                                            if (rawItemStackSplit.Length == 2)
                                            {
                                                if (int.TryParse(rawItemStackSplit[0], out int itemCount))
                                                {
                                                    inventory.items.Add(rawItemStackSplit[1], itemCount);
                                                    Trace.WriteLine($"Added item stack: {rawItemStack}");
                                                }
                                                else
                                                {
                                                    Trace.WriteLine($"Error: Invalid item stack: {rawItemStack}");
                                                }
                                            }
                                            else
                                            {
                                                Trace.WriteLine($"Error: Invalid item stack: {rawItemStack}");
                                            }
                                        }
                                        
                                        Trace.WriteLine("Adding inventory component.");
                                        zone.Entities[currentIndex].AddComponent(inventory);
                                    }
                                    else if (component == "Weapon")
                                    {
                                        if (componentData.Length == 2)
                                        {
                                            if(int.TryParse(componentData[1], out int damage))
                                            {
                                                zone.Entities[currentIndex].AddComponent(new Weapon(componentData[0], damage));
                                            }
                                            else
                                            {
                                                Trace.WriteLine($"Error: Invalid weapon damage: {componentData[1]}");
                                            }
                                        }
                                        else if (componentData.Length == 0)
                                        {
                                            zone.Entities[currentIndex].AddComponent(new Weapon());
                                        }
                                        else
                                        {
                                            Trace.WriteLine("Error: Invalid number of parameters on weapon component.");
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
                Trace.WriteLine($"Error loading zone file: {x}, {y}");
                Trace.WriteLine(e.Message);

                Game.Stop(true);
            }

            return zone;
        }
    }
}
