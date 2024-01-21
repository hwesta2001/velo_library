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
            libraryManager = new LibraryManager();
            libraryManager.CreateLibrary();
            Console.WriteLine("\nWelcome to Velo Library");

            Operations();
        }

        private static void Operations()
        {
            Console.WriteLine("\nChoose an option from the following list:");
            Console.WriteLine("\ts - Show books list."); //done
            Console.WriteLine("\ta - Add a book to library"); //done
            Console.WriteLine("\tr - Remove a book from library");
            Console.WriteLine("\td - Delete book from library list");
            Console.WriteLine("\tf - Find a book in library");
            Console.WriteLine("\tb - Borrow a book from library");
            Console.WriteLine("\tt - Return a book to library");
            Console.WriteLine("\tc - Clear");
            Console.WriteLine("\tx - Exit");

            Console.WriteLine("What do you want to do? ");

            switch (Console.ReadLine())
            {
                case "x":
                    Environment.Exit(0);
                    break;
                case "c":
                    Console.Clear();
                    Operations();
                    break;
                case "s":
                    libraryManager.ShowBookList();
                    Operations();
                    break;
                case "a":
                    libraryManager.ShowBookList();
                    Console.WriteLine("Add an existing book? (y)es or (n)no?");
                    switch (Console.ReadLine())
                    {
                        case "y":
                        case "yes":
                            TryAddExistingBook();
                            break;
                        case "n":
                        case "no":
                            libraryManager.CreateAndAddBookToLibrary();
                            break;
                        default:
                            Console.WriteLine("Invalid entry");
                            break;
                    }
                    Operations();
                    break;
                default:
                    Console.WriteLine("Would you like to continue the operation? (y)es?");
                    if (Console.ReadLine() == "y" || Console.ReadLine() == "yes")
                    {
                        Operations();
                    }
                    else
                    {
                        Environment.Exit(0);
                    }
                    break;
            }

        }

        private static void TryAddExistingBook()
        {
            Console.WriteLine("Write BookId you want to add ");
            int id = Convert.ToInt32(Console.ReadLine());
            if (libraryManager.AddExistingBook(id))
            {
                Console.WriteLine("\nOne more '" + libraryManager.GetBookWithId(id).Title + "' added.");
                Console.WriteLine("Press a key to continue...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Invalid book id. Do you want to try again? (y)es?");
                if (Console.ReadLine() == "y" || Console.ReadLine() == "yes")
                {
                    libraryManager.ShowBookList();
                    TryAddExistingBook();
                }
                else
                {
                    Operations();
                }
            }
        }
    }
}
