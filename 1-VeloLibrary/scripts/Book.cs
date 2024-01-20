using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace VeloLibrary
{
    internal class Book
    {
        public int BookId {  get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public int StockAmount { get; set; }
        public uint LentAmount { get; set; }
    }
}
