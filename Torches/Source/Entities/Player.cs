using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Torches.Entities
{
    public class Player : Entity
    {
        public string name { get; set; }
        public int health { get; set; }
        public int maxHealth { get; set; }
        
        public Player(int x, int y, string name, int health, int maxHealth)
        {
            this.x = x;
            this.y = y;
            this.name = name;
            this.health = health;
            this.maxHealth = maxHealth;
            symbol = '@';
            isSolid = false;
        }

        override public void Render()
        {
            Renderer.PrintAt(Constants.MapX + x,  Constants.MapY + Zone.Height - y - 1, symbol, Color.OrangeRed);
        }
    }
}
