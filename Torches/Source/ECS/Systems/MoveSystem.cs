using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Torches.ECS;

namespace Torches.ECS
{
    public class MoveSystem : ISystem
    {
        public bool Update(string[] segments, ref World world)
        {
            if(segments.First() == "move" || segments.First() == "m")
            {
                // The command contains a direction if it has two or more segments
                if (segments.Length >= 2)
                {
                    string direction = segments[1].ToLower();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        TryMovePlayer(ref world, 1, 0);
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {
                        TryMovePlayer(ref world, -1, 0);
                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {
                        TryMovePlayer(ref world, 0, 1);
                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {
                        TryMovePlayer(ref world, 0, -1);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
                else // move the player using WASD
                {
                    ConsoleKeyInfo key;
                    do
                    {
                        Renderer.PrintGameOutput("Use WASD or Arrow keys to move, ESC to exit.");
                        // Get arrow key input
                        key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.D)
                        {
                            TryMovePlayer(ref world, 1, 0);
                        }
                        else if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.A)
                        {
                            TryMovePlayer(ref world, -1, 0);
                        }
                        if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                        {
                            TryMovePlayer(ref world, 0, 1);
                        }
                        else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                        {
                            TryMovePlayer(ref world, 0, -1);
                        }

                    } while (key.Key != ConsoleKey.Escape);
                    Renderer.PrintGameOutput("Enter a command... (type 'help' or 'h' for additional information)");
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        // This function moves the player if the position is available (ie. not solid and in map)
        private void TryMovePlayer(ref World world, int dx, int dy)
        {
            if (!world.GetCurrentZone().IsSolidAt(world.GetPlayer().GetComponent<ZonePosition>().x + dx, world.GetPlayer().GetComponent<ZonePosition>().y + dy))
            {
                world.GetPlayer().GetComponent<ZonePosition>().x += dx;
                world.GetPlayer().GetComponent<ZonePosition>().y += dy;
                world.GetCurrentZone().Render();
                Renderer.RenderEntity(world.GetPlayer());
            }
        }
    }
}
