using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Torches.ECS
{
    class LootSystem : ISystem
    {
        public void Update(ref World world) { }
        public bool UpdateCommand(string[] segments, ref World world)
        {
            if (segments[0] == "loot" || segments[0] == "l")
            {
                Entity e = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>());
                if (e != null)
                {
                    if (e.HasFlag(EntityFlags.Loot))
                    {
                        Loot(ref world, ref e);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("Cannot drop here.");
                    }

                    return true;
                }
            }
            else if (segments[0] == "drop" || segments[0] == "d")
            {
                if(world.GetPlayer().GetComponent<Inventory>().items.Count > 0)
                {
                    Entity e = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>());
                    if (e == null)
                    {
                        e = new Entity(EntityFlags.Loot);
                        e.AddComponent(new Position(world.GetPlayer().GetComponent<Position>()))
                            .AddComponent(new Symbol('o'))
                            .AddComponent(new Colour(System.Drawing.Color.Gray))
                            .AddComponent(new Inventory());
                        world.GetCurrentZone().Entities.Add(e);

                        Loot(ref world, ref e);

                        return true;
                    }
                    else if (e.HasFlag(EntityFlags.Loot) && e.HasComponent<Inventory>())
                    {
                        Loot(ref world, ref e);
                    }
                    else
                    {
                        Renderer.PrintGameOutput("Can't drop items here...");
                    }
                }
                else
                {
                    Renderer.PrintGameOutput("You have no items to drop.");
                }

                return true;
            }


            return false;
        }

        private void Loot(ref World world, ref Entity e)
        {
            if (e.HasComponent<Coins>())
            {
                world.GetPlayer().GetComponent<Coins>().coins += e.GetComponent<Coins>().coins;
                e.GetComponent<Coins>().coins = 0;
            }

            if (e.HasComponent<Inventory>())
            {
                Trace.WriteLine("Inventory menu");
                InventoryMenu.Start("Player", "Ground", ref world.GetPlayer().GetComponent<Inventory>().items, ref e.GetComponent<Inventory>().items);
            }

            if (e.HasComponent<Weapon>())
            {
                WeaponMenu.Start(ref world, ref e);
            }

            if (e.HasComponent<Inventory>() && e.HasComponent<Weapon>())
            {
                if (e.GetComponent<Inventory>().items.Count == 0 &&
                    (e.GetComponent<Weapon>().name == "" || e.GetComponent<Weapon>().name == "none" || e.GetComponent<Weapon>().damage == 1))
                {
                    e.Removed = true;
                }
            }
            else if (e.HasComponent<Inventory>())
            {
                if (e.GetComponent<Inventory>().items.Count == 0)
                {
                    e.Removed = true;
                }
            }
            else if (e.HasComponent<Weapon>())
            {
                if (e.GetComponent<Weapon>().name == "" || e.GetComponent<Weapon>().name == "none" || e.GetComponent<Weapon>().damage == 1)
                {
                    e.Removed = true;
                }
            }
            else
            {
                e.Removed = true;
            }

            Renderer.RenderPlayerInfo(world.GetPlayer());
        }
    }
}
