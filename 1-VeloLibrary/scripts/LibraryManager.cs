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
        public string jsonFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "books.json");
        public List<Book> books = new List<Book>();

        public void CreateLibrary()
        {
            if (File.Exists(jsonFilePath))
            {
                books.Clear();
                books = ReadJsonToList(jsonFilePath);
                Console.WriteLine("Existing Library Loaded.");
            }
            else
            {
                InitJsonFile();
                Console.WriteLine("New Library Created.");
            }

            ShowBookList();
        }

        public void ShowBookList()
        {
            Console.WriteLine("Book List:");
            foreach (var book in books)
            {
                Console.WriteLine($"BookId: {book.BookId}, Title: {book.Title}, Author: {book.Author}, ISBN: {book.ISBN}, StockAmount: {book.StockAmount}, LentAmount: {book.LentAmount}");
            }
        }

        public void CreateAndAddBookToLibrary()
        {
            Book book = new Book();
            Console.WriteLine("Book Title: ");
            book.Title = Console.ReadLine();
            Console.WriteLine("Book Author: ");
            book.Author = Console.ReadLine();
            Console.WriteLine("Book ISBN: ");
            book.ISBN = Console.ReadLine();
            Console.WriteLine("Book added");
            AddBookToLibrary(book);
        }

        public bool AddExistingBook(int id)
        {
            if (id < 0 || id >= books.Count)
            {
                return false;
            }
            else
            {
                AddBookToLibrary(GetBookWithId(id));
                return true;
            }
        }

        public Book GetBookWithId(int id)
        {
            return books[id];
        }

        private void InitJsonFile()
        {
            Book dummyBook0 = new Book
            {
                BookId = 10,
                Title = "Dummy_Book",
                Author = "Dummy_Author",
                ISBN = "000-0000000000",
                StockAmount = 0,
                LentAmount = 0
            };

            Book dummyBook01 = new Book
            {
                BookId = 11,
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

        public void AddBookToLibrary(Book book)
        {
            if (books.Contains(book))
            {
                book.StockAmount++;
            }
            else
            {
                book.StockAmount = 1;
                books.Add(book);
            }
            RefreshBooksJson();
        }

        public void RemoveBookToLibrary(Book book)
        {
            if (books.Contains(book))
            {
                book.StockAmount--;
                if (book.StockAmount <= 0)
                {
                    books.Remove(book);
                    Console.WriteLine(book.Title + " removed from library.");
                }
            }
            RefreshBooksJson();
        }

        public void RefreshBooksJson()
        {
            foreach (Book book in books)
            {
                //bookid leri indexlere eşleyerek id ile kitap çağrmayı kolaylaştırabiliriz.
                book.BookId = books.IndexOf(book);
            }
            File.WriteAllText(jsonFilePath, JsonConvert.SerializeObject(books, Formatting.Indented));
        }

        List<Book> ReadJsonToList(string filePath)
        {
            return JsonConvert.DeserializeObject<List<Book>>(File.ReadAllText(filePath));
        }


    }
}
