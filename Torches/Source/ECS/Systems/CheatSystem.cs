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

        public bool Update(string[] segments, ref World world)
        {
            // Put cheat commands here.
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
