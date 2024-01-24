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
        readonly string books_jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.json");
        readonly string lents_jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lents.json");
        List<LentData> lents = new List<LentData>();
        List<Book> books = new List<Book>();
        Book GetBook(string title)
        {
            for (int i = 0; i < books.Count; i++)
            {
                if (title == books[i].Title)
                {
                    return books[i];
                }
            }
            return null;
        }
        Book[] GetBooks(string author)
        {
            Book[] _books = null;
            for (int i = 0; i < books.Count; i++)
            {
                if (author == books[i].Author)
                {
                    _books.Append(books[i]).ToArray();
                }
            }
            return _books;
        }

        void LentBookAdd(Book book, DateTime dt)
        {
            LentData ld = new LentData(book, dt);
            lents.Add(ld);
            RefreshLentsJson();
        }

        public void CreateLibrary()
        {
            if (File.Exists(books_jsonFilePath))
            {
                books.Clear();
                books = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(books_jsonFilePath));
            }
            else
            {
                InitBookJsonFile();
            }

            if (File.Exists(lents_jsonFilePath))
            {
                lents.Clear();
                lents = JsonConvert.DeserializeObject<List<LentData>>(File.ReadAllText(lents_jsonFilePath));
            }
            else
            {
                InitLentJsonFile();
            }
            Operations();
        }

        #region JSON Methods
        void InitLentJsonFile()
        {
            lents.Clear();
            foreach (var book in books)
            {
                if (book.LentAmount > 0)
                {
                    for (int i = 0; i < book.LentAmount; i++)
                    {
                        LentBookAdd(book, DateTime.Today);
                    }
                }
            }
            RefreshLentsJson();
        }

        void InitBookJsonFile()
        {
            Book dummyBook0 = new Book
            {
                BookNo = 0,
                Title = "Dummy_Book",
                Author = "Dummy_Author",
                ISBN = "000-0000000000",
                StockAmount = 0,
                LentAmount = 0,
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
                //her kitap eklenip cıkarıldığında list idlere göre kitaplar sıralanır
                //bookno lari indexlere eşleyerek no ile kitap çağrmayı kolaylaştırabiliriz.
                book.BookNo = books.IndexOf(book);
                // totalAmount her seferinde lent+stock size olacaktır.
                book.TotalAmount = book.LentAmount + book.StockAmount;
            }
            // her kitap ekleme çıkarma işlemi sonrası json dosyamızı güncelliyoruz.
            File.WriteAllText(books_jsonFilePath, JsonConvert.SerializeObject(books, Formatting.Indented));
        }
        void RefreshLentsJson()
        {
            File.WriteAllText(lents_jsonFilePath, JsonConvert.SerializeObject(lents, Formatting.Indented));
        }

        #endregion

        void Operations()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n*******        OPERATIONS           ******");
            Console.ResetColor();

            Console.WriteLine(" s - Show books list."); //done
            Console.WriteLine(" l - Show lent books list.");
            Console.WriteLine(" a - Add a book to library"); //done
            Console.WriteLine(" r - Remove a book from library"); //done
            Console.WriteLine(" d - Delete book from library list"); //done
            Console.WriteLine(" f - Find a book in library"); //done
            Console.WriteLine(" b - Borrow a book from library"); //done
            Console.WriteLine(" t - Return a book to library"); //done
            Console.WriteLine(" c - Clear"); //done
            Console.WriteLine(" x - Exit"); //done

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

                case "l":
                    ShowLentBookList();
                    PressToContinue();
                    Operations();
                    break;

                case "a":
                    AddBook();
                    PressToContinue();
                    Operations();
                    break;

                case "r":
                    RemoveBook();
                    PressToContinue();
                    Operations();
                    break;

                case "d":
                    DeleteBook();
                    PressToContinue();
                    Operations();
                    break;

                case "f":
                    FindBook();
                    PressToContinue();
                    Operations();
                    break;

                case "b":
                    BarrowBook();
                    PressToContinue();
                    Operations();
                    break;

                case "t":
                    ReturnBook();
                    PressToContinue();
                    Operations();
                    break;

                // ****************************
                // console operations
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
                    if (line == "y" || line == "yes" || line == "Yes" || line == "Y" || line == "YES")
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


        void ErrorCall(Action f, string message = null)
        {
            Console.WriteLine(message ?? "Invalid entry! " + " Want to try again? (y)es?");
            string line = Console.ReadLine();
            if (line == "y" || line == "yes" || line == "Yes" || line == "Y" || line == "YES")
            {
                Console.WriteLine("\n");
                ShowBookList();
                f();
            }
            else
            {
                Operations();
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

        void ShowLentBookList()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            if (lents.Count > 0)
            {
                Console.WriteLine("\n  Lent Books List:                        ");
                foreach (var lent in lents)
                {
                    Book book = lent.book;
                    Console.WriteLine($"  {book.BookNo} - {book.Title}  |  Author: {book.Author}  |  ISBN: {book.ISBN}  |  Borrow Date: {lent.BorrowTime}  |  Return Date: {lent.ReturnTime}");
                }
            }
            else
            {
                Console.WriteLine("\n  There are no lent books now             ");
            }
            Console.ResetColor();
        }

        void AddBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Gray;
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
                    if (!AddExistingBook(id))
                    {
                        ErrorCall(AddBook);
                    }
                }
                catch
                {
                    ErrorCall(AddBook);
                }
            }
        }

        bool InvalidStringEntry(string line)
        {
            switch (line)
            {
                // simdilik on adet space kadar invalidString sayılıcak
                // sonradan diğer istenmeyen stringler de eklenebilir.
                case "":
                case " ":
                case "  ":
                case "   ":
                case "    ":
                case "     ":
                case "      ":
                case "       ":
                case "        ":
                case "         ":
                case "          ":
                    return true;
                default:
                    return false;
            }
        }

        void CreateAndAddBookToLibrary()
        {
            Book book = new Book();
            string readedLine = "";

            Console.WriteLine("Book Title: ");
            readedLine = Console.ReadLine();
            if (InvalidStringEntry(readedLine)) ErrorCall(CreateAndAddBookToLibrary, "Invalid book title,");
            else book.Title = readedLine;

            Console.WriteLine("Book Author: ");
            readedLine = Console.ReadLine();
            if (InvalidStringEntry(readedLine)) ErrorCall(CreateAndAddBookToLibrary, "Invalid author name,");
            else book.Author = readedLine;

            Console.WriteLine("Book ISBN: ");
            readedLine = Console.ReadLine();
            if (InvalidStringEntry(readedLine)) ErrorCall(CreateAndAddBookToLibrary, "Invalid ISBN,");
            else book.ISBN = readedLine;

            Console.WriteLine("How many copy do you add? ");
            try
            {
                int stockSize = Convert.ToInt32(Console.ReadLine());
                AddBookToLibrary(book, stockSize);
            }
            catch
            {
                ErrorCall(CreateAndAddBookToLibrary);
            }
        }

        bool AddExistingBook(int id)
        {
            if (id >= 0 && id < books.Count)
            {
                Console.WriteLine("How many books want you add?");
                string line = Console.ReadLine();
                try
                {
                    int stokSize = Convert.ToInt32(line);
                    AddBookToLibrary(books[id], stokSize);
                    return true;
                }
                catch
                {
                    return false;
                }
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
                book.StockAmount += stockSize ?? 1;
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

        void RemoveBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter 'Book No' or 'Book Title' to remove.");
            Console.ResetColor();
            string addLine = Console.ReadLine();

            try
            {
                int id = Convert.ToInt32(addLine);
                if (id >= 0 && id < books.Count)
                {
                    Console.WriteLine("How many books want to remove?");
                    try
                    {
                        int amount = Convert.ToInt32(Console.ReadLine());
                        RemoveBookFromLibrary(books[id], amount);
                    }
                    catch
                    {
                        ErrorCall(RemoveBook);
                    }
                }
                else
                {
                    ErrorCall(RemoveBook);
                }

            }
            catch
            {
                Book _book = GetBook(addLine);

                if (_book != null)
                {
                    Console.WriteLine("How many books want to remove?");
                    try
                    {
                        int amount = Convert.ToInt32(Console.ReadLine());
                        RemoveBookFromLibrary(books[_book.BookNo], amount);
                    }
                    catch
                    {
                        ErrorCall(RemoveBook);
                    }
                }
                else
                {
                    ErrorCall(RemoveBook, "Wrong book title!");
                }
            }
        }

        void RemoveBookFromLibrary(Book book, int? amount = null)
        {
            if (books.Contains(book))
            {
                book.StockAmount -= amount ?? 1;
                if (book.StockAmount < 0) book.StockAmount = 0;

                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\n" + (amount ?? 1) + " books '" + book.Title + "' has been removed.");
                Console.ResetColor();

            }
            RefreshBooksJson();
        }

        void DeleteBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter 'Book No' or 'Book Title' to delete book.");
            Console.ResetColor();
            string addLine = Console.ReadLine();

            try
            {
                int id = Convert.ToInt32(addLine);
                if (id >= 0 && id < books.Count)
                {
                    Console.WriteLine(books[id].Title + " removed from library list.");
                    books.RemoveAt(id);
                    RefreshBooksJson();
                }
                else
                {
                    ErrorCall(DeleteBook);
                }

            }
            catch
            {
                Book _book = GetBook(addLine);

                if (_book != null)
                {
                    Console.WriteLine(_book.Title + " removed from library list.");
                    books.RemoveAt(_book.BookNo);
                    RefreshBooksJson();
                }
                else
                {
                    ErrorCall(DeleteBook, "Wrong book title!");
                }
            }
        }

        void FindBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter 'Book No' or 'Book Title' to find and select book.");
            Console.ResetColor();

            Book seletedBook = null;

            string addLine = Console.ReadLine();

            try
            {
                int id = Convert.ToInt32(addLine);
                if (id >= 0 && id < books.Count)
                {
                    seletedBook = books[id];
                }
                else
                {
                    ErrorCall(FindBook);
                    return;
                }

            }
            catch
            {
                Book _book = GetBook(addLine);

                if (_book != null)
                {
                    seletedBook = books[_book.BookNo];
                }
                else
                {
                    ErrorCall(FindBook, "Wrong book title!");
                    return;
                }
            }

            if (seletedBook != null)
            {
                Console.BackgroundColor = ConsoleColor.Gray;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine(seletedBook.Title + " selected.\nWhat do you want to do? ");
                Console.ResetColor();
                Console.WriteLine("(b)orrow, re(t)urn, (a)dd, (r)emove, (d)elete book?");
                string line = Console.ReadLine();
                switch (line)
                {
                    case "b":
                        if (!BorrowABookWithId(seletedBook.BookNo))
                            ErrorCall(FindBook);
                        break;

                    case "t":
                        if (!ReturnABookWithId(seletedBook.BookNo))
                            ErrorCall(FindBook);
                        break;

                    case "a":
                        Console.WriteLine("How many books want to add?");
                        try
                        {
                            int amount = Convert.ToInt32(Console.ReadLine());
                            AddBookToLibrary(seletedBook, amount);
                        }
                        catch
                        {
                            ErrorCall(FindBook);
                        }
                        break;

                    case "r":
                        Console.WriteLine("How many books want to remove?");
                        try
                        {
                            int amount = Convert.ToInt32(Console.ReadLine());
                            RemoveBookFromLibrary(seletedBook, amount);
                        }
                        catch
                        {
                            ErrorCall(FindBook);
                        }
                        break;

                    case "d":
                        Console.WriteLine("Are you sure to delete this book? (y)es?");
                        string _line = Console.ReadLine();
                        if (_line == "y" && _line == "yes")
                        {
                            Console.WriteLine(seletedBook.Title + " removed from library list.");
                            books.RemoveAt(seletedBook.BookNo);
                            RefreshBooksJson();
                        }
                        else
                        {
                            ErrorCall(FindBook);
                        }
                        break;

                    default:
                        ErrorCall(FindBook);
                        break;
                }
            }

        }

        void BarrowBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter 'Book No' or 'Book Title' to borrow a book.");
            Console.ResetColor();
            string addLine = Console.ReadLine();

            try
            {
                int id = Convert.ToInt32(addLine);
                if (id >= 0 && id < books.Count)
                {
                    if (!BorrowABookWithId(id))
                        ErrorCall(BarrowBook);
                }
                else
                {
                    ErrorCall(BarrowBook);
                }

            }
            catch
            {
                Book _book = GetBook(addLine);

                if (_book != null)
                {
                    if (!BorrowABookWithId(_book.BookNo))
                        ErrorCall(BarrowBook);
                }
                else
                {
                    ErrorCall(BarrowBook, "Wrong book title!");
                }
            }
        }

        bool BorrowABookWithId(int id)
        {
            if (books[id].StockAmount > 0)
            {
                books[id].LentAmount++;
                Console.WriteLine(books[id].Title + "   borrowed from library.");
                RemoveBookFromLibrary(books[id]);
                LentBookAdd(books[id], DateTime.Today);
                return true;
            }
            else
            {
                Console.WriteLine(books[id].Title + " - book is out of library stocks");
                return false;
            }
        }

        void ReturnBook()
        {
            ShowBookList();
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter 'Book No' or 'Book Title' to return a book.");
            Console.ResetColor();
            string addLine = Console.ReadLine();

            try
            {
                int id = Convert.ToInt32(addLine);
                if (id >= 0 && id < books.Count)
                {
                    if (!ReturnABookWithId(id))
                        ErrorCall(ReturnBook);
                }
                else
                {
                    ErrorCall(ReturnBook);
                }

            }
            catch
            {
                Book _book = GetBook(addLine);

                if (_book != null)
                {
                    if (!ReturnABookWithId(_book.BookNo))
                        ErrorCall(ReturnBook);
                }
                else
                {
                    ErrorCall(BarrowBook, "Wrong book title!");
                }
            }
        }

        bool ReturnABookWithId(int id)
        {
            if (books[id].LentAmount > 0)
            {
                books[id].LentAmount--;
                Console.WriteLine(books[id].Title + " - book was returned");
                AddBookToLibrary(books[id]);
                return true;
            }
            else
            {
                Console.WriteLine(books[id].Title + " - there is no lent book");
                return false;
            }
        }





    }
}
