using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Drawing;
using System.Diagnostics;

namespace Torches.ECS
{
    class MerchantSystem : ISystem
    {
        public void Update(ref World world)
        {
            
        }
        public bool UpdateCommand(string[] segments, ref World world)
        {
            if (segments[0] == "interact" || segments[0] == "i")
            {
                if (segments.Length >= 2)
                {
                    string direction = segments[1].ToLower().Trim();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        return HandleMerchant(ref world, 1, 0);
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        return HandleMerchant(ref world, -1, 0);
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        return HandleMerchant(ref world, 0, 1);
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        return HandleMerchant(ref world, 0, -1);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
                else
                {
                    // Check for the tribesman in all four directions.
                    if (!HandleMerchant(ref world, 1, 0))
                        if (!HandleMerchant(ref world, -1, 0))
                            if (!HandleMerchant(ref world, 0, 1))
                                return HandleMerchant(ref world, 0, -1);
                            else return true;
                        else return true;
                    else return true;
                }
            }


            return false;
        }


        private bool HandleMerchant(ref World world, int dx, int dy)
        {

            Entity merchant = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
            if (merchant == null)
            {
                return false;
            }
            else if (merchant.HasFlag(EntityFlags.Merchant))
            {
                // Links the item to it's cost.
                Dictionary<string, int> itemsForSale = new Dictionary<string, int>();
                // Links the weapon to it's cost.
                List<Tuple<Weapon, int>> weaponsForSale = new List<Tuple<Weapon, int>>();

                using (StreamReader sr = new StreamReader("Resources/SystemData/merchant.dat"))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] segments = line.Split(',');

                        if(segments[0] == "Item")
                        {
                            if (segments.Length == 3)
                            {
                                if (int.TryParse(segments[1], out int cost))
                                {
                                    itemsForSale[segments[2]] = cost;
                                }
                                else
                                {
                                    Trace.WriteLine($"Error: Invalid merchant.dat cost entry '{segments[1]}'.");
                                }
                            }
                        }
                        else if(segments[0] == "Weapon")
                        {
                            if (segments.Length == 4)
                            {
                                if(int.TryParse(segments[1], out int cost) && int.TryParse(segments[3], out int damage))
                                {
                                    weaponsForSale.Add(new Tuple<Weapon, int>(new Weapon(segments[2], damage), cost));
                                }
                            }
                        }
                        else
                        {
                            Trace.WriteLine($"Error: Invalid merchant.dat entry '{segments[0]}'.");
                        }
                        
                    }

                    // Ensure the merchant has items the player can buy;
                    if(itemsForSale.Count == 0 && weaponsForSale.Count == 0)
                    {
                        Trace.WriteLine("Warning: Merchant has no items to sell.");
                        return true;
                    }

                    int row = 0;
                    ConsoleKeyInfo key;

                    do
                    {
                        PrintUI(itemsForSale, weaponsForSale, row);

                        // Handle keyboard input
                        key = Console.ReadKey(true);
                        if (key.Key == ConsoleKey.DownArrow)
                        {
                            if (row < itemsForSale.Count + weaponsForSale.Count - 1)
                            {
                                row++;
                            }
                        }
                        else if (key.Key == ConsoleKey.UpArrow)
                        {
                            if (row > 0)
                            {
                                row--;
                            }
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            // Buy item/weapon.
                            if (row < itemsForSale.Count)
                            {
                                // Buy an item.
                                if(world.GetPlayer().GetComponent<Coins>().coins - itemsForSale.ElementAt(row).Value >= 0)
                                {
                                    world.GetPlayer().GetComponent<Coins>().coins -= itemsForSale.ElementAt(row).Value;
                                    if (world.GetPlayer().GetComponent<Inventory>().items.ContainsKey(itemsForSale.ElementAt(row).Key))
                                    {
                                        world.GetPlayer().GetComponent<Inventory>().items[itemsForSale.ElementAt(row).Key] += 1;
                                    }
                                    else
                                    {
                                        world.GetPlayer().GetComponent<Inventory>().items[itemsForSale.ElementAt(row).Key] = 1;
                                    }
                                }
                            }
                            else
                            {
                                // Buy a weapon.
                                if(world.GetPlayer().GetComponent<Coins>().coins - weaponsForSale.ElementAt(row - itemsForSale.Count).Item2 >= 0)
                                {
                                    // Player can only buy a weapon with a greater damage.
                                    if(world.GetPlayer().GetComponent<Weapon>().damage < weaponsForSale.ElementAt(row - itemsForSale.Count).Item1.damage)
                                    {
                                        world.GetPlayer().GetComponent<Coins>().coins -= weaponsForSale.ElementAt(row - itemsForSale.Count).Item2;
                                        world.GetPlayer().GetComponent<Weapon>().name = weaponsForSale.ElementAt(row - itemsForSale.Count).Item1.name;
                                        world.GetPlayer().GetComponent<Weapon>().damage = weaponsForSale.ElementAt(row - itemsForSale.Count).Item1.damage;
                                    }
                                }
                            }
                            Renderer.RenderPlayerInfo(world.GetPlayer());
                        }

                    } while (key.Key != ConsoleKey.Escape);

                    // Clear output field
                    for(int i = 0; i < weaponsForSale.Count + itemsForSale.Count + 5; i++)
                    {
                        Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + i, new string(' ', 100), Color.LightGray);
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        private void PrintUI(Dictionary<string, int> itemsForSale, List<Tuple<Weapon, int>> weaponsForSale, int row)
        {
            Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY,
$@"Merchant Buy Menu -- Use ARROW KEYS to select an item            
Press ENTER to buy an item, Hold ESC to finish.
_________________________________________________________________
 Cost (Coins)                 | Item / Weapon                           
_________________________________________________________________", Color.LightGray);

            // Print items.
            for (int i = 0; i < itemsForSale.Count; i++)
            {
                if(i == row)
                {
                    Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + 5 + i,
                    $"{itemsForSale.ElementAt(i).Value.ToString().PadRight(30)}| 1x {itemsForSale.ElementAt(i).Key.PadRight(56)}"
                    , Color.DarkOrange);
                }
                else
                {
                    Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + 5 + i,
                    $"{itemsForSale.ElementAt(i).Value.ToString().PadRight(30)}| 1x {itemsForSale.ElementAt(i).Key.PadRight(56)}"
                    , Color.LightGray);
                }
            }

            // Print weapons below items.
            for (int i = itemsForSale.Count; i < weaponsForSale.Count + itemsForSale.Count; i++)
            {
                if (i == row)
                {
                    Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + 5 + i,
                    $"{weaponsForSale.ElementAt(i - itemsForSale.Count).Item2.ToString().PadRight(30)}| {weaponsForSale.ElementAt(i - itemsForSale.Count).Item1.name} (dmg={weaponsForSale.ElementAt(i - itemsForSale.Count).Item1.damage})                      "
                    , Color.DarkOrange);
                }
                else
                {
                    Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + 5 + i,
                    $"{weaponsForSale.ElementAt(i - itemsForSale.Count).Item2.ToString().PadRight(30)}| {weaponsForSale.ElementAt(i - itemsForSale.Count).Item1.name} (dmg={weaponsForSale.ElementAt(i - itemsForSale.Count).Item1.damage})                      "
                    , Color.LightGray);
                }
            }
        }
    }
}
