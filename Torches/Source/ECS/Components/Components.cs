using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Diagnostics;

namespace Torches.ECS
{
    public class ItemStack
    {
        int typeID;
        List<Item> items;

        public ItemStack()
            :this(-1) {}

        public ItemStack(int type)
        {
            this.typeID = type;
            items = new List<Item>();
        }

        public ItemStack(int type, int count)
        {
            this.typeID = type;
            items = new List<Item>();

            for(int i = 0; i < count; i++)
            {
                items.Add(new Item(type));
            }
        }

        public ItemStack(List<Item> items)
            :this(items, items.Count > 0 ? items[0].type : -1)
        {
            
        }

        public ItemStack(List<Item> items, int type)
        {
            this.items = items;
            this.typeID = type;

            for (int i = items.Count - 1; i >= 0; i--)
            {
                if (this.items[i].type != this.typeID)
                {
                    Trace.WriteLine("Had to remove item " + this.items[i].ToString() + " from ItemStack.");
                    this.items.RemoveAt(i);
                }
            }
        }

        // Function withdraws a certain number of items from the stack.
        public ItemStack Withdraw(int numItems)
        {
            if(items.Count >= numItems)
            {
                List<Item> toReturn = items.GetRange(0, numItems);
                items = items.GetRange(numItems, items.Count - numItems);
                return new ItemStack(toReturn, typeID);
            }
            else
            {
                return Withdraw(items.Count);
            }
        }
    }

    public class Item
    {
        public int type;
        public string displayName;

        public Item()
            : this(-1, "error") { }

        public Item(int type)
            :this(type, type.ToString())
        {
            
        }

        public Item(int type, string displayName)
        {
            this.type = type;
            this.displayName = displayName;
        }

        public override string ToString()
        {
            return displayName;
        }
    }

    #region Components
    public class ZonePosition : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public int x;
        public int y;

        public ZonePosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class Symbol : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public char symbol;

        public Symbol(char symbol)
        {
            this.symbol = symbol;
        }
    }

    public class Health : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public int health;
        public int maxHealth;

        public Health(int health, int maxHealth)
        {
            this.health = health;
            this.maxHealth = maxHealth;
        }
    }

    public class Character : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public string name;

        public Character(string name)
        {
            this.name = name;
        }
    }

    public class Inventory : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public List<ItemStack> itemStacks;

        public Inventory(List<ItemStack> itemStacks)
        {
            this.itemStacks = itemStacks;
        }

        public Inventory()
            :this(new List<ItemStack>()) { }
    }
    
    public class Colour : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public Color color;

        public Colour(Color color)
        {
            this.color = color;
        }

        public Colour()
            :this(Color.White) { }
    }

    public class Enemy : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        // This is an array of the different damage coefficients that the player can do to the enemy.
        public int[] playerAttackPattern;
        // This is an array of the different damages that the enemy can inflict to the player.
        public int[] playerDefencePattern;

        // Delay of attack/defence scroller in milliseconds
        public int delay;

        public Enemy(int[] playerAttackPattern, int[] playerDefencePattern, int delay)
        {
            this.playerAttackPattern = playerAttackPattern;
            this.playerDefencePattern = playerDefencePattern;
            this.delay = delay;
        }
    }
    
    #endregion
}
