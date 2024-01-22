using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace VeloLibrary
{
    internal class LibraryManager
    {
        readonly string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.json");
        List<Book> books = new List<Book>();

        public void CreateLibrary()
        {
            if (File.Exists(jsonFilePath))
            {
                books.Clear();
                books = ReadJsonToList(jsonFilePath);
                //Console.WriteLine("Existing Library Loaded."); //for debuging
            }
            else
            {
                InitJsonFile();
                //Console.WriteLine("New Library Created."); //for debuging
            }
            Operations();
        }

        #region JSON Methods
        void InitJsonFile()
        {
            Book dummyBook0 = new Book
            {
                BookNo = 0,
                Title = "Dummy_Book",
                Author = "Dummy_Author",
                ISBN = "000-0000000000",
                StockAmount = 0,
                LentAmount = 0
            };

            Book dummyBook01 = new Book
            {
                BookNo = 1,
                Title = "Dummy_Book1",
                Author = "Dummy_Author1",
                ISBN = "000-0000000001",
                StockAmount = 0,
                LentAmount = 0
            };

            books.Clear();
            // Add 10 of dummy_book
            for (int i = 0; i < 10; i++)
            {
                AddBookToLibrary(dummyBook0);
            }
            AddBookToLibrary(dummyBook01);
        }

        void RefreshBooksJson()
        {
            foreach (Book book in books)
            {
                //bookid leri indexlere eşleyerek id ile kitap çağrmayı kolaylaştırabiliriz.
                book.BookNo = books.IndexOf(book);
            }
            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(books, Formatting.Indented));
        }

        List<Book> ReadJsonToList(string filePath)
        {
            return JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(filePath));
        }
        #endregion

        void Operations()
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
                    ShowBookList();
                    PressToContinue();
                    Operations();
                    break;

                case "a":
                    AddBook();
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
                    string line = Console.ReadLine();
                    if (line == "y" || line == "yes")
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



        void PressToContinue()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue...              ");
            Console.ResetColor();
            Console.ReadKey();
        }

        void ShowBookList()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n  Book List:                              ");
            Console.ResetColor();
            foreach (var book in books)
            {
                Console.WriteLine($"  {book.BookNo} - {book.Title}  |  Author: {book.Author}  |  ISBN: {book.ISBN}  |  StockAmount: {book.StockAmount}  |  LentAmount: {book.LentAmount}");
            }
        }


        private void AddBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter 'Book No' for add an existing book. ");
            Console.WriteLine("Enter    (a)    for add a  new book.      ");
            Console.ResetColor();
            string addLine = Console.ReadLine();

            if (addLine == "a")
            {
                CreateAndAddBookToLibrary();
            }
            else
            {
                try
                {
                    int id = Convert.ToInt32(addLine);
                    if (AddExistingBook(id))
                    {
                        PressToContinue();
                    }
                    else
                    {
                        AddError();
                    }
                }
                catch
                {
                    AddError();
                }
            }
            Operations();
        }

        void CreateAndAddBookToLibrary()
        {
            Book book = new Book();
            string readedLine = "";

            Console.WriteLine("Book Title: ");
            readedLine = Console.ReadLine();
            if (readedLine == "") CreateError();
            else book.Title = readedLine;

            Console.WriteLine("Book Author: ");
            readedLine = Console.ReadLine();
            if (readedLine == "") CreateError();
            else book.Author = readedLine;

            Console.WriteLine("Book ISBN: ");
            readedLine = Console.ReadLine();
            if (readedLine == "") CreateError();
            else book.ISBN = readedLine;

            Console.WriteLine("How many copy do you add? ");
            try
            {
                int stockSize = Convert.ToInt32(Console.ReadLine());
                AddBookToLibrary(book, stockSize);
                PressToContinue();
            }
            catch
            {
                CreateError();
            }
        }

        void CreateError()
        {
            Console.WriteLine("It is not a valid entry! \nWant to try again. (y)es?");
            string line = Console.ReadLine();
            if (line == "y" || line == "yes")
            {
                Console.WriteLine("\n");
                CreateAndAddBookToLibrary();
            }
            else
            {
                Operations();
            }
        }

        void AddError()
        {
            Console.WriteLine("Invalid entry! \nWant to try again? (y)es?");
            string line = Console.ReadLine();
            if (line == "y" || line == "yes")
            {
                Console.WriteLine("\n");
                ShowBookList();
                AddBook();
            }
            else
            {
                Operations();
            }
        }

        bool AddExistingBook(int id)
        {
            if (id >= 0 && id < books.Count)
            {
                AddBookToLibrary(books[id]);
                return true;
            }
            else
            {
                return false;
            }
        }

        void AddBookToLibrary(Book book, int? stockSize = null)
        {
            if (books.Contains(book))
            {
                book.StockAmount++;
            }
            else
            {
                book.StockAmount = stockSize ?? 1;
                books.Add(book);
            }
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n" + (stockSize ?? 1) + " books titled '" + book.Title + "' has been added.");
            Console.ResetColor();
            RefreshBooksJson();
        }

        void RemoveBookFromLibrary(Book book)
        {
            if (books.Contains(book))
            {
                book.StockAmount--;
                if (book.StockAmount < 0) book.StockAmount = 0;
            }
            RefreshBooksJson();
        }

        void BorrowABookWithId(int id)
        {
            if (books[id].StockAmount > 0)
            {
                books[id].LentAmount++;
                Console.WriteLine(books[id].Title + " - book was lent");
                RemoveBookFromLibrary(books[id]);
            }
            else
            {
                Console.WriteLine(books[id].Title + " - book is out of library stocks");
            }
        }

    }
}
