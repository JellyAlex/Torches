using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Torches.ECS
{
    [Flags]
    public enum EntityFlags : short
    {
        None            = 0,
        Solid           = 1,
        Player          = 2,
        Tribesman       = 4,
        Enemy           = 8,
        Loot            = 16,
        DigSpot         = 32,
        MazeGuide       = 64,
        Hermit          = 128,
        HermitCompleter = 256,
        Merchant        = 512
    }

    // An entity is a collection of components (information). It is treated differently depending on the data structures it contains.
    public class Entity
    {
        public static uint latestEntityID = 0;
        public readonly uint id;
        
        private readonly List<IComponent> components;

        public EntityFlags flags;

        public bool Removed { get; set; } = false;

        public Entity()
            :this(new EntityFlags()) { }

        public Entity(EntityFlags flags)
        {
            id = latestEntityID++;
            components = new List<IComponent>();
            this.flags = flags;
        }

        public Entity AddComponent(IComponent newComponent)
        {
            // Make sure a blank component is not entered
            if (newComponent == null)
            {
                Trace.WriteLine("ERROR: Component entered on entity " + id.ToString() + " was null.");
            }
            else
            {
                components.Add(newComponent);
            }

            // Return this entity so multiple commands can be chained.
            return this;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            //Check if a component of the specified type exists
            foreach (IComponent cp in components)
                if(cp is T)
                    return true;

            return false;
        }

        public T GetComponent<T>() where T : class, IComponent
        {
            // Search for and return the component that was requested.
            foreach (IComponent cp in components)
                if (cp is T)
                    return (T)cp;

            Trace.WriteLine("ERROR: Entity " + id.ToString() + " does not contain component " + typeof(T).ToString());
            return null;
        }

        public Entity AddFlag(EntityFlags flag)
        {
            flags |= flag;
            return this;
        }

        public bool HasFlag(EntityFlags flag)
        {
            return (flags & flag) == flag;
        }

        public Entity RemoveFlag(EntityFlags flag)
        {
            flags &= ~flag;
            return this;
        }
    }
}
