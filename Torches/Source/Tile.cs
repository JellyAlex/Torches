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

        public Tile()
            :this('E', false) { }

        public Tile(char symbol, bool isSolid)
        {
            this.symbol = symbol;
            this.isSolid = isSolid;
        }
    }
}
