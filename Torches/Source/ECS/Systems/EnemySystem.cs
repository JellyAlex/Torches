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
            Entity enemy = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
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
                
                
                bool isAttacking = true;
                Console.CursorVisible = false;

                // Create a stopwatch to keep track of delay between loops
                Stopwatch stopwatch = new Stopwatch();

                // Store the enemy details for easy, efficient access.
                Enemy enemyCP = enemy.GetComponent<Enemy>();
                
                PrintEnemyDetails(enemy, isAttacking);

                int scrollIndex = 0;

                while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
                {
                    if (enemy.GetComponent<Health>().health == 0 || world.GetPlayer().GetComponent<Health>().health == 0)
                        continue;

                    if (isAttacking)
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
                                Renderer.PrintAt(Constants.TextInputX + i, Constants.TextInputY, '#', Color.FromArgb(51 * enemyCP.playerDefendPattern[i], 255, 255 - 51 * enemyCP.playerDefendPattern[i]));
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

                            if (isAttacking)
                            {
                                if(enemy.GetComponent<Health>().health - world.GetPlayer().GetComponent<Damager>().damage * enemyCP.playerAttackPattern[scrollIndex] > 0)
                                {
                                    enemy.GetComponent<Health>().health -= world.GetPlayer().GetComponent<Damager>().damage * enemyCP.playerAttackPattern[scrollIndex];

                                    isAttacking = false;
                                    scrollIndex = 0;
                                    PrintEnemyDetails(enemy, isAttacking);
                                }
                                else
                                {
                                    enemy.GetComponent<Health>().health = 0;

                                    // Add the enemy's items to the player.
                                    if(enemy.HasComponent<Inventory>())
                                    {
                                        //world.GetPlayer().GetComponent<Inventory>().items = 
                                        //    (new Inventory(world.GetPlayer().GetComponent<Inventory>().items) + enemy.GetComponent<Inventory>()).items;

                                        InventoryMenu.Start("Player", "Ground", ref world.GetPlayer().GetComponent<Inventory>().items, ref enemy.GetComponent<Inventory>().items);

                                        Renderer.RenderPlayerInfo(world.GetPlayer());

                                        // Create a loot drop on the ground if the player does not collect all the items.
                                        if(enemy.GetComponent<Inventory>().items.Count > 0)
                                        {
                                            Entity loot = new Entity(EntityFlags.Loot);
                                            loot.AddComponent(new Position(enemy.GetComponent<Position>().x, enemy.GetComponent<Position>().y))
                                                .AddComponent(new Symbol('o'))
                                                .AddComponent(new Colour(Color.Gray))
                                                .AddComponent(new Inventory(enemy.GetComponent<Inventory>().items));

                                            Renderer.RenderEntity(loot);

                                            world.GetCurrentZone().Entities.Add(loot);
                                        }
                                    }

                                    if(enemy.HasComponent<Weapon>())
                                    {
                                        WeaponMenu.Start(ref world, ref enemy);
                                    }

                                    PrintOutcome(enemy, true);
                                    enemy.Removed = true;
                                }

                                // Clear text input
                                Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
                                Console.WriteLine(new string(' ', 100));
                            }
                            else
                            {
                                if (world.GetPlayer().GetComponent<Health>().health - enemy.GetComponent<Damager>().damage * enemyCP.playerDefendPattern[scrollIndex] > 0)
                                {
                                    world.GetPlayer().GetComponent<Health>().health -= enemy.GetComponent<Damager>().damage * enemyCP.playerDefendPattern[scrollIndex];
                                    // Show the player's health.
                                    Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 1, $"Health: {world.GetPlayer().GetComponent<Health>().health} / {world.GetPlayer().GetComponent<Health>().maxHealth}", Color.LightGray);
                                    isAttacking = true;
                                    scrollIndex = 0;
                                }
                                else
                                {
                                    world.GetPlayer().GetComponent<Health>().health = 0;
                                    // Show the player's health.
                                    Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 1, $"Health: {world.GetPlayer().GetComponent<Health>().health} / {world.GetPlayer().GetComponent<Health>().maxHealth}", Color.LightGray);
                                    PrintOutcome(enemy, false);
                                    Game.Stop(false);
                                }

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
                            Console.CursorVisible = true;
                            return true;
                        }
                    }
                    stopwatch.Reset();
                }

                Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.White);
                Renderer.PrintGameOutput(
                    "Enter a command... ".PadRight(100) + "\n" +
                    new string(' ', 100) + "\n" +
                    new string(' ', 100) + "\n" +
                    new string(' ', 100) + "\n");
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

        private void PrintOutcome(Entity enemy, bool victory)
        {
            string name;
            if (enemy.HasComponent<Character>())
            {
                name = enemy.GetComponent<Character>().name;
            }
            else
            {
                name = "an enemy";
            }

            if (victory)
            {
                Renderer.PrintGameOutput($"You defeated {name}!".PadRight(100) + "\n"
                    + "Hold ESC to continue.".PadRight(100) + "\n"
                    + new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
            }
            else
            {
                Renderer.PrintGameOutput($"You died while defending against {name}!");
                
            }
        }
    }
}
