﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZNStatChess.Types
{
    public struct ZNPair<TKey, TValue> : IComparable<ZNPair<TKey, TValue>>
        where TKey : IComparable
    {
        private TKey key;
        private TValue value;

        public ZNPair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public TKey Key
        {
            get { return key; }
        }

        public TValue Value
        {
            get { return value; }
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            s.Append('[');
            if (Key != null)
            {
                s.Append(Key.ToString());
            }
            s.Append(", ");
            if (Value != null)
            {
                s.Append(Value.ToString());
            }
            s.Append(']');
            return s.ToString();
        }


        public int CompareTo(ZNPair<TKey, TValue> other)
        {
            return key.CompareTo(other.key);
        }
    }
}
