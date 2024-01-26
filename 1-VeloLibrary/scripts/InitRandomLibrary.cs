using System;
using System.Collections.Generic;

namespace VeloLibrary
{
    internal class InitRandomLibrary
    {

        internal List<Book> RandomBooks = new List<Book>();
        internal List<LentData> RandomLents = new List<LentData>();

        string[] titles = new string[]
        {
            // 10 Random Title
            "Whispering Shadows",     //0
            "Enigma of Echoes",       //1
            "Midnight Serenade",      //2
            "Forgotten Dreamscape",   //3
            "Siren's Lullaby",        //4
            "Quantum Chronicles",     //5
            "Celestial Cipher",       //6
            "Velvet Embrace",         //7
            "Ember Veil",             //8
            "Ethereal Echoes"         //9
        };

        string[] authors = new string[]
        { 
            //10 Random Author Name
            "Isabella Rainhart",      //0
            "Isabella Rainhart",       //1    
            "Isabella Rainhart",        //2
            "Atticus Sterling",       //3
            "Evangeline Frost",       //4    
            "Thaddeus Blackwood",     //5    
            "Imogen Ravenhurst",      //6    
            "Imogen Ravenhurst",       //7    
            "Lysander Nightshade",    //8    
            "Octavia Moonstone"       //9     
        };

        string[] ISBNs = new string[]
        { 
            //10 Random ISBN
            "ISBN1234567890",         //0
            "ISBN2345678901",         //1
            "ISBN3456789012",         //2
            "ISBN4567890123",         //3
            "ISBN5678901234",         //4
            "ISBN6789012345",         //5
            "ISBN7890123456",         //6
            "ISBN8901234567",         //7
            "ISBN9012345678",         //8
            "ISBN0123456789"          //9
        };

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
                int lentSize = rLentSize.Next(0, (int)(stockSize * .5f));
                book.StockAmount = stockSize;
                book.LentAmount = lentSize;
                RandomBooks.Add(book);
            }
        }

    }
}
