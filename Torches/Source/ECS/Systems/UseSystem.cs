using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    class UseSystem : ISystem
    {
        private const int BandageHealAmount = 30;

        public void Update(ref World world) { }
        public bool UpdateCommand(string[] segments, ref World world)
        {
            if((segments[0] == "use" || segments[0] == "u") && segments.Length == 2)
            {
                if(segments[1] == "bandage")
                {
                    if(world.GetPlayer().GetComponent<Inventory>().items.ContainsKey("bandage"))
                    {
                        if(world.GetPlayer().GetComponent<Inventory>().items["bandage"] > 0)
                        {
                            world.GetPlayer().GetComponent<Inventory>().items["bandage"]--;
                            if(world.GetPlayer().GetComponent<Health>().health + BandageHealAmount > world.GetPlayer().GetComponent<Health>().maxHealth)
                            {
                                world.GetPlayer().GetComponent<Health>().health = world.GetPlayer().GetComponent<Health>().maxHealth;
                            }
                            else
                            {
                                world.GetPlayer().GetComponent<Health>().health += BandageHealAmount;
                            }
                            Renderer.RenderPlayerInfo(world.GetPlayer());
                        }
                        else
                        {
                            Renderer.PrintGameOutput("You do not have this item.");
                        }
                    }
                    else
                    {
                        Renderer.PrintGameOutput("You do not have this item.");
                    }
                }

                return true;
            }


            return false;
        }
    }
}
