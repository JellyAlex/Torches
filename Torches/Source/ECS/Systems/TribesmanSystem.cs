using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using System.Drawing;

namespace Torches.ECS
{
    public class TribesmanSystem : ISystem
    {
        public void Update(ref World world)
        {
            // Open zone(1, 0)'s bottom door if the tribesman quest is completed.
            Zone z = world.GetZone(1, 0);
            if (z != null)
            {
                if (Quests.TribesmanQuest != QuestStatus.Completed && z.Doors[3] == true)
                {
                    z.Doors[3] = false;

                    if (world.GetCurrentZone().IsPos(1, 0))
                    {
                        world.GetCurrentZone().RenderDoors();
                    }
                }
            }
        }

        public bool UpdateCommand(string[] segments, ref World world)
        {
            if(segments[0] == "interact" || segments[0] == "i")
            {
                if(segments.Length >= 2)
                {
                    string direction = segments[1].ToLower().Trim();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        return HandleTribesman(ref world, 1, 0);
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        return HandleTribesman(ref world, -1, 0);
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        return HandleTribesman(ref world, 0, 1);
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        return HandleTribesman(ref world, 0, -1);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
                else
                {
                    // Check for the tribesman in all four directions.
                    if (!HandleTribesman(ref world, 1, 0))
                        if (!HandleTribesman(ref world, -1, 0))
                            if (!HandleTribesman(ref world, 0, 1))
                                return HandleTribesman(ref world, 0, -1);
                            else return true;
                        else return true;
                    else return true;
                }
            }

            return false;
        }

        private bool HandleTribesman(ref World world, int dx, int dy)
        {
            Entity tribesman = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
            if (tribesman == null)
            {
                return false;
            }
            else if (tribesman.HasFlag(EntityFlags.Tribesman))
            {
                if(Quests.TribesmanQuest == QuestStatus.Available)
                {
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Tribesman: Greetings traveller...".PadRight(100), 30);

                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Tribesman: It seems I have dropped my precious wooden staff".PadRight(100) + "\n" +
                        "somewhere on the ground... Could you find it for me?".PadRight(100), 30);

                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Tribesman: While moving, press SPACE to dig...".PadRight(100) + "\n" +
                        "If you find it, I will open this door and give you a reward.".PadRight(100), 30);
                    Console.ReadKey();

                    // Clear input & output fields
                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.Gray);
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                        + new string(' ', 100) + "\n"
                        + new string(' ', 100) + "\n");

                    Quests.TribesmanQuest = QuestStatus.Started;
                }
                else if(Quests.TribesmanQuest == QuestStatus.Started)
                {
                    if(world.GetPlayer().GetComponent<Inventory>().items.ContainsKey("wooden staff"))
                    {
                        if(world.GetPlayer().GetComponent<Inventory>().items["wooden staff"] > 0)
                        {
                            // Set the quest to completed.
                            Quests.TribesmanQuest = QuestStatus.Completed;

                            // Remove one wooden staff from the player's inventory.
                            world.GetPlayer().GetComponent<Inventory>().items["wooden staff"]--;
                            if(world.GetPlayer().GetComponent<Inventory>().items["wooden staff"] <= 0)
                            {
                                world.GetPlayer().GetComponent<Inventory>().items.Remove("wooden staff");
                            }

                            // Give player 2 gold
                            if(world.GetPlayer().GetComponent<Inventory>().items.ContainsKey("gold"))
                            {
                                world.GetPlayer().GetComponent<Inventory>().items["gold"] += 2;
                            }
                            else
                            {
                                world.GetPlayer().GetComponent<Inventory>().items["gold"] = 2;
                            }

                            // Show the player's inventory changes.
                            Renderer.RenderPlayerInfo(world.GetPlayer());

                            Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                            Renderer.PrintGameOutputDelayed("Tribesman: Thank you very much, have some gold!".PadRight(100), 30);

                            Thread.Sleep(1000);
                            Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                            Console.ReadKey();
                            // Open the bottom door.
                            world.GetCurrentZone().Doors[3] = true;
                            world.GetCurrentZone().RenderDoors();

                            Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                            Renderer.PrintGameOutputDelayed("Tribesman: Oh, Let me open this door for you.".PadRight(100), 30);

                            Thread.Sleep(1000);
                            Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                            Console.ReadKey();
                            // Clear input & output fields
                            Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.Gray);
                            Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                        }
                    }
                    else
                    {
                        Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                        Renderer.PrintGameOutputDelayed("Tribesman: Have you found my staff yet? ...".PadRight(100), 30);
                    }
                }
                else
                {
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n"
                                + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Tribesman: ...".PadRight(100), 50);
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
