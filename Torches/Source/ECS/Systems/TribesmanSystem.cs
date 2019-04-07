using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    public class TribesmanSystem : ISystem
    {
        public bool Update(string[] segments, World world)
        {
            if(segments[0] == "interact" || segments[0] == "i")
            {
                if(segments.Length >= 2)
                {
                    string direction = segments[1].ToLower();

                    if (direction == "right" || direction == "r" || direction == "east" || direction == "e")
                    {
                        Entity tribesman = world.GetCurrentZone().GetEntityAt(world.GetPlayer().GetComponent<ZonePosition>().x + 1, world.GetPlayer().GetComponent<ZonePosition>().y);
                        if(tribesman == null)
                        {
                            return false;
                        }
                        else if(tribesman.HasFlag(EntityFlags.Tribesman))
                        {
                            Renderer.PrintGameOutput("You have met the tribesman.");

                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else if (direction == "left" || direction == "l" || direction == "west" || direction == "w")
                    {

                    }
                    else if (direction == "up" || direction == "u" || direction == "north" || direction == "n")
                    {

                    }
                    else if (direction == "down" || direction == "d" || direction == "south" || direction == "s")
                    {

                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Unknown direction: " + direction);
                    }
                }
            }

            return false;
        }
    }
}
