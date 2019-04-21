using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Torches
{
    class InventoryMenu
    {
        public static void Start(string name1, string name2, ref Dictionary<string, int> inv1, ref Dictionary<string, int> inv2)
        {
            int row = 0; // Row number, starting from top.
            bool left = inv1.Count > 0; // If the left column is selected.

            ConsoleKeyInfo key;
            do
            {
                CleanInventory(inv1);
                CleanInventory(inv2);

                // Make sure the selection is not on an empty item
                if (left)
                {
                    if (row >= inv1.Count)
                    {
                        if (inv1.Count > 0)
                        {
                            row = inv1.Count - 1;
                        }
                        else
                        {
                            left = false;
                            row = 0;
                        }
                    }
                }
                else
                {
                    if (row >= inv2.Count)
                    {
                        if (inv2.Count > 0)
                        {
                            row = inv2.Count - 1;
                        }
                        else
                        {
                            left = true;
                            row = 0;
                        }
                    }
                }

                PrintUI(name1, name2, inv1, inv2, left, row);

                // Handle different inputs
                key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.RightArrow)
                {
                    if (left && inv2.Count > 0)
                    {
                        left = false;

                        row = row < inv2.Count ? row : inv2.Count - 1;
                    }
                }
                else if (key.Key == ConsoleKey.LeftArrow)
                {
                    if (!left && inv1.Count > 0)
                    {
                        left = true;

                        row = row < inv1.Count ? row : inv1.Count - 1;
                    }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    if (row > 0)
                    {
                        row--;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (row < (left ? inv1.Count : inv2.Count) - 1)
                    {
                        row++;
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    // Swap items to the other inventory, ensure that only items with a count are moved.
                    if (left)
                    {
                        string item = inv1.ElementAt(row).Key;

                        if (inv1[item] <= 0)
                        {
                            left = false;
                        }
                        else
                        {
                            if (inv2.ContainsKey(item))
                            {
                                inv2[item]++;
                            }
                            else
                            {
                                inv2[item] = 1;
                            }

                            inv1[item]--;
                        }
                    }
                    else
                    {
                        string item = inv2.ElementAt(row).Key;

                        if (inv2[item] <= 0)
                        {
                            left = true;
                        }
                        else
                        {
                            if (inv1.ContainsKey(item))
                            {
                                inv1[item]++;
                            }
                            else
                            {
                                inv1[item] = 1;
                            }

                            inv2[item]--;
                        }
                    }
                }

            } while (key.Key != ConsoleKey.Escape);

            // Clear output when done.
            for (int i = 0; i < inv1.Count + inv2.Count + 5; i++)
            {
                Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + i, new string(' ', 100), Color.LightGray);
            }
        }

        private static void PrintUI(string name1, string name2, Dictionary<string, int> inv1, Dictionary<string, int> inv2, bool left, int row)
        {
            Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY,
                $@"Inventory Menu -- Use ARROW KEYS to select an item            
Press ENTER to swap an item to the other inventory, Hold ESC to finish.
_________________________________________________________________
{name1.PadRight(30)}|{name2.PadRight(60)}
-----------------------------------------------------------------", Color.LightGray);

            for (int i = 0; i < inv1.Count + inv2.Count; i++)
            {
                if (i < inv1.Count)
                {
                    if (left && row == i)
                    {
                        Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + i + 5,
                        $"{inv1.ElementAt(i).Value}x {inv1.ElementAt(i).Key}".PadRight(30) + "|", Color.DarkOrange);
                    }
                    else
                    {
                        Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + i + 5,
                        $"{inv1.ElementAt(i).Value}x {inv1.ElementAt(i).Key}".PadRight(30) + "|", Color.LightGray);
                    }
                }
                else
                {
                    Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY + i + 5,
                        new string(' ', 30) + "|", Color.LightGray);
                }
            }

            for (int i = 0; i < inv1.Count + inv2.Count; i++)
            {
                if (i < inv2.Count)
                {
                    if (!left && row == i)
                    {
                        Renderer.PrintAt(Constants.TextOutputX + 30, Constants.TextOutputY + i + 5,
                        $"{inv2.ElementAt(i).Value}x {inv2.ElementAt(i).Key}".PadRight(30), Color.DarkOrange);
                    }
                    else
                    {
                        Renderer.PrintAt(Constants.TextOutputX + 30, Constants.TextOutputY + i + 5,
                        $"{inv2.ElementAt(i).Value}x {inv2.ElementAt(i).Key}".PadRight(30), Color.LightGray);
                    }
                }
                else
                {
                    Renderer.PrintAt(Constants.TextOutputX + 30, Constants.TextOutputY + i + 5,
                        new string(' ', 30), Color.LightGray);
                }
            }
        }

        public static void CleanInventory(Dictionary<string, int> inventory)
        {
            List<string> toRemove = inventory.Where(pair => pair.Value <= 0)
                         .Select(pair => pair.Key)
                         .ToList();

            foreach (var key in toRemove)
            {
                inventory.Remove(key);
            }
        }
    }
}