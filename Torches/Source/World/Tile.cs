using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches
{
    public class Tile
    {
        public char Symbol { get; set; }
        public bool IsSolid { get; set; }
        

        public Tile(char symbol = 'E', bool isSolid = false)
        {
            Symbol = symbol;
            IsSolid = isSolid;
        }
    }
}
