using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    public class WeaponSystem : ISystem
    {
        public void Update(ref World world)
        {
            // Loop through entities and set their damage values to the damage of their weapon.
            foreach (Entity e in world.GetCurrentZone().Entities)
            {
                if (e.HasComponent<Damager>())
                {
                    if (e.HasComponent<Weapon>())
                    {
                        e.GetComponent<Damager>().damage = e.GetComponent<Weapon>().damage;
                    }
                    else
                    {
                        e.GetComponent<Damager>().damage = e.GetComponent<Damager>().baseDamage;
                    }
                }
            }

            // Update the player's damage.
            if (world.GetPlayer().HasComponent<Damager>())
            {
                if (world.GetPlayer().HasComponent<Weapon>())
                {
                    world.GetPlayer().GetComponent<Damager>().damage =
                        world.GetPlayer().GetComponent<Damager>().baseDamage * world.GetPlayer().GetComponent<Weapon>().damage;
                }
                else
                {
                    world.GetPlayer().GetComponent<Damager>().damage = world.GetPlayer().GetComponent<Damager>().baseDamage;
                }
            }

        }
        public bool UpdateCommand(string[] segments, ref World world)
        {
            return false;
        }
    }
}
