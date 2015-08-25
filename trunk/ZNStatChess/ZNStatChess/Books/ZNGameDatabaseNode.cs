using System;
using System.Collections.Generic;
using System.Timers;
using System.Collections;

namespace ZNStatChess.Books
{
    class ZNGameDatabaseNode : IComparable, IComparable<ZNGameDatabaseNode>, IDisposable, ICloneable
    {
        private ZNGameDatabaseNode parentInternal;
        List<ZNGameDatabaseNode> childrenInternal;
        ZNGame valueInternal;
        ZNGameDatabase databaseInternal;

        internal ZNGameDatabaseNode Parent
        {
            get
            {
                return parentInternal;
            }
        }

        internal List<ZNGameDatabaseNode> Children
        {
            get
            {
                return childrenInternal;
            }
        }

        internal ZNGame Value
        {
            get
            {
                return valueInternal;
            }
        }

        internal ZNGameDatabaseNode(ZNGameDatabase db, ZNGameDatabaseNode parent, ZNGame value)
        {
            databaseInternal = db;
            parentInternal = parent;
            childrenInternal = new List<ZNGameDatabaseNode>();
            valueInternal = value;
        }


        #region IComparable<ZNGameDatabaseNode> Members

        public int CompareTo(ZNGameDatabaseNode other)
        {
            return valueInternal.CompareTo(other.valueInternal);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (obj is ZNGameDatabaseNode)
            {
                ZNGameDatabaseNode other = (ZNGameDatabaseNode)obj;
                return CompareTo(other);
            }
            throw new ArgumentException("Argument has to be a ZNGameDatabaseNode!"); 
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            foreach (ZNGameDatabaseNode child in childrenInternal)
            {
                child.Dispose();
            }
            childrenInternal = null;
        }

        #endregion

        void AddChild(ZNGameDatabaseNode child)
        {
            childrenInternal.Add(child);
            databaseInternal.NeedSort = true;
        }

        void RemoveRange(ZNGameDatabaseNode child, int index, int count)
        {
            childrenInternal.RemoveRange(index, count);
        }


        internal void Sort()
        {
            childrenInternal.Sort();
            foreach (ZNGameDatabaseNode child in childrenInternal)
            {
                child.Sort();
            }
        }

        internal bool HasChild()
        {
            return childrenInternal.Count != 0;
        }

        internal ZNGameDatabaseNode NextChild(ZNGameDatabaseNode current)
        {
            if (childrenInternal.Count == 0) return null;
            int index = childrenInternal.IndexOf(current);
            if (index == childrenInternal.Count - 1) return null;
            return childrenInternal[index + 1];
        }

        internal void Add()
        {
            throw new NotImplementedException();
        }

        internal void Comprim(byte depth)
        {
            throw new NotImplementedException();
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        internal void Remove()
        {
            throw new NotImplementedException();
        }
    }
}
