using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using Torches.ECS;

namespace Torches.ECS
{
    // An entity is a collection of components (information). It is treated differently depending on the data structures it contains.
    public class Entity
    {
        public static uint latestEntityID = 0;
        public readonly uint id;
        public readonly List<IComponent> components;
        public EntityFlags flags;

        public Entity()
        {
            id = latestEntityID++;
            components = new List<IComponent>();
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
                // Add new component to component list.
                components.Add(newComponent);
                newComponent.entity = this;
            }

            // Return this entity so multiple commands can be chained.
            return this;
        }

        public bool HasComponent<T>() where T : class, IComponent
        {
            // Check if a component of the specified type exists
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

        public bool HasFlag(EntityFlags flag)
        {
            return (flags & flag) == flag;
        }
    }
}
