using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    [Flags]
    public enum EntityFlags : short
    {
        None = 0,
        Tribesman = 1
    }
}
