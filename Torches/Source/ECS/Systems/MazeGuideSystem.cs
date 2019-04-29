using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Torches.ECS
{
    public class MazeGuideSystem : ISystem
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
                        return HandleMazeGuide(ref world, 1, 0);
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        return HandleMazeGuide(ref world, -1, 0);
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        return HandleMazeGuide(ref world, 0, 1);
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        return HandleMazeGuide(ref world, 0, -1);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
                else
                {
                    // Check for the tribesman in all four directions.
                    if (!HandleMazeGuide(ref world, 1, 0))
                        if (!HandleMazeGuide(ref world, -1, 0))
                            if (!HandleMazeGuide(ref world, 0, 1))
                                return HandleMazeGuide(ref world, 0, -1);
                            else return true;
                        else return true;
                    else return true;
                }
            }

            return false;
        }

        private bool HandleMazeGuide(ref World world, int dx, int dy)
        {
            Entity guide = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
            if (guide == null)
            {
                return false;
            }
            else if (guide.HasFlag(EntityFlags.MazeGuide))
            {
                // If the maze door is open the maze guide says nothing.
                if (world.GetCurrentZone().Doors[3])
                {
                    Renderer.PrintGameOutputDelayed("Maze Guide: ...".PadRight(100), 30);
                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    // Clear input/output fields
                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.Gray);
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
                }
                else
                {
                    // Print maze guide dialogue.
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Maze Guide: Warning traveller... behind this door is a dark, dark place...".PadRight(100), 30);

                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
                    Renderer.PrintGameOutputDelayed("Maze Guide: I could tell you how to get through the maze, ".PadRight(100) + "\n" +
                        "but then I'd have to kill you...".PadRight(100), 30);

                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, "(press any key)", Color.Gray);
                    Console.ReadKey();

                    // Open the maze doors.
                    world.GetCurrentZone().Doors[3] = true;
                    world.GetZone(-1, -3).Doors[0] = true;
                    world.GetCurrentZone().RenderDoors();

                    // Clear input/output fields.
                    Renderer.PrintAt(Constants.TextInputX, Constants.TextInputY, new string(' ', 100), Color.Gray);
                    Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
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
