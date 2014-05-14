using System;

namespace PirateWars
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            /*using (TestGame game = new TestGame())
                game.Run();*/
            using (Game1 game = new Game1())
            {
                game.Run();
            }
        }
    }
#endif
}

