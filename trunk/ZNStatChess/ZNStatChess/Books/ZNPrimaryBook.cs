using System;
using System.IO;
using ZNStatChess.ChessEngine;

namespace ZNStatChess.Books
{
    internal class ZNPrimaryBook : ZNBook
    {
        static ZNPrimaryBook instanceInternal;
        static byte[] descriptionInternal = { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };

        static ZNPrimaryBook()
        {
            instanceInternal = new ZNPrimaryBook();
        }

        protected ZNPrimaryBook()
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
            get { return descriptionInternal; }
        }

        public static ZNPrimaryBook Instance
        {
            get { return instanceInternal; }
        }
    }
}
