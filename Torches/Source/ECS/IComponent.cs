using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches.ECS
{
    // A component is a data structure which an entity can subscribe to.
    public interface IComponent
    {
        Entity entity { set; get; }
    }
}
