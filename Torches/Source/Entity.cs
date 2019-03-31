using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Torches.Entities;

namespace Torches
{
    public class Entity : Tile
    {
        public int x { get; set; }
        public int y { get; set; }
        //protected string type;

        public Entity(char symbol = 'E', bool isSolid = false, int x = 0, int y = 0)
            :base(symbol, isSolid)
        {
            this.x = x;
            this.y = y;
        }

        virtual public void Render()
        {
            Renderer.PrintAt(Constants.MapX, Constants.MapY + Zone.Height - y - 1, symbol, Color.White);
        }

        virtual public void Interact(Player player) { }
    }
}
