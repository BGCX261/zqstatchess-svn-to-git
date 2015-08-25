using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZNStatChess.Types;

namespace ZNStatChess.ChessEngine
{
    struct ZNPiece
    {
        private ZNColor colorInternal;
        private ZNPieceType typeInternal;
        private byte positionInternal;

        internal ZNPiece(byte position)
        {
            colorInternal = ZNColor.EEmpty;
            typeInternal = ZNPieceType.EEmpty;
            positionInternal = position;
        }
        internal ZNPiece(ZNColor color, ZNPieceType type, byte position)
        {
            colorInternal = color;
            typeInternal = type;
            positionInternal = position;
        }

        internal ZNColor Color
        {
            get
            {
                return colorInternal;
            }
        }
        internal ZNPieceType Type
        {
            get
            {
                return typeInternal;
            }
            set
            {
                typeInternal = value;
            }
        }
        internal byte Position
        {
            get
            {
                return positionInternal;
            }
            set
            {
                positionInternal = value;
            }
        }
    }
}
