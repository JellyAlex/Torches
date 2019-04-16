using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches
{
    public class Tile
    {
        public char symbol { get; set; }
        public bool isSolid { get; set; }
        

        public Tile(char symbol = 'E', bool isSolid = false)
        {
            this.symbol = symbol;
            this.isSolid = isSolid;
        }
    }
}
