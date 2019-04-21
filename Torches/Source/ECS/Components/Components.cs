using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Diagnostics;

namespace Torches.ECS
{
    public class Position : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public int x;
        public int y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Position TranslateX(int dx)
        {
            x += dx;
            return this;
        }

        public Position TranslateY(int dy)
        {
            y += dy;
            return this;
        }

        public Position Translate(int dx, int dy)
        {
            x += dx;
            y += dy;
            return this;
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
        public int[] playerDefendPattern;

        // Delay of attack/defence scroller in milliseconds
        public int delay;

        public Enemy(int[] playerAttackPattern, int[] playerDefendPattern, int delay)
        {
            this.playerAttackPattern = playerAttackPattern;
            this.playerDefendPattern = playerDefendPattern;
            this.delay = delay;
        }
    }

    public class Damager : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public int damage;
        public int baseDamage;

        public Damager(int damage)
        {
            this.damage = damage;
            baseDamage = damage;
        }
    }

    public class Inventory : IComponent
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public Dictionary<string, int> items;

        public Inventory(Dictionary<string, int> items)
        {
            this.items = items;
        }

        public Inventory()
            : this(new Dictionary<string, int>()) { }

        // Allow two inventories to be added together.
        public static Inventory operator +(Inventory a, Inventory b)
        {
            Inventory result = new Inventory(a.items);

            foreach(KeyValuePair<string, int> itemstack in b.items)
            {
                if(result.items.ContainsKey(itemstack.Key))
                {
                    result.items[itemstack.Key] += b.items[itemstack.Key];
                }
                else
                {
                    result.items.Add(itemstack.Key, b.items[itemstack.Key]);
                }
            }

            return result;
        }
    }

    public class Weapon : IComponent // TODO: Implement weapons.
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public string name;
        public int damage;

        public Weapon(string name = "none", int damage = 1)
        {
            this.name = name;
            this.damage = damage;
        }

        public Weapon(Weapon weapon)
        {
            name = weapon.name;
            damage = weapon.damage;
        }
    }

    public class Coins : IComponent // TODO: Implement currency.
    {
        #region IComponent implementation
        public Entity entity { set; get; }
        #endregion

        public int coins;

        public Coins(int coins)
        {
            this.coins = coins;
        }

        public Coins()
            : this(0) { }
    }
}
