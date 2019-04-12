using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    // A system is a class which adds functionality to the entities. (ie. completes all logic)
    public interface ISystem
    {
        bool Update(string[] segments, ref World world);
    }
}
