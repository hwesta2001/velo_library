using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace VeloLibrary
{
    internal class InitRandomLibrary
    {

        internal List<Book> RandomBooks = new List<Book>();
        internal List<LentData> RandomLents = new List<LentData>();


        string[] titles = new string[] { "Whispering Shadows", "Enigma of Echoes", "Midnight Serenade", "Forgotten Dreamscape", "Siren's Lullaby", "Quantum Chronicles", "Celestial Cipher", "Velvet Embrace", "Ember Veil", "Ethereal Echoes" };

        string[] authors = new string[] { "Isabella Rainhart", "Jasper Evergreen", "Seraphina Wilde", "Atticus Sterling", "Evangeline Frost", "Thaddeus Blackwood", "Imogen Ravenhurst", "Orion Stormfield", "Lysander Nightshade", "Octavia Moonstone" };

        string[] ISBNs = new string[] { "ISBN1234567890", "ISBN2345678901", "ISBN3456789012", "ISBN4567890123", "ISBN5678901234", "ISBN6789012345", "ISBN7890123456", "ISBN8901234567", "ISBN9012345678", "ISBN0123456789" };

        public void MakeBooks()
        {
            Random rStockSize = new Random();
            Random rLentSize = new Random();
            for (int i = 0; i < titles.Length; i++)
            {
                Book book = new Book();
                book.Title = titles[i];
                book.Author = authors[i];
                book.ISBN = ISBNs[i];
                int stockSize = rStockSize.Next(1, 20);
                int lentSize = rLentSize.Next(0, stockSize);
                book.StockAmount = stockSize;
                book.LentAmount = lentSize;
                RandomBooks.Add(book);
            }

            foreach (var item in RandomBooks)
            {
                Console.WriteLine(
                    item.Title +
                    " | Author: " + item.Author +
                    " | ISBN: " + item.ISBN +
                    " | Stock Amount: " + item.StockAmount +
                    " | Lent Amount: " + item.LentAmount
                    );
            }
        }

    }
}
