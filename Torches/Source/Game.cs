using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Torches
{
    class Game
    {
        bool running = true;

        World world;
        
        public Game()
        {
            Renderer.PrintUI();

            world = new World();
            
            while (running)
            {
                Renderer.PrintGameOutput("Enter a command...");
                string command = InputCommand();

                Update(command);

                running = false;
            }
        }

        private void Update(string command)
        {
            world.Update(command);

            //Renderer.PrintAt(Constants.TextOutputX, Constants.TextOutputY, command);
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
    }
}
