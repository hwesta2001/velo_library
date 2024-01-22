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
            WelcomeSheet();
            Operations();
        }

        private static void WelcomeSheet()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("*****************************************");
            Console.WriteLine("*******  Welcome to Velo Library  *******");
            Console.WriteLine("*****************************************");
            Console.ResetColor();
        }

        private static void Operations()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\nChoose an option from the following list;");
            Console.ResetColor();

            Console.WriteLine(" s - Show books list."); //done
            Console.WriteLine(" a - Add a book to library"); //done
            Console.WriteLine(" r - Remove a book from library");
            Console.WriteLine(" d - Delete book from library list");
            Console.WriteLine(" f - Find a book in library");
            Console.WriteLine(" b - Borrow a book from library");
            Console.WriteLine(" t - Return a book to library");
            Console.WriteLine(" c - Clear");
            Console.WriteLine(" x - Exit");

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("What do you want to do?                   ");
            Console.ResetColor();

            switch (Console.ReadLine())
            {

                case "s":
                    libraryManager.ShowBookList();
                    PressToContinue();
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

                case "c":
                    Console.Clear();
                    Operations();
                    break;

                case "x":
                    Environment.Exit(0);
                    break;

                default:
                    Console.WriteLine("Invalid entry                        ");
                    Console.WriteLine("Would you like to continue? (y)es?   ");
                    if (Console.ReadLine() == "y" || Console.ReadLine() == "yes")
                    {
                        Operations();
                    }
                    else
                    {
                        Console.WriteLine("Closing Library. See You Again!   ");
                        Environment.Exit(0);
                    }
                    break;
            }

        }

        private static void TryAddExistingBook()
        {
            Console.WriteLine("Write Book Number you want to add ");

            try
            {
                int id = Convert.ToInt32(Console.ReadLine());
                if (libraryManager.AddExistingBook(id))
                {
                    Console.WriteLine("\n,'" + libraryManager.GetBookWithId(id).Title + "' added.");
                    PressToContinue();
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
            catch
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

        private static void PressToContinue()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Press any key to continue...              ");
            Console.ResetColor();
            Console.ReadKey();

        }
    }
}
