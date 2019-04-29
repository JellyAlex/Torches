using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

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
--------..--------    ---------------------------------------
|                |    |                    | Items:         |
|                |    | Health:  /         | 0              |
|                |    | Weapon:            | 0              |
:                :    | Helmet:            | 0              |
|                |    | Chestplate:        | 0              |
|                |    | Leggings:          | 0              |
|                |    | Boots:             | 0              |
|                |    | Coins: 0           | 0              |
--------..--------    ---------------------------------------
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
                    Console.Write(c, Color.Red);
                }
                else if (c == '|' || c == '\\' || c == '/' || c == '_')
                {
                    Console.Write(c, Color.DarkOrange);
                }
                else if (c == '^')
                {
                    Console.Write(c, Color.Gray);
                }
                else
                {
                    Console.Write(c, Color.White);
                }
                
                //Thread.Sleep(10);
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
            Color color = Color.DarkKhaki;

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

        public static void PrintGameOutputDelayed(string text, int delay)
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(Constants.TextOutputX, Constants.TextOutputY);

            for (int i = 0; i < text.Length; i++)
            {
                Console.Write(text[i], Color.DarkKhaki);
                if(text[i] != ' ')
                {
                    Thread.Sleep(delay);
                }
                
            }

            Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
            Console.CursorVisible = true;
        }

        public static void RenderEntity(Entity e)
        {
            if (e.HasComponent<Position>() && e.HasComponent<Symbol>())
            {
                if (e.HasComponent<Colour>())
                {
                    PrintAt(Constants.MapX + e.GetComponent<Position>().x, Constants.MapY + Zone.Height - e.GetComponent<Position>().y - 1, e.GetComponent<Symbol>().symbol, e.GetComponent<Colour>().color);
                }
                else
                {
                    PrintAt(Constants.MapX + e.GetComponent<Position>().x, Constants.MapY + Zone.Height - e.GetComponent<Position>().y - 1, e.GetComponent<Symbol>().symbol, Color.White);
                }
            }
            else
            {
                Trace.WriteLine("Entity (id " + e.id + ") doesn't have required components to render.");
            }
        }

        public static void RenderPlayerInfo(Entity player)
        {
            int i = 0;
            foreach(KeyValuePair<string, int> itemstack in player.GetComponent<Inventory>().items)
            {
                // Only print seven items.
                if (i >= 7)
                    break;

                PrintAt(Constants.PlayerItemsX, Constants.PlayerItemsY + 1 + i, $"{itemstack.Value}x {itemstack.Key}".PadRight(15), Color.LightGray);
                i++;
            }

            // Print the player's health.
            PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 1, $"Health: {player.GetComponent<Health>().health} / {player.GetComponent<Health>().maxHealth}".PadRight(19), Color.LightGray);

            // Print the player's coins.
            PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 7,
                    $"Coins: {player.GetComponent<Coins>().coins}".PadRight(19), Color.LightGray);

            // Print the player's weapon.
            if (player.HasComponent<Weapon>())
            {
                PrintAt(Constants.PlayerStatsX, Constants.PlayerStatsY + 2, 
                    $"Weapon: {player.GetComponent<Weapon>().name} ({player.GetComponent<Weapon>().damage})".PadRight(19), Color.LightGray);
            }
        }
    }
}
