using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    class LootSystem : ISystem
    {
        public bool Update(string[] segments, ref World world)
        {
            if(segments[0] == "loot" || segments[0] == "l")
            {
                Entity e = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<Position>());
                if (e != null)
                {
                    if(e.HasFlag(EntityFlags.Loot) )
                    {
                        if (e.HasComponent<Coins>())
                        {
                            world.GetPlayer().GetComponent<Coins>().coins += e.GetComponent<Coins>().coins;
                            e.GetComponent<Coins>().coins = 0;
                        }

                        if (e.HasComponent<Inventory>())
                        {
                            InventoryMenu.Start("Player", "Ground", ref world.GetPlayer().GetComponent<Inventory>().items, ref e.GetComponent<Inventory>().items);
                        }

                        if (e.HasComponent<Weapon>())
                        {
                            WeaponMenu.Start(ref world, ref e);
                        }

                        if(e.HasComponent<Inventory>() && e.HasComponent<Weapon>())
                        {
                            if(e.GetComponent<Inventory>().items.Count == 0 && 
                                (e.GetComponent<Weapon>().name == "" || e.GetComponent<Weapon>().name == "none" || e.GetComponent<Weapon>().damage == 1))
                            {
                                e.Removed = true;
                            }
                        }
                        else if(e.HasComponent<Inventory>())
                        {
                            if (e.GetComponent<Inventory>().items.Count == 0)
                            {
                                e.Removed = true;
                            }
                        }
                        else if(e.HasComponent<Weapon>())
                        {
                            if(e.GetComponent<Weapon>().name == "" || e.GetComponent<Weapon>().name == "none" || e.GetComponent<Weapon>().damage == 1)
                            {
                                e.Removed = true;
                            }
                        }
                        else
                        {
                            e.Removed = true;
                        }

                        Renderer.RenderPlayerInfo(world.GetPlayer());

                        return true;
                    }
                }
            }
            else if (segments[0] == "drop" || segments[0] == "d")
            {
                // TODO: player able to drop items.

            }


            return false;
        }

    }
}
