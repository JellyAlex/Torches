using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    // A system is a class which completes functionality/logic for certain entities.
    public interface ISystem
    {
        void Update(ref World world);
        bool UpdateCommand(string[] segments, ref World world);
    }
}
