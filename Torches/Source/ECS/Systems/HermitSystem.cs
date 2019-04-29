using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Drawing;

namespace Torches.ECS
{
    class HermitSystem : ISystem
    {
        public void Update(ref World world) { }
        public bool UpdateCommand(string[] segments, ref World world)
        {
            if (segments[0] == "interact" || segments[0] == "i")
            {
                if (segments.Length >= 2)
                {
                    string direction = segments[1].ToLower().Trim();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        return HandleHermit(ref world, 1, 0);
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        return HandleHermit(ref world, -1, 0);
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        return HandleHermit(ref world, 0, 1);
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        return HandleHermit(ref world, 0, -1);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
                else
                {
                    // Check for the tribesman in all four directions.
                    if (!HandleHermit(ref world, 1, 0))
                        if (!HandleHermit(ref world, -1, 0))
                            if (!HandleHermit(ref world, 0, 1))
                                return HandleHermit(ref world, 0, -1);
                            else return true;
                        else return true;
                    else return true;
                }
            }

            return false;
        }

        public bool HandleHermit(ref World world, int dx, int dy)
        {
            Entity hermit = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
            if (hermit == null)
            {
                return false;
            }
            else if (hermit.HasFlag(EntityFlags.Hermit))
            {
                if(Quests.HermitQuest == QuestStatus.Available)
                {
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Hermit: Help... HELP... I was just chased down the eastern".PadRight(100) + " \n" +
                        "mountain by a hord of orcs!".PadRight(100), 30);

                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Hermit: Please battle them to get them away from this peaceful land...".PadRight(100) + " \n" +
                        "Also, I think I saw a treasure chest at the peak...".PadRight(100), 30);

                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    Quests.HermitQuest = QuestStatus.Started;
                }
                else if(Quests.HermitQuest == QuestStatus.Started)
                {
                    Renderer.PrintGameOutputDelayed("Hermit: Have you killed the orcs yet? ...", 40);
                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();
                }
                else
                {
                    Renderer.PrintGameOutput("Hermit: Praise the lord those orcs didn't kill you...");
                }

                // Clear fields.
                Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.Gray);
                Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
