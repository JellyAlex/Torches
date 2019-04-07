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

        private static List<ECS.ISystem> systems;
        
        public static void Start()
        {
            running = true;

            Renderer.PrintUI();

            world = new World();

            systems = new List<ECS.ISystem>();
            systems.Add(new ECS.MoveSystem());
            
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
                // If the update returns false, the command has not been handled
                else if(!Update(segments))
                {
                    // Make sure the command isn't too long to print.
                    if (command.Length <= 30)
                    {
                        // Give the user feedback on the dud command.
                        Renderer.PrintGameOutput("(Error) Invalid command: " + command + ". Use command 'help'");
                    }
                    else
                    {
                        Renderer.PrintGameOutput("(Error) Invalid command. Use command 'help'");
                    }
                }
            }
        }
        
        private static bool Update(string[] segments)
        {
            foreach(ECS.ISystem s in systems)
            {
                if (s.Update(segments, world))
                    return true;
            }

            return false;
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
