using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches
{
    static class Game
    {
        private static bool running = false;

        private static World world;
        
        public static void Start()
        {
            running = true;

            Renderer.PrintUI();

            world = new World();
            
            while (running)
            {
                string command = InputCommand();
                string[] segments = command.Split(' ');

                if(segments.First() == "quit" || segments.First() == "exit")
                {
                    running = false;
                }
                else if(segments.First() == "help" || segments.First() == "h")
                {
                    Help.OpenHelpMenu(segments);
                }
                else
                {
                    Update(command);
                }

            }
        }

        private static void Update(string command)
        {
            world.Update(command);
        }

        public static string InputCommand()
        {
            Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
            string command = Console.ReadLine();
            Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
            Console.WriteLine(new string(' ', 60));
            Console.SetCursorPosition(Constants.TextInputX, Constants.TextInputY);
            return command;
        }

        public static void Stop()
        {
            running = false;
        }
    }
}
