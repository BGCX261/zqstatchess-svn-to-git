using System;
using ZNStatChess.Types;
using System.IO;

namespace ZNStatChess.Books
{
    internal class ZNSecondaryBook : ZNPrimaryBook
    {
        static ZNSecondaryBook instanceInternal;
        static byte[] descriptionInternal = { 8, 8, 8, 8, 8, 8, 8, 8, 8 };

        public new static ZNSecondaryBook Instance
        {
            get { return instanceInternal; }
        }

        static ZNSecondaryBook()
        {
            instanceInternal = new ZNSecondaryBook();
        }

        private ZNSecondaryBook()
        {

        }

        protected override byte[] Description
        {
            get { return descriptionInternal; }
        }
    }
}