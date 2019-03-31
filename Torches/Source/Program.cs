using System;

namespace Torches
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            Game.Start();

            Console.WriteLine("Finished. Press any key to close...");
            Console.ReadKey();
        }
    }
}
