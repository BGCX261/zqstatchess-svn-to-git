using System;
using System.Collections;
using System.Timers;

namespace ZNStatChess.Books
{
    class ZNGameDatabase : IEnumerable, ICloneable
    {
        byte depth;
        ZNGameDatabaseNode root;
        bool needSort;
        Timer sortTimer;

        internal bool NeedSort
        {
            set
            {
                needSort = value;
            }
        }

        internal ZNGameDatabaseNode Root
        {
            get
            {
                return root;
            }
        }

        public ZNGameDatabase(byte de)
        {
            depth = de;
            root = new ZNGameDatabaseNode(this, null, null);
            sortTimer = new Timer(100);
            sortTimer.Elapsed += new ElapsedEventHandler(sortHandle);
            sortTimer.Start();
        }

        #region IEnumerable Members

        public IEnumerator GetEnumerator()
        {
            return new ZNGameDatabaseEnumerator(this);
        }

        #endregion

        void sortHandle(object sender, ElapsedEventArgs e)
        {
            if (needSort)
            {
                root.Sort();
                needSort = false;
            }

        }

        internal void Add(ZNGame game)
        {
            root.Add();
        }

        internal void Comprim()
        {
            root.Comprim(depth);
        }

        public object Clone()
        {
            return root.Clone();
        }

        internal void Comprim(byte step)
        {
            root.Comprim(step);
        }

        internal void Remove(ZNGame g)
        {
            root.Remove();
        }

        internal void RemoveTheseAndTheirChildren(ZNGameDatabase gameDatabaseShallower, out IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }
}
