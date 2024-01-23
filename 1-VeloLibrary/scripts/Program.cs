using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VeloLibrary
{
    internal class Program
    {
        static LibraryManager libraryManager;
        static void Main(string[] args)
        {
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
