using System;

namespace VeloLibrary
{
    internal class Program
    {
        public static LibraryManager libraryManager;
        static void Main(string[] args)
        {
            libraryManager = new LibraryManager();
            libraryManager.CreateLibrary();
        }
    }
}
