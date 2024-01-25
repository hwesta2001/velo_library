using System;

namespace VeloLibrary
{
    internal class Program
    {
        static LibraryManager libraryManager;
        static InitRandomLibrary InitRandom = new InitRandomLibrary();
        static void Main(string[] args)
        {
            InitRandom.MakeBooks();
            Console.ReadKey();
            WelcomeSheet();
            libraryManager = new LibraryManager();
            libraryManager.CreateLibrary();
        }

        private static void WelcomeSheet()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("******************************************");
            Console.WriteLine("*******  Welcome to Velo Library   *******");
            Console.WriteLine("*******    Made by Hasan YILMAZ    *******");
            Console.WriteLine("******************************************");
            Console.ResetColor();
        }


    }
}
