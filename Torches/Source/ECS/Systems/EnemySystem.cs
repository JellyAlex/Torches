using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Threading;
using System.Drawing;
using System.Diagnostics;

namespace Torches.ECS
{
    class EnemySystem : ISystem
    {
        public bool Update(string[] segments, ref World world)
        {
            if (segments[0] == "interact" || segments[0] == "i" || segments[0] == "attack" || segments[0] == "a")
            {
                if (segments.Length >= 2)
                {
                    string direction = segments[1].ToLower().Trim();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        return HandleEnemy(ref world, 1, 0);
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        return HandleEnemy(ref world, -1, 0);
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        return HandleEnemy(ref world, 0, 1);
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        return HandleEnemy(ref world, 0, -1);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
                else
                {
                    // Check for the tribesman in all four directions.
                    if (!HandleEnemy(ref world, 1, 0))
                        if (!HandleEnemy(ref world, -1, 0))
                            if (!HandleEnemy(ref world, 0, 1))
                                return HandleEnemy(ref world, 0, -1);
                            else return true;
                        else return true;
                    else return true;
                }
            }

            return false;
        }

        private bool HandleEnemy(ref World world, int dx, int dy)
        {
            // Get the entity at the specified position
            Entity enemy = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<ZonePosition>().x + dx, world.GetPlayer().GetComponent<ZonePosition>().y + dy);
            // Check if an entity is found
            if (enemy == null)
            {
                return false;
            }
            else if (enemy.HasComponent<Enemy>())
            {
                if(enemy.HasComponent<Character>())
                {
                    // Print the enemy's name
                    Renderer.PrintGameOutput($"          {enemy.GetComponent<Character>().name}".PadRight(100) + "\n" +
                        "__________" + new string('_', enemy.GetComponent<Character>().name.Length) + "__________".PadRight(100) + "\n"
                        + "Press SPACE to attack, Hold ESC to retreat.");
                    

                    int scrollIndex = 0;

                    Console.CursorVisible = false;

                    // Create a stopwatch to keep track of delay between loops
                    Stopwatch stopwatch = new Stopwatch();

                    while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                    {
                        for (int i = 0; i < enemy.GetComponent<Enemy>().playerAttackPattern.Length; i++)
                        {
                            if (i == scrollIndex)
                            {
                                Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '|',
                                    Color.White);
                            }
                            else
                            {
                                // TODO: fix colouring
                                //Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '#', Color.FromArgb());
                            }
                        }

                        if (scrollIndex == enemy.GetComponent<Enemy>().playerAttackPattern.Length - 1)
                        {
                            scrollIndex = 0;
                        }
                        else
                        {
                            scrollIndex++;
                        }
                        
                        // Delay using the stopwatch's elapsed time.
                        stopwatch.Start();
                        while(stopwatch.ElapsedMilliseconds < enemy.GetComponent<Enemy>().delay)
                        {
                            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                            {


                                Trace.WriteLine($"Space triggered, scroll index: {scrollIndex}");
                            }
                            if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape)
                            {
                                // Clear output fields
                                Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.White);
                                Renderer.PrintGameOutput(
                                    "Enter a command... ".PadRight(100) + "\n" +
                                    new string(' ', 100) + "\n" +
                                    new string(' ', 100) + "\n" +
                                    new string(' ', 100) + "\n");

                                return true;
                            }
                        }
                        stopwatch.Reset();
                    }

                    Console.CursorVisible = true;
                }
                else
                {
                    // Print an enemy with name "enemy"
                    Renderer.PrintGameOutput($"          Enemy".PadRight(100) + "\n" + "_________________________\n");
                }

                

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
