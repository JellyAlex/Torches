using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Console = Colorful.Console;
using Torches.ECS;
using System.Diagnostics;

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
            Console.CursorVisible = false;

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
            Console.CursorVisible = true;
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

        public static void PrintGameOutputColoured(string text)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(Constants.TextOutputX, Constants.TextOutputY);

            int prevTagIndex = -3;
            Color color = Color.White;

            for (int i = 0; i < text.Length; i++)
            {
                if(prevTagIndex == i - 1)
                {
                    switch (text[i])
                    {
                        case 'W':
                            color = Color.White;
                            break;
                        case 'w':
                            color = Color.Gray;
                            break;
                        case 'R':
                            color = Color.Red;
                            break;
                        case 'r':
                            color = Color.DarkRed;
                            break;
                        case 'G':
                            color = Color.Green;
                            break;
                        case 'g':
                            color = Color.LightGreen;
                            break;
                        case 'B':
                            color = Color.Blue;
                            break;
                        case 'b':
                            color = Color.LightBlue;
                            break;
                        case 'Y':
                            color = Color.Yellow;
                            break;
                        case 'y':
                            color = Color.LightYellow;
                            break;
                        case 'P':
                            color = Color.Pink;
                            break;
                        case 'p':
                            color = Color.Purple;
                            break;
                    }
                }
                else if(text[i] == '`')
                {
                    prevTagIndex = i;
                }
                else
                {
                    Console.Write(text[i], color);
                }
            }

            Console.CursorVisible = true;
        }

        public static void RenderEntity(Entity e)
        {
            if (e.HasComponent<ZonePosition>() && e.HasComponent<Symbol>())
            {
                if (e.HasComponent<Colour>())
                {
                    PrintAt(Constants.MapX + e.GetComponent<ZonePosition>().x, Constants.MapY + Zone.Height - e.GetComponent<ZonePosition>().y - 1, e.GetComponent<Symbol>().symbol, e.GetComponent<Colour>().color);
                }
                else
                {
                    PrintAt(Constants.MapX + e.GetComponent<ZonePosition>().x, Constants.MapY + Zone.Height - e.GetComponent<ZonePosition>().y - 1, e.GetComponent<Symbol>().symbol, Color.White);
                }
            }
            else
            {
                Trace.WriteLine("Entity (id " + e.id + ") doesn't have required components to render.");
            }
        }
    }
}
