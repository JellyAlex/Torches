using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Torches
{
    public class Entity : Tile
    {
        protected int x { get; set; }
        protected int y { get; set; }
        //protected string type;

        virtual public void Render()
        {
            Renderer.PrintAt(Constants.MapX, Constants.MapY, symbol, Color.White);
        }
    }
}
