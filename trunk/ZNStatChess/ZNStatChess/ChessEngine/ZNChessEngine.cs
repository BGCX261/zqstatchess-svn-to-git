using System;
using ZNStatChess.Books;
using ZNStatChess.Types;
using System.Threading;

namespace ZNStatChess.ChessEngine 
{
    public delegate void GoDelegate();
    class ZNChessEngine
    {
        ZNBook pbook;
        ZNBook sbook;
        ZNBook tbook;
        int hash;

        Thread worker;
        public GoDelegate Go;  // TODO: Je to k necemu?

        internal ZNChessEngine(int? hashSize, string pathToPbook, string pathToSbook, string pathToTBook)
        {
            pbook = ZNPrimaryBook.LoadBook(pathToPbook);
            sbook = ZNSecondaryBook.LoadBook(pathToSbook);
            tbook = ZNTertiaryBook.LoadBook(pathToTBook);
            if (hashSize != null)
            {
                hash = (int)hashSize;
            }
            Go = new GoDelegate(DoGo);
        }

        private void DoGo()
        {
        }

    }
}
