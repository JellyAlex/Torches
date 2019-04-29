using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Torches.ECS;

namespace Torches
{
    class WeaponMenu
    {
        public static void Start(ref World world, ref Entity e)
        {
            if (e.GetComponent<Weapon>().name != "" && e.GetComponent<Weapon>().name != "none" && e.GetComponent<Weapon>().damage != 1)
            {
                Renderer.PrintGameOutput($"Swap current weapon {world.GetPlayer().GetComponent<Weapon>().name} (dmg={world.GetPlayer().GetComponent<Weapon>().damage})" +
                    $" for weapon {e.GetComponent<Weapon>().name} (dmg={e.GetComponent<Weapon>().damage})? (yes/no)");

                if (Game.InputCommand() == "yes")
                {
                    string tempName = world.GetPlayer().GetComponent<Weapon>().name;
                    int tempDamage = world.GetPlayer().GetComponent<Weapon>().damage;

                    world.GetPlayer().GetComponent<Weapon>().name = e.GetComponent<Weapon>().name;
                    world.GetPlayer().GetComponent<Weapon>().damage = e.GetComponent<Weapon>().damage;

                    e.GetComponent<Weapon>().name = tempName;
                    e.GetComponent<Weapon>().damage = tempDamage;
                }

                // Clear output field.
                Renderer.PrintGameOutput(new string(' ', 100) + "\n"
                    + new string(' ', 100) + "\n");
            }
        }
    }
}
