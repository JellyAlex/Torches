using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Torches.ECS
{
    class CheatSystem : ISystem
    {
        private bool isGodMode = false;
        public void Update(ref World world) { }
        public bool UpdateCommand(string[] segments, ref World world)
        {
            // Cheat command to make the player almost unkillable
            if(segments[0] == "godmode")
            {
                if(segments.Length == 2)
                {
                    if (segments[1].ToLower() == "on")
                    {
                        isGodMode = true;
                        world.GetPlayer().GetComponent<Health>().maxHealth = 9999;
                        Renderer.PrintGameOutput("Godmode on.");
                    }
                    else if(segments[1].ToLower() == "off")
                    {
                        isGodMode = false;
                        world.GetPlayer().GetComponent<Health>().maxHealth = 200;
                        Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 1, $"Health: {world.GetPlayer().GetComponent<Health>().health} / {world.GetPlayer().GetComponent<Health>().maxHealth}", Color.LightGray);

                        Renderer.PrintGameOutput("Godmode off.");
                    }
                }

                if (isGodMode)
                {
                    world.GetPlayer().GetComponent<Health>().health = 9999;
                    Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 1, $"Health: {world.GetPlayer().GetComponent<Health>().health} / {world.GetPlayer().GetComponent<Health>().maxHealth}", Color.LightGray);

                }

                return true;
            }
            if(isGodMode)
            {
                world.GetPlayer().GetComponent<Health>().health = 9999;
                Renderer.PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 1, $"Health: {world.GetPlayer().GetComponent<Health>().health} / {world.GetPlayer().GetComponent<Health>().maxHealth}", Color.LightGray);
            }

            // Cheat command to remove a tile/entity from the world.
            if(segments[0] == "remove" && segments.Length == 3)
            {
                if(int.TryParse(segments[1], out int dx) && int.TryParse(segments[2], out int dy))
                {
                    Remove(ref world, dx, dy);
                    return true;
                }
                else
                {
                    return false;
                }
                
            }

            // Cheat command to teleport to different areas.
            if(segments[0] == "tp" && segments.Length == 3)
            {
                if(int.TryParse(segments[1], out int x) && int.TryParse(segments[2], out int y))
                {
                    world.GetPlayer().GetComponent<Position>().x = x;
                    world.GetPlayer().GetComponent<Position>().y = y;
                }
                else
                {
                    Renderer.PrintGameOutput("Bad position");
                }
            }

            // Cheat command to teleport between zones.
            if (segments[0] == "tpz" && segments.Length == 5)
            {
                if (int.TryParse(segments[1], out int x) && int.TryParse(segments[2], out int y)
                    && int.TryParse(segments[3], out int zx) && int.TryParse(segments[4], out int zy))
                {
                    world.ChangeZone(zx, zy, x, y);
                }
                else
                {
                    Renderer.PrintGameOutput("Bad position");
                }

                return true;
            }

            if(segments[0] == "give" && segments.Length > 1)
            {
                if(world.GetPlayer().GetComponent<Inventory>().items.ContainsKey(segments[1]))
                {
                    if(segments.Length == 3)
                    {
                        if(int.TryParse(segments[2], out int amount))
                        {
                            world.GetPlayer().GetComponent<Inventory>().items[segments[1]] += amount;
                        }
                        else
                        {
                            world.GetPlayer().GetComponent<Inventory>().items[segments[1]]++;
                        }
                    }
                    else
                    {
                        world.GetPlayer().GetComponent<Inventory>().items[segments[1]]++;
                    }
                }
                else
                {
                    if (segments.Length == 3)
                    {
                        if (int.TryParse(segments[2], out int amount))
                        {
                            world.GetPlayer().GetComponent<Inventory>().items[segments[1]] = amount;
                        }
                        else
                        {
                            world.GetPlayer().GetComponent<Inventory>().items[segments[1]] = 1;
                        }
                    }
                    else
                    {
                        world.GetPlayer().GetComponent<Inventory>().items[segments[1]] = 1;
                    }
                }

                Renderer.RenderPlayerInfo(world.GetPlayer());

                return true;
            }

            if(segments[0] == "coins" && segments.Length == 2)
            {
                if (int.TryParse(segments[1], out int coins))
                {
                    world.GetPlayer().GetComponent<Coins>().coins = coins;
                    Renderer.RenderPlayerInfo(world.GetPlayer());
                }
                else
                {
                    Renderer.PrintGameOutput("Error: Unknown coin value");
                }

                return true;
            }

            // Cheat command to print the player's position
            if(segments[0] == "pos")
            {
                Position p = world.GetPlayer().GetComponent<Position>();
                Renderer.PrintGameOutput($"Player Position: {p.x}, {p.y} in zone {world.GetCurrentZone().X}, {world.GetCurrentZone().Y}".PadRight(100));

                return true;
            }

            return false;
        }

        private void Remove(ref World world, int dx, int dy)
        {
            Entity e = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
            if (e != null)
            {
                e.Removed = true;
            }

            if(world.GetCurrentZone().Tiles[world.GetPlayer().GetComponent<Position>().y + dy, world.GetPlayer().GetComponent<Position>().x + dx].IsSolid)
            {
                world.GetCurrentZone().Tiles[world.GetPlayer().GetComponent<Position>().y + dy, world.GetPlayer().GetComponent<Position>().x + dx].IsSolid = false;
                world.GetCurrentZone().Tiles[world.GetPlayer().GetComponent<Position>().y + dy, world.GetPlayer().GetComponent<Position>().x + dx].Symbol = ' ';
                world.GetCurrentZone().RenderTile(world.GetPlayer().GetComponent<Position>().x + dx, world.GetPlayer().GetComponent<Position>().y + dy);
            }
        }
    }
}
