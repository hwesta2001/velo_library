using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace VeloLibrary
{
    internal class LibraryManager
    {
        readonly string books_jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.json");
        readonly string lents_jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lents.json");

        // BOOKS ve LENT_BOOKS property şeklinde atanmasının sebebi...
        // başka sistemler ve classlar tarfından da çekilip kullanılabilir olmasını sağlamak içindir.
        public List<Book> BOOKS { get; private set; } = new List<Book>();
        public List<LentData> LENT_BOOKS { get; private set; } = new List<LentData>();

        ///uniqueLentId her kitap ödünç alındığında bir arttırılır...
        //bu idler her ödünç kitap için eşsiz olduğu için kitap ödünc alımında ekrana yazdırılır...
        //... iade işlemi bu uniqueLentId ile yapılır.
        int uniqueLentId = 0;

        void LentBookAdd(Book book, DateTime borrowTime)
        {
            uniqueLentId++;
            LentData ld = new LentData(uniqueLentId, book, borrowTime);
            LENT_BOOKS.Add(ld);
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nYou borrowed the book '" + book.Title + "'.");
            Console.ForegroundColor = ConsoleColor.Black;
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine("Your Unique Lent Book ID is: '" + ld.UniqLentId + "'            ");
            Console.WriteLine("Don't forget 'Unique Lent Book ID'!            ");
            Console.WriteLine("You need it for return the book.               ");
            Console.ResetColor();
            RefreshLentsJson();
        }
        bool GetLentedBookWithLentId(int uniqLentId, out Book book, out int lentIndex)
        {
            lentIndex = -1;
            book = null;
            bool r_bool = false;
            for (int i = 0; i < LENT_BOOKS.Count; i++)
            {
                if (uniqLentId == LENT_BOOKS[i].UniqLentId)
                {
                    book = LENT_BOOKS[i].book;
                    lentIndex = i;
                    r_bool = true;
                }
            }
            return r_bool;
        }
        Book GetBook(string title)
        {
            for (int i = 0; i < BOOKS.Count; i++)
            {
                if (title == BOOKS[i].Title)
                {
                    return BOOKS[i];
                }
            }
            return null;
        }
        Book[] GetBooks(string author)
        {
            Book[] _books = null;
            for (int i = 0; i < BOOKS.Count; i++)
            {
                if (author == BOOKS[i].Author)
                {
                    _books.Append(BOOKS[i]).ToArray();
                }
            }
            return _books;
        }

        bool IsYes(string line)
        {
            switch (line)
            {
                case "y":
                case "yes":
                case "Y":
                case "Yes":
                case "YES":
                    return true;
                default:
                    return false;
            }
        }
        bool InvalidStringEntry(string line)
        {
            switch (line)
            {
                // simdilik on adet space e kadar invalidString sayılıcak
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

        #region JSON Methods

        void InitBookJsonFile()
        {
            InitRandomLibrary irb = new InitRandomLibrary();
            irb.MakeBooks();
            BOOKS.Clear();
            BOOKS = irb.RandomBooks;
            RefreshBooksJson();
        }
        void InitLentJsonFile()
        {
            LENT_BOOKS.Clear();

            // rasgele 0-20 gün arasını bugünden çıkarıp BorrowTime belirlenir.
            // 14günden büyük cıkan günler için gecikme meydana gelmiş demektir
            Random rLentTimeBeforeInitDay = new Random();
            foreach (var book in BOOKS)
            {
                if (book.LentAmount > 0)
                {
                    for (int i = 0; i < book.LentAmount; i++)
                    {
                        int subtractedDays = rLentTimeBeforeInitDay.Next(0, 20);
                        LentBookAdd(book, DateTime.Today.AddDays(-subtractedDays));
                    }
                }
            }
            RefreshLentsJson();
        }

        void RefreshBooksJson()
        {
            foreach (Book book in BOOKS)
            {
                //her kitap eklenip cıkarıldığında list indexlere göre kitaplar sıralanır
                //bookno lari indexlere eşleyerek no ile kitap çağrmayı kolaylaştırabiliriz.
                book.BookNo = BOOKS.IndexOf(book);
                // totalAmount her seferinde lent+stock size olacaktır.
                book.TotalAmount = book.LentAmount + book.StockAmount;
            }
            // her kitap ekleme çıkarma işlemi sonrası json dosyamızı güncelliyoruz.
            File.WriteAllText(books_jsonFilePath, JsonConvert.SerializeObject(BOOKS, Formatting.Indented));
        }
        void RefreshLentsJson()
        {
            File.WriteAllText(lents_jsonFilePath, JsonConvert.SerializeObject(LENT_BOOKS, Formatting.Indented));
        }

        #endregion


        public void CreateLibrary() // ANA CALISTIRICI. 
        {
            //book.json dosyası varsa ordan books listesini cekip oluşturur.
            //book.json dosyası yoksa yeni bir tane oluşturup randomBook lar ekler.
            if (File.Exists(books_jsonFilePath))
            {
                BOOKS.Clear();
                BOOKS = JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(books_jsonFilePath));

                if (File.Exists(lents_jsonFilePath))
                {
                    //lents.json dosyası varsa lent bookları ordan çeker.
                    LENT_BOOKS.Clear();
                    LENT_BOOKS = JsonConvert.DeserializeObject<List<LentData>>(File.ReadAllText(lents_jsonFilePath));
                }
                else
                {
                    //lents.json dosyası yoksa yeni bir random lents.json yaratır.
                    InitLentJsonFile();
                }

            }
            else
            {
                InitBookJsonFile();                 // random book lar ekleniyor.
                File.Delete(lents_jsonFilePath);    // lentJson file siliyor ki aşağıda yeniden yapılacak.
                InitLentJsonFile();                 // yeni bir random lents.json yaratır.
            }



            // her açılışta lents dataadki son UniqLentId ile eşleştir.
            // böylece her açılışta uniqueLentId ler kaldığı yerden devam eder.
            uniqueLentId = LENT_BOOKS.Last<LentData>().UniqLentId;

            Console.Clear();
            WelcomeSheet();
            Operations();
        }
        void WelcomeSheet()
        {
            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("******************************************");
            Console.WriteLine("*******  Welcome to Velo Library   *******");
            Console.WriteLine("*******    Made by Hasan YILMAZ    *******");
            Console.WriteLine("******************************************");
            Console.ResetColor();
        }

        void Operations()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n*******        OPERATIONS           ******");
            Console.ResetColor();

            Console.WriteLine(" s - (S)how books list.");                     //done
            Console.WriteLine(" a - (A)dd a book to library");                //done
            Console.WriteLine(" r - (R)emove a book from library");           //done
            Console.WriteLine(" d - (D)elete book from library list");        //done
            Console.WriteLine(" f - (F)ind a book in library");               //done
            Console.WriteLine(" b - (B)orrow a book from library");           //done
            Console.WriteLine(" t - Re(t)urn a book to library");             //done
            Console.WriteLine(" l - Show (l)ent books list.");                //done
            Console.WriteLine(" o - Show (o)verdue books list.");             //done
            Console.WriteLine(" c - (C)lear");                                //done
            Console.WriteLine(" x - E(x)it");                                 //done

            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" What do you want to do?                  ");
            Console.ResetColor();

            switch (Console.ReadLine())
            {

                case "s":
                case "S":
                    ShowBookList();
                    //toplam kitap sayısı hesapla:
                    int total = 0;
                    foreach (var item in BOOKS) total += item.TotalAmount;
                    Console.WriteLine($"\nThere are {BOOKS.Count} different titled books in library. ");
                    Console.WriteLine($"There are {total} total books in library.                  ");
                    PressToContinue();
                    Operations();
                    break;


                case "a":
                case "A":
                    AddBook();
                    PressToContinue();
                    Operations();
                    break;

                case "R":
                case "r":
                    RemoveBook();
                    PressToContinue();
                    Operations();
                    break;

                case "D":
                case "d":
                    DeleteBook();
                    PressToContinue();
                    Operations();
                    break;

                case "F":
                case "f":
                    FindBook();
                    PressToContinue();
                    Operations();
                    break;

                case "B":
                case "b":
                    BarrowBook();
                    PressToContinue();
                    Operations();
                    break;

                case "T":
                case "t":
                    ShowLentBookList();
                    ReturnBook();
                    PressToContinue();
                    Operations();
                    break;

                case "L":
                case "l":
                    ShowLentBookList();
                    Console.WriteLine("t - Do you want to re(t)urn a book?");
                    if (Console.ReadLine() == "t")
                    {
                        ReturnBook();
                    }
                    PressToContinue();
                    Operations();
                    break;

                case "O":
                case "o":
                    ShowOverdueBooks();
                    PressToContinue();
                    Operations();
                    break;

                //********************//
                // console operations //
                //********************//
                case "C":
                case "c":
                    Console.Clear();
                    Operations();
                    break;

                case "x":
                case "X":
                    Environment.Exit(0);
                    break;

                //********************//
                // default operation  //
                //********************//
                default:
                    Console.WriteLine("Invalid entry                        ");
                    Console.WriteLine("Would you like to continue? (y)es?   ");
                    string line = Console.ReadLine();
                    if (IsYes(line))
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
            Console.WriteLine((message ?? "Invalid entry! ") + " Want to try again? (y)es?");
            string line = Console.ReadLine();
            if (IsYes(line))
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
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" Press any key to continue...             ");
            Console.ResetColor();
            Console.ReadKey();
        }

        void ShowBookList()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n  Book List:                              ");
            Console.ResetColor();
            foreach (var book in BOOKS)
            {
                Console.WriteLine($"{book.BookNo} - {book.Title} | Author: {book.Author} | ISBN: {book.ISBN} | Stock: {book.StockAmount} | Lent: {book.LentAmount} | Total: {book.TotalAmount}");
            }

        }

        void ShowLentBookList()
        {
            if (LENT_BOOKS.Count > 0)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("\n  Lent Books List:                        ");
                Console.WriteLine("BT:Borrow Time, RT:Retun Time               ");
                Console.ResetColor();

                foreach (var lent in LENT_BOOKS)
                {
                    Console.WriteLine($"{lent.book.BookNo} - LentId {lent.UniqLentId} - {lent.book.Title} | {lent.book.Author} | {lent.book.ISBN} | BT: {lent.BorrowTime.ToShortDateString()} | RT: {lent.ReturnTime.ToShortDateString()}");
                }
                Console.WriteLine(" o - Do you want list (o)verdue books?   ");
                Console.WriteLine(" Or enter any key to continue...         ");
                string line = Console.ReadLine();
                switch (line)
                {
                    case "o":
                        ShowOverdueBooks();
                        break;
                    default:
                        break;
                }
            }
            else
            {
                Console.WriteLine("\n  There are no lent books now.             ");
            }
        }

        void ShowOverdueBooks()
        {
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(" OVERDUE BOOKS ");

            Console.WriteLine($"TODAY: {DateTime.Today.ToShortDateString()} ");
            Console.WriteLine("Book return period is 14 days.");

            Console.ResetColor();

            if (LENT_BOOKS.Count > 0)
            {
                List<LentData> expiredBooks = new List<LentData>();
                for (int i = 0; i < LENT_BOOKS.Count; i++)
                {
                    if (DateTime.Today.Subtract(LENT_BOOKS[i].BorrowTime).Days > 14)
                    {
                        expiredBooks.Add(LENT_BOOKS[i]);
                    }
                }
                if (expiredBooks.Count > 0)
                {
                    foreach (var item in expiredBooks)
                    {
                        Console.WriteLine($"{item.book.BookNo} - LentId {item.UniqLentId} - {item.book.Title} | {item.book.Author} | {item.book.ISBN} | BT: {item.BorrowTime.ToShortDateString()} | RT: {item.ReturnTime.ToShortDateString()} | OT: {DateTime.Today.Subtract(item.BorrowTime).Days - 14}");
                    }
                    Console.WriteLine("BT:Borrow Time, RT:Retun Time, OT: Overdue Days");
                    Console.WriteLine("Enter 'd' for (d)elete an overdue book ");
                    if (Console.ReadLine() == "d") DeleteOverDueBook();
                }
                else
                {
                    Console.WriteLine("There is no overdue books");
                }

            }
            else
            {
                Console.WriteLine("There are no lent books");
            }
        }

        void DeleteOverDueBook()
        {
            Console.WriteLine("Enter 'Lent Id' for delete for an overdue book.  ");
            Console.WriteLine("Enter 'o' for show list (o)verdue books.         ");
            string addLine = Console.ReadLine();
            try
            {
                int lentId = Convert.ToInt32(addLine);
                if (GetLentedBookWithLentId(lentId, out Book book, out int index))
                {
                    book.LentAmount--;
                    LENT_BOOKS.RemoveAt(index);
                    Console.WriteLine($"{book.Title} an overdue book deleted from library.");
                    RefreshLentsJson();
                    RefreshBooksJson();
                    if (book.TotalAmount <= 0)
                    {
                        Console.WriteLine("Wanna delete book in the library list? (y)es?");
                        string line = Console.ReadLine();
                        if (IsYes(line))
                        {
                            Console.WriteLine(BOOKS[book.BookNo].Title + " removed from library list.");
                            BOOKS.RemoveAt(book.BookNo);
                            RefreshBooksJson();
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Wrong LentId");
                }

            }
            catch
            {
                switch (addLine)
                {
                    case "o":
                    case "O":
                        ShowOverdueBooks();
                        break;

                    default:
                        Console.WriteLine("Invalid entry");
                        break;
                }
            }


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
            if (id >= 0 && id < BOOKS.Count)
            {
                Console.WriteLine("How many books want you add?");
                string line = Console.ReadLine();
                try
                {
                    int stokSize = Convert.ToInt32(line);
                    AddBookToLibrary(BOOKS[id], stokSize);
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
            if (BOOKS.Contains(book))
            {
                book.StockAmount += stockSize ?? 1;
            }
            else
            {
                book.StockAmount = stockSize ?? 1;
                BOOKS.Add(book);
            }
            Console.BackgroundColor = ConsoleColor.Yellow;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("\n" + (stockSize ?? 1) + " books '" + book.Title + "' has been added. ");
            Console.WriteLine("There are '" + book.StockAmount + "' books in the library now.  ");
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
                if (id >= 0 && id < BOOKS.Count)
                {
                    Console.WriteLine("How many books want to remove?");
                    try
                    {
                        int amount = Convert.ToInt32(Console.ReadLine());
                        RemoveBookFromLibrary(BOOKS[id], amount);
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
                        RemoveBookFromLibrary(BOOKS[_book.BookNo], amount);
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
            if (BOOKS.Contains(book))
            {
                if (book.StockAmount <= 0)
                {
                    Console.WriteLine("'" + book.Title + "' library does not have this book in stock.");
                }
                else
                {
                    book.StockAmount -= amount ?? 1;
                    Console.WriteLine((amount ?? 1) + " books '" + book.Title + "' has been removed.");
                    Console.WriteLine("There are '" + book.StockAmount + "' books in the library now.");
                }
                if (book.StockAmount < 0) book.StockAmount = 0;

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
                if (id >= 0 && id < BOOKS.Count)
                {
                    if (BOOKS[id].LentAmount > 0)
                    {
                        ErrorCall(DeleteBook, "You can not delete lent books.\nPlease wait for return all books.");
                    }
                    else
                    {
                        Console.WriteLine(BOOKS[id].Title + " removed from library list.");
                        BOOKS.RemoveAt(id);
                        RefreshBooksJson();
                    }
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
                    BOOKS.RemoveAt(_book.BookNo);
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
                if (id >= 0 && id < BOOKS.Count)
                {
                    seletedBook = BOOKS[id];
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
                    seletedBook = BOOKS[_book.BookNo];
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
                Console.WriteLine("(b)orrow, (a)dd, (r)emove, (d)elete book?");
                string line = Console.ReadLine();
                switch (line)
                {
                    case "b":
                        if (!BorrowABookWithId(seletedBook.BookNo))
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

                        if (seletedBook.LentAmount > 0)
                        {
                            ErrorCall(FindBook, (seletedBook.Title + " book was lent.\nYou can not delete lent books.\nPlease wait for return all books"));
                        }
                        else
                        {
                            Console.WriteLine("Are you sure to delete this book? (y)es?");
                            string d_line = Console.ReadLine();
                            if (IsYes(d_line))
                            {
                                Console.WriteLine(seletedBook.Title + " removed from library list.");
                                BOOKS.RemoveAt(seletedBook.BookNo);
                                RefreshBooksJson();
                            }
                            else
                            {
                                ErrorCall(FindBook);
                            }
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
                if (id >= 0 && id < BOOKS.Count)
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
            if (BOOKS[id].StockAmount > 0)
            {
                BOOKS[id].LentAmount++;
                Console.WriteLine(BOOKS[id].Title + "   borrowed from library.");
                RemoveBookFromLibrary(BOOKS[id]);
                LentBookAdd(BOOKS[id], DateTime.Today);
                return true;
            }
            else
            {
                Console.WriteLine(BOOKS[id].Title + " - book is out of library stocks");
                return false;
            }
        }

        void ReturnBook()
        {
            Console.BackgroundColor = ConsoleColor.Gray;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine("Enter (l) - Show (l)ent books list or...    ");
            Console.WriteLine("Enter ' Unique Lent ID' to return your book.");
            Console.ResetColor();
            string addLine = Console.ReadLine();

            try
            {
                int id = Convert.ToInt32(addLine);
                if (GetLentedBookWithLentId(id, out Book book, out int lentIndex))
                {
                    if (ReturnABookWithId(book.BookNo))
                    {
                        LENT_BOOKS.RemoveAt(lentIndex);
                        RefreshLentsJson();
                    }
                    else
                    {
                        ErrorCall(ReturnBook);
                    }
                }
                else
                {
                    ErrorCall(ReturnBook, "You entered invalid LentId");
                }

            }
            catch
            {
                if (addLine == "l")
                {
                    ShowLentBookList();
                    ReturnBook();
                }
                else
                {
                    ErrorCall(ReturnBook);
                }
            }
        }

        bool ReturnABookWithId(int id)
        {
            if (BOOKS[id].LentAmount > 0)
            {
                BOOKS[id].LentAmount--;
                Console.WriteLine(BOOKS[id].Title + " - book was returned");
                AddBookToLibrary(BOOKS[id]);
                return true;
            }
            else
            {
                Console.WriteLine(BOOKS[id].Title + " - there is no lent book");
                return false;
            }
        }
    }
}
