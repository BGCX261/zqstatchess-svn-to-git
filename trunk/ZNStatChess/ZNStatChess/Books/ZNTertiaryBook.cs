using System;
using System.IO;
using ZNStatChess.ChessEngine;

namespace ZNStatChess.Books
{
    internal class ZNTertiaryBook : ZNBook
    {
        static ZNTertiaryBook instanceInternal;

        public static ZNTertiaryBook Instance
        {
            get { return instanceInternal; }
        }

        static ZNTertiaryBook()
        {
            instanceInternal = new ZNTertiaryBook();
        }

        private ZNTertiaryBook()
        {
        }

        #region ZNBook Members

        public override int CreateBook(string aPath)
        {
            throw new NotImplementedException();
        }

        public override ZNMove FindNextMove(ZNMove aLastMove)
        {
            throw new NotImplementedException();
        }

        static internal ZNBook LoadBook(string aPath)
        {
            throw new NotImplementedException();
        }

        #endregion

        protected override byte[] Description
        {
            get { throw new NotSupportedException(); }
        }

    }
}