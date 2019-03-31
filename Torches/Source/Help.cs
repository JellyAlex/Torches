using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Diagnostics;

namespace Torches
{
    static class Help
    {
        public static void OpenHelpMenu(string[] segments)
        {
            Renderer.PrintGameOutput("You have entered the help menu.                                         \n" +
                                     "Use the arrow keys to navigate and press ESC to exit.");

            int line_index = 0;
            string[] lines = File.ReadAllLines("Resources/Help/help.txt", Encoding.UTF8);
            
            ConsoleKeyInfo key;
            do
            {
                string output = "";
                for (int i = 0; i < (lines.Length >= 4 ? 4 : lines.Length); i++)
                {
                    output += lines[i] + "                                    \n";
                }
                Renderer.PrintGameOutput(output);

                key = Console.ReadKey();
                Trace.WriteLine("Read key: " + key.Key.ToString());

                if (key.Key == ConsoleKey.UpArrow)
                {
                    if(line_index > 0)
                    {
                        line_index -= 1;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (line_index < lines.Length - 4)
                    {
                        line_index += 1;
                    }
                }

            } while (key.Key != ConsoleKey.Escape);
            
            
        }
    }
}
