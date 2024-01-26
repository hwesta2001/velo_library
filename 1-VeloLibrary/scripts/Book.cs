using System;

namespace VeloLibrary
{
    internal class Book
    {
        public int BookNo;
        public string Title;
        public string Author;
        public string ISBN;
        public int StockAmount;
        public int LentAmount;
        public int TotalAmount;
    }

    internal class LentData
    {
        public int UniqLentId; // Return book bu id ile yapılır.
        public Book book;
        public DateTime BorrowTime;
        public DateTime ReturnTime;

        public LentData(int uLentId, Book book, DateTime borrowTime)
        {
            UniqLentId = uLentId;
            this.book = book;
            BorrowTime = borrowTime;
            ReturnTime = borrowTime.AddDays(14); // 14 gün teslin süresi
        }
    }

}
