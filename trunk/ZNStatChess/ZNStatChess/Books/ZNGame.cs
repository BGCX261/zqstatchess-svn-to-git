using System;
using ZNStatChess.Types;

namespace ZNStatChess.Books
{
    class ZNGame : IComparable
    {
        short[] movesInternal;
        ZNResult resultInternal;
        byte ratingInternal;
        int halfpointsInternal;
        int numberInternal;

        internal ZNGame(short[] m, ZNResult r, int depth, byte ra)
        {
            movesInternal = new short[depth];
            Array.Copy(m, movesInternal, depth);
            resultInternal = r;
            ratingInternal = ra;
        }

        internal ZNGame(short[] m, ZNResult r)
            : this(m, r, m.Length, 0)
        { }

        internal ZNGame(short[] m, int depth, byte ra) :
            this(m, ZNResult.ENotAvialable, depth, ra)
        { }

        internal short[] Moves
        {
            get { return movesInternal; }
        }

        internal ZNResult Result
        {
            get { return resultInternal; }
        }

        internal byte Rating
        {
            get { return ratingInternal; }
            set { ratingInternal = value; }
        }


        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }
            if (obj is ZNGame)
            {
                ZNGame other = (ZNGame)obj;
                short[] otherMoves = other.movesInternal;
                int smallerLenght = movesInternal.Length;
                if (smallerLenght > otherMoves.Length) smallerLenght = otherMoves.Length;
                for (int i = 0; i < smallerLenght; i++ )
                {
                    if (movesInternal[i] > otherMoves[i]) return 1;
                    if (movesInternal[i] < otherMoves[i]) return -1;
                }
                if (movesInternal.Length > smallerLenght) return 1;
                if (otherMoves.Length > smallerLenght) return -1;
                return 0;

            }
            throw new ArgumentException("Argument has to be a ZNGame!");
        }


        public int Number
        {
            get
            {
                return numberInternal;
            }
            set
            {
                numberInternal = value;
            }
        }

        public int Halfpoints
        {
            get
            {
                return halfpointsInternal;
            }
            set
            {
                halfpointsInternal = value;
            }
        }
    }
}
