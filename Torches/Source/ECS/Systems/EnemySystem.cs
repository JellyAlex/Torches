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
                    // Check for the enemy in all four directions.
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
                // Ensure the enemy is able to be killed.
                if (!enemy.HasComponent<Health>() || !enemy.HasComponent<Damager>())
                    return false;

                // The index of the vertical line which the player tries to stop in the right region.
                int scrollIndex = 0;
                
                bool isAttacking = true;
                Console.CursorVisible = false;

                // Create a stopwatch to keep track of delay between loops
                Stopwatch stopwatch = new Stopwatch();

                // Store the enemy details for easy, efficient access.
                Enemy enemyCP = enemy.GetComponent<Enemy>();
                
                PrintEnemyDetails(enemy, isAttacking);

                while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                {
                    if(isAttacking)
                    {
                        for (int i = 0; i < enemyCP.playerAttackPattern.Length; i++)
                        {
                            if (i == scrollIndex)
                            {
                                Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '|', Color.White);
                            }
                            else
                            {
                                Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '#', Color.FromArgb(51 * enemyCP.playerAttackPattern[i], 255, 255 - 51 * enemyCP.playerAttackPattern[i]));
                            }
                        }

                        if (scrollIndex == enemyCP.playerAttackPattern.Length - 1)
                        {
                            scrollIndex = 0;
                        }
                        else
                        {
                            scrollIndex++;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < enemyCP.playerDefendPattern.Length; i++)
                        {
                            if (i == scrollIndex)
                            {
                                Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '|', Color.White);
                            }
                            else
                            {
                                Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '#', Color.FromArgb(255, 51 * enemyCP.playerDefendPattern[i], 255 - 51 * enemyCP.playerDefendPattern[i]));
                            }
                        }

                        if (scrollIndex == enemyCP.playerDefendPattern.Length - 1)
                        {
                            scrollIndex = 0;
                        }
                        else
                        {
                            scrollIndex++;
                        }
                    }

                    // Delay using the stopwatch's elapsed time.
                    stopwatch.Start();
                    while (stopwatch.ElapsedMilliseconds < enemyCP.delay)
                    {
                        if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                        {
                            // Player attack action

                            // TODO: Implement weapons with different damage values.
                            if (isAttacking)
                            {
                                if(enemy.GetComponent<Health>().health - world.GetPlayer().GetComponent<Damager>().damage * enemyCP.playerAttackPattern[scrollIndex] > 0)
                                {
                                    enemy.GetComponent<Health>().health -= world.GetPlayer().GetComponent<Damager>().damage * enemyCP.playerAttackPattern[scrollIndex];
                                }
                                else
                                {
                                    enemy.GetComponent<Health>().health = 0;
                                    // TODO: Kill enemy and end battle
                                    Trace.WriteLine("Player won!");
                                }

                                isAttacking = false;
                                PrintEnemyDetails(enemy, isAttacking);
                                // Clear text input
                                Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
                                Console.WriteLine(new string(' ', 100));
                            }
                            else
                            {
                                if (world.GetPlayer().GetComponent<Health>().health - enemy.GetComponent<Damager>().damage * enemyCP.playerDefendPattern[scrollIndex] > 0)
                                {
                                    world.GetPlayer().GetComponent<Health>().health -= enemy.GetComponent<Damager>().damage * enemyCP.playerDefendPattern[scrollIndex];
                                }
                                else
                                {
                                    world.GetPlayer().GetComponent<Health>().health = 0;
                                    // TODO: kill player & restart.
                                    Trace.WriteLine("Player lost :(");
                                }

                                isAttacking = true;
                                PrintEnemyDetails(enemy, isAttacking);
                                // Clear text input
                                Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
                                Console.WriteLine(new string(' ', 100));
                            }
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

                return true;
            }
            else
            {
                return false;
            }
        }

        private void PrintEnemyDetails(Entity enemy, bool isAttacking)
        {
            string name;
            if (enemy.HasComponent<Character>())
            {
                name = enemy.GetComponent<Character>().name;
            }
            else
            {
                name = "Enemy";
            }

            if(isAttacking)
            {
                name = "Attacking " + name + $" - {enemy.GetComponent<Health>().health} / {enemy.GetComponent<Health>().maxHealth}";
            }
            else
            {
                name = "Defending against " + name + $" - {enemy.GetComponent<Health>().health} / {enemy.GetComponent<Health>().maxHealth}";
            }

            // Print the enemy's name
            Renderer.PrintGameOutput($"          {name}".PadRight(100) + "\n" +
                "__________" + new string('_', name.Length) + "__________".PadRight(100) + "\n"
                + "Press SPACE to attack, Hold ESC to retreat.");
        }
    }
}
