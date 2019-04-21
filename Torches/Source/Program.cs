using System;

namespace Torches
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            while(!Game.Success)
            {
                Game.Start();
            }
        }
    }
}
