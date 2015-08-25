using System;
using System.Collections;
using System.Collections.Generic;

namespace ZNStatChess.Books
{
    internal class ZNGameDatabaseEnumerator : IEnumerator
    {
        private ZNGameDatabase data;
        private ZNGameDatabaseNode current;

        public ZNGameDatabaseEnumerator(ZNGameDatabase d)
        {
            data = d;
            current = data.Root;
        }

        #region IEnumerator Members

        public object Current
        {
            get
            {
                return current;
            }
        }

        public bool MoveNext()
        {
            if (current.HasChild())
            {
                current = current.Children[0];
                return true;
            }
            else
            {
                current = current.Parent.NextChild(current);
                return (current != null);
            }
        }

        public void Reset()
        {
            current = data.Root;
        }

        #endregion
    }
}
