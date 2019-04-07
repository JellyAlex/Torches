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
        const int NumLines = 6;

        public static void OpenHelpMenu(string[] segments)
        {
            Renderer.PrintGameOutput("Use the arrow keys to navigate and press ESC to exit.");

            int line_index = 0;
            string[] lines = File.ReadAllLines("Resources/Help/help.txt", Encoding.UTF8);
            
            ConsoleKeyInfo key;
            do
            {
                // Collect the lines that should appear on the screen.
                string output = "";
                for (int i = line_index; i < (lines.Length - line_index >= NumLines ? line_index + NumLines : lines.Length); i++)
                {
                    output += lines[i].PadRight(60) +"\n";
                }
                
                // Send the current lines of text to be coloured and outputted.
                Renderer.PrintGameOutputColoured("\n" + output);

                // Get arrow key input
                key = Console.ReadKey(true);
                
                if (key.Key == ConsoleKey.UpArrow)
                {
                    if (line_index > 0)
                    {
                        line_index -= 1;
                    }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    if (line_index < lines.Length - NumLines)
                    {
                        line_index += 1;
                    }
                }

            } while (key.Key != ConsoleKey.Escape);

            for(int i = 0; i <= NumLines; i++)
            {
                Renderer.PrintGameOutput(new string('\n', i));
            }
            
        }
    }
}
