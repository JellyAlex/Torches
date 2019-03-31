using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Console = Colorful.Console;

namespace Torches
{
    static class Renderer
    {
        #region UI Strings
        private const string titleLarge = @"
        (  .         . .     )   .         )               )
      )        _____               _                 (       )
          .  '/__   \___  _ __ ___| |__   ___  ___    .  '  .
   (    )       / /\/ _ \| '__/ __| '_ \ / _ \/ __|  (.   ',(
    .' ( . )   / / | |_| | | | |__| | | |  __/\__ \  ,  ()   ( .
 ). , (   (  ) \/   \___/|_|  \___|_| |_|\___||___/ ( , '  ,    )
(_,) .  ) _) _ ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ ')  (,  ,. (' )";


        private const string uiBase = @"
__________________________________________________________________
       Map                         Character
--------..--------    --------------------------------------
|                |    |                    | Items:        |
|                |    | Health:            | 0             |
|                |    | Weapon:            | 0             |
:                :    | Helmet:            | 0             |
|                |    | Chestplate:        | 0             |
|                |    | Leggings:          | 0             |
|                |    | Boots:             | 0             |
|                |    |                    | 0             |
--------..--------    --------------------------------------
__________________________________________________________________
> 
__________________________________________________________________
";
        #endregion

        public static void PrintUI()
        {
            Console.Clear();

            foreach (char c in titleLarge)
            {
                if (c == '(' || c == ')' || c == '.' || c == '\'' || c == ',')
                {
                    Console.Write(c, Color.Orange);
                }
                else if (c == '|' || c == '\\' || c == '/' || c == '_')
                {
                    Console.Write(c, Color.OrangeRed);
                }
                else if (c == '^')
                {
                    Console.Write(c, Color.Gray);
                }
                else
                {
                    Console.Write(c, Color.White);
                }
            }

            foreach (char c in uiBase)
            {
                if (c == '_')
                {
                    Console.Write(c, Color.Brown);
                }
                else if (c == '>')
                {
                    Console.Write(c, Color.DarkKhaki);
                }
                else if (c == '|' || c == '-')
                {
                    Console.Write(c, Color.DarkOrange);
                }
                else
                {
                    Console.Write(c, Color.LightGray);
                }
            }
        }

        public static void PrintAt(int x, int y, string text, Color color)
        {
            int tempX = Console.CursorLeft;
            int tempY = Console.CursorTop;

            Console.SetCursorPosition(x, y);
            Console.Write(text, color);
            Console.SetCursorPosition(tempX, tempY);
        }

        public static void PrintAt(int x, int y, char c, Color color)
        {
            int tempX = Console.CursorLeft;
            int tempY = Console.CursorTop;

            Console.SetCursorPosition(x, y);
            Console.Write(c, color);
            Console.SetCursorPosition(tempX, tempY);
        }

        public static void PrintGameOutput(string text)
        {
            PrintAt(Constants.TextOutputX, Constants.TextOutputY, text.PadRight(100, ' '), Color.DarkKhaki);
        }
    }
}
