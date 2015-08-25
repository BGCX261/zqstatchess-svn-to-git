using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZNStatChess.Types;

namespace ZNStatChess.ChessEngine
{
    class ZNBoard
    {
        private static ZNPiece WK = new ZNPiece(ZNColor.EWhite, ZNPieceType.EKing, 32);
        private static ZNPiece WQ = new ZNPiece(ZNColor.EWhite, ZNPieceType.EQueen, 24);
        private static ZNPiece WB1 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EBishop, 16);
        private static ZNPiece WB2 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EBishop, 40);
        private static ZNPiece WN1 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EKnight, 8);
        private static ZNPiece WN2 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EKnight, 48);
        private static ZNPiece WR1 = new ZNPiece(ZNColor.EWhite, ZNPieceType.ERook, 0);
        private static ZNPiece WR2 = new ZNPiece(ZNColor.EWhite, ZNPieceType.ERook, 56);

        private static ZNPiece WP1 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 1);
        private static ZNPiece WP2 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 9);
        private static ZNPiece WP3 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 17);
        private static ZNPiece WP4 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 25);
        private static ZNPiece WP5 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 33);
        private static ZNPiece WP6 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 41);
        private static ZNPiece WP7 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 49);
        private static ZNPiece WP8 = new ZNPiece(ZNColor.EWhite, ZNPieceType.EPawn, 57);

        private static ZNPiece BK = new ZNPiece(ZNColor.EBlack, ZNPieceType.EKing, 39);
        private static ZNPiece BQ = new ZNPiece(ZNColor.EBlack, ZNPieceType.EQueen, 31);
        private static ZNPiece BB1 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EBishop, 23);
        private static ZNPiece BB2 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EBishop, 47);
        private static ZNPiece BN1 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EKnight, 15);
        private static ZNPiece BN2 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EKnight, 55);
        private static ZNPiece BR1 = new ZNPiece(ZNColor.EBlack, ZNPieceType.ERook, 7);
        private static ZNPiece BR2 = new ZNPiece(ZNColor.EBlack, ZNPieceType.ERook, 63);

        private static ZNPiece BP1 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 6);
        private static ZNPiece BP2 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 14);
        private static ZNPiece BP3 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 22);
        private static ZNPiece BP4 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 30);
        private static ZNPiece BP5 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 38);
        private static ZNPiece BP6 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 46);
        private static ZNPiece BP7 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 54);
        private static ZNPiece BP8 = new ZNPiece(ZNColor.EBlack, ZNPieceType.EPawn, 62);

        private static ZNPiece EMPTY = new ZNPiece();

        private static ZNPiece[] DEFAULT_POSITIONS = { WK, WQ, WB1, WB2, WN1, WN2, WR1, WR2,
                                                       WP1, WP2, WP3, WP4, WP5, WP6, WP7, WP8,
                                                       BK, BQ, BB1, BB2, BN1, BN2, BR1, BR2,
                                                       BP1, BP2, BP3, BP4, BP5, BP6, BP7, BP8
                                                     };

        private ZNPiece[] piecesInternal;

        internal ZNBoard()
        {
            piecesInternal = new ZNPiece[64];
            for (int i = 0; i < 64; i++)
            {
                piecesInternal[i] = EMPTY;
            }
            ZNPiece p;
            for (int i = 0; i < 32; i++)
            {
                p = DEFAULT_POSITIONS[i];
                piecesInternal[p.Position] = p;
            }
        }

        internal ZNPiece this[int position]
        {
            get
            {
                return piecesInternal[position];
            }
        }

        internal bool DoMove(short move)
        {
            byte oldPosition = (byte)(move & 0xff00 >> 2);
            byte newPosition = (byte)(move & 0x00ff);
            ZNPiece piece = piecesInternal[oldPosition];
            piecesInternal[oldPosition] = EMPTY;
            piecesInternal[newPosition] = piece;

            return true;
        }

        internal bool DoMove(ZNMove move)
        {
            return DoMove(move.iMove);
        }

        internal void DoPromotion(byte position, ZNPieceType newType)
        {
            ZNPiece piece = piecesInternal[position];
            if (piece.Type != ZNPieceType.EPawn)
            {
                throw new ArgumentException("This is not a pawn!");
            }
            piecesInternal[position] = new ZNPiece(piece.Color, newType, position);
        }
    }
}
