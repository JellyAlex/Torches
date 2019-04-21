using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

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
                    // Get direction from the second part of the command
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
                    Console.CursorVisible = false;
                    ConsoleKeyInfo key;
                    do
                    {
                        Renderer.PrintGameOutput("Use WASD or Arrow keys to move, SPACE to dig, ESC to finish.");
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
                        else if (key.Key == ConsoleKey.UpArrow || key.Key == ConsoleKey.W)
                        {
                            TryMovePlayer(ref world, 0, 1);
                        }
                        else if (key.Key == ConsoleKey.DownArrow || key.Key == ConsoleKey.S)
                        {
                            TryMovePlayer(ref world, 0, -1);
                        }
                        else if(key.Key == ConsoleKey.Spacebar)
                        {
                            if(Dig(ref world, world.GetPlayer().GetComponent<Position>())
                                || Dig(ref world, new Position(world.GetPlayer().GetComponent<Position>().x + 1, world.GetPlayer().GetComponent<Position>().y))
                                || Dig(ref world, new Position(world.GetPlayer().GetComponent<Position>().x - 1, world.GetPlayer().GetComponent<Position>().y)))
                            {
                                Renderer.PrintGameOutputColoured("\n`w... I think I found something ... ".PadRight(100));
                                Console.CursorVisible = false;
                            }
                            else
                            {
                                Renderer.PrintGameOutputColoured("\n`w... Nothing here ... ".PadRight(100));
                                Console.CursorVisible = false;
                            }

                            Renderer.RenderEntity(world.GetPlayer());
                        }

                    } while (key.Key != ConsoleKey.Escape);
                    Renderer.PrintGameOutput("Enter a command... (type 'help' for additional information)");
                    Console.CursorVisible = true;
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
            // Check if player is trying to leave the zone.
            int targetX = world.GetPlayer().GetComponent<Position>().x + dx;
            int targetY = world.GetPlayer().GetComponent<Position>().y + dy;


            // Right door.
            if (targetX == Zone.Width && targetY == Zone.Height / 2)
            {
                if (world.GetCurrentZone().Doors[0])
                {
                    world.ChangeZone(world.GetCurrentZone().X + 1, world.GetCurrentZone().Y, 0, targetY);
                }
            }
            // Top door.
            else if (targetY == Zone.Height && targetX == Zone.Width / 2 - 1 || targetY == Zone.Height && targetX == Zone.Width / 2)
            {
                if (world.GetCurrentZone().Doors[1])
                {
                    world.ChangeZone(world.GetCurrentZone().X, world.GetCurrentZone().Y + 1, targetX, 0);
                }
            }
            // Left door.
            else if (targetX == -1 && targetY == Zone.Height / 2)
            {
                if (world.GetCurrentZone().Doors[2])
                {
                    world.ChangeZone(world.GetCurrentZone().X - 1, world.GetCurrentZone().Y, Zone.Width - 1, targetY);
                }
            }
            // Bottom door.
            else if (targetY == -1 && targetX == Zone.Width / 2 - 1 || targetY == -1 && targetX == Zone.Width / 2)
            {
                if (world.GetCurrentZone().Doors[3])
                {
                    world.ChangeZone(world.GetCurrentZone().X, world.GetCurrentZone().Y - 1, targetX, Zone.Height - 1);
                }
            }

            // Make sure player can't go through solid objects.
            else if (!world.GetCurrentZone().IsSolidAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy))
            {
                // Change the player's position.
                world.GetPlayer().GetComponent<Position>().x += dx;
                world.GetPlayer().GetComponent<Position>().y += dy;


                // Render the tile that the player was standing on.
                Entity e = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x - dx, world.GetPlayer().GetComponent<Position>().y - dy);
                if (e != null)
                {
                    if(e.HasComponent<Symbol>() && e.HasComponent<Position>())
                    {
                        Renderer.RenderEntity(e);
                    }
                    else
                    {
                        world.GetCurrentZone().RenderTile(world.GetPlayer().GetComponent<Position>().x - dx,
                            world.GetPlayer().GetComponent<Position>().y - dy);
                    }
                }
                else
                {
                    world.GetCurrentZone().RenderTile(world.GetPlayer().GetComponent<Position>().x - dx,
                       world.GetPlayer().GetComponent<Position>().y - dy);
                }
                

                // Render the player.
                Renderer.RenderEntity(world.GetPlayer());
            }
        }

        private bool Dig(ref World world, Position position)
        {
            if(position.x < 0 || position.y < 0 || position.x >= Zone.Width || position.y >= Zone.Height)
            {
                return false;
            }

            Entity e = world.GetCurrentZone().GetEntityAt(position);

            if (world.GetCurrentZone().GetTileAt(position).Symbol == '.')
            {
                world.GetCurrentZone().GetTileAt(position).Symbol = ',';

                // Only render tile if there isn't an entity on it.
                if(e == null)
                {
                    world.GetCurrentZone().RenderTile(position);
                }
            }

            if (e != null)
            {
                if (e.HasFlag(EntityFlags.DigSpot))
                {
                    e.RemoveFlag(EntityFlags.DigSpot)
                        .AddFlag(EntityFlags.Loot)
                        .AddComponent(new Colour(System.Drawing.Color.SaddleBrown));

                    if (e.HasComponent<Symbol>())
                    {
                        e.GetComponent<Symbol>().symbol = 'o';
                    }
                    else
                    {
                        e.AddComponent(new Symbol('o'));
                    }

                    Renderer.RenderEntity(e);

                    return true;
                }
            }

            return false;
        }
    }
}
