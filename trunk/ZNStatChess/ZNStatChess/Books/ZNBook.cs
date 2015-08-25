using System;
using ZNStatChess.ChessEngine;
using ZNStatChess.Types;
using System.IO;
using System.Text;
using System.Threading;
using System.Collections.Generic;
using System.Collections;

namespace ZNStatChess.Books
{
    abstract class ZNBook
    {
        abstract protected byte[] Description
        {
            get;
        }

        abstract public ZNMove FindNextMove(ZNMove aLastMove);
        abstract public int CreateBook(string aPath);

        private int CreateBook(ZNBookTypes type, string path)
        {
            byte[] description = { };
            switch (type)
            {
                case ZNBookTypes.EPrimary:
                    description = ZNPrimaryBook.Instance.Description;
                    return CreateBookWithDescription(description, path);
                case ZNBookTypes.ESecondary:
                    description = ZNSecondaryBook.Instance.Description;
                    return CreateBookWithDescription(description, path);
                case ZNBookTypes.ETercialy:
                    return CreateBookWithAllMoves(path);
            }
            return ZNConstants.EXIT_FAILURE;
        }

        private int CreateBookWithAllMoves(string path)
        {
            throw new NotImplementedException();
        }

        private int CreateBookWithDescription(byte[] description, string path)
        {
            if (File.Exists(path + ".ogb"))
            {
                File.Move(path + ".ogb", path + DateTime.Now + ".ogb");
            }
            FileStream writer = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 40960, true);
            StreamReader reader = File.OpenText(path);
            string line;
            byte depth = (byte)description.Length;
            ZNGameDatabase gameDatabase = new ZNGameDatabase(depth);
            StringBuilder pgnGame = new StringBuilder();
            ZNGame game;
            int gamesInDatabase;

            while (true)
            {
                line = reader.ReadLine();
                if (line == null) break;
                pgnGame.Append(line);
                if (IsWholePGNGame(pgnGame.ToString()))
                {
                    game = GetGameFromPGN(pgnGame.ToString());
                    if (game != null)
                    {
                        gameDatabase.Add(game);
                    }
                    // TODO: Creating of file with damaged games.
                    pgnGame.Clear();
                }
            }
            reader.Close();
            Thread.Sleep(125); // For gameDatabase.Sort();

            gameDatabase.Comprim();
            ZNGameDatabase gameDatabaseShallower;
            List<ZNGame> gamesForDelete;

            for (byte step = 1; step < depth; step++)
            {
                gamesForDelete = new List<ZNGame>();

                gameDatabaseShallower = gameDatabase.Clone() as ZNGameDatabase;
                gameDatabaseShallower.Comprim(step);

                gamesInDatabase = 0;
                foreach (ZNGame g in gameDatabaseShallower) gamesInDatabase += g.Number;

                int neededMostPlayed = (int)Math.Ceiling(description[step] / Math.E);

                List<ZNPair<int, ZNGame>> mostPlayed = new List<ZNPair<int, ZNGame>>();
                List<ZNPair<int, ZNGame>> bestRated = new List<ZNPair<int, ZNGame>>();
                int numberBound = gamesInDatabase / (description[step] * description[step]);
                int howtimesPlayed;
                ArrayList mostPlayedCount = new ArrayList(new int[neededMostPlayed]);
                ArrayList bestRatedCount = new ArrayList(new int[step - neededMostPlayed]);

                foreach (ZNGame g in gameDatabaseShallower)
                {
                    howtimesPlayed = g.Number;
                    if (howtimesPlayed >= numberBound)
                    {
                        g.Rating = (byte)Math.Round(Math.Sqrt(g.Number / gamesInDatabase) * (g.Halfpoints / (g.Number * 2)) * (g.Halfpoints / (g.Number * 2)) / byte.MaxValue);
                        if (howtimesPlayed > (int)mostPlayedCount[0])
                        {
                            mostPlayed.Add(new ZNPair<int, ZNGame>(howtimesPlayed, g));
                            mostPlayedCount[0] = howtimesPlayed;
                            mostPlayedCount.Sort();
                        }
                        mostPlayed.Sort();
                        // TODO: Sekundarni razeni dle hodnocení
                        mostPlayed.RemoveRange(0, mostPlayed.Count - neededMostPlayed - 1);


                        gameDatabaseShallower.Remove(g);

                        if (g.Rating > (int)bestRatedCount[0])
                        {
                            bestRated.Add(new ZNPair<int, ZNGame>(howtimesPlayed, g));
                            bestRatedCount[0] = howtimesPlayed;
                            bestRatedCount.Sort();
                        }
                        bestRated.Sort();
                        // TODO: Sekundarni razeni dle hodnocení
                        bestRated.RemoveRange(0, mostPlayed.Count - step + neededMostPlayed - 1);
                    }
                    else
                    {
                        gamesForDelete.Add(g);
                    }
                }

                // TODO: Je to opravdu funkcni asyncronni volani?
                IAsyncResult result;
                gameDatabase.RemoveTheseAndTheirChildren(gameDatabaseShallower, out result);
                WriteGamesToFileFixedLengths(writer, gameDatabaseShallower);
                while (!result.IsCompleted)
                {
                    Thread.Sleep(125);
                }

                writer.Close();
            }


            return ZNConstants.EXIT_SUCCESS;
        }

        private void WriteGamesToFileFixedLengths(FileStream writer, ZNGameDatabase gameDatabaseShallower)
        {
            throw new NotImplementedException();
        }

        private bool IsWholePGNGame(string p)
        {
            p = ZNHelper.DeleteWhitespacesFromString(p);
            return p.Contains("1-0") || p.Contains("0-1") || p.Contains("1/2-1/2") || p.Contains("[Result\"*\"]");
        }

        private ZNGame GetGameFromPGN(string pgnData)
        {
            if (pgnData.Contains("[Result\"*\"]"))
            {
                return null;
            }
            StringReader reader = new StringReader(pgnData);
            List<short> moves = new List<short>();
            ZNResult result;
            string shortedPgnData;
            string[] movesPgnCodes;
            StringBuilder builder = new StringBuilder();
            ZNBoard board = new ZNBoard();

            char ch = (char)0;

            while (ch >= 0)
            {
                ch = (char)reader.Read();
                switch (ch)
                {
                    // TODO: Vyresit cisla tahu.
                    case ';':
                        // TODO: A co ; na konci radku?
                        reader.ReadLine();
                        break;
                    case '{':
                        while ((char)reader.Read() != '}') ;
                        break;
                    case '[':
                        while ((char)reader.Read() != ']') ;
                        break;
                    case '(':
                        while ((char)reader.Read() != ')') ;
                        break;
                    case '<':
                        while ((char)reader.Read() != '>') ;
                        break;
                    case '$':
                        while (Char.IsDigit((char)reader.Read())) ;
                        break;
                    case '!':
                        break;
                    case '?':
                        break;
                    case '+':
                        break;
                    case '#':
                        break;
                    case '.':
                        break;
                    default:
                        builder.Append(ch);
                        break;
                }
            }

            reader.Close();
            shortedPgnData = builder.ToString();
            if (shortedPgnData.EndsWith("1-0"))
            {
                result = ZNResult.EWhiteWin;
                shortedPgnData = shortedPgnData.Remove(shortedPgnData.Length - 4);
            }
            else if (shortedPgnData.EndsWith("0-1"))
            {
                result = ZNResult.EBlackWin;
                shortedPgnData = shortedPgnData.Remove(shortedPgnData.Length - 4);
            }
            else if (shortedPgnData.EndsWith("1/2-1/2"))
            {
                result = ZNResult.EDraw;
                shortedPgnData = shortedPgnData.Remove(shortedPgnData.Length - 8);
            }
            else
            {
                return null;
            }
            shortedPgnData = shortedPgnData.ToLowerInvariant();
            builder.Clear();

            movesPgnCodes = shortedPgnData.Split(new char[] { ' ', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            byte newPosition;
            byte oldPosition;
            int isWhite = 1;

            foreach (string move in movesPgnCodes)
            {
                newPosition = 0;
                oldPosition = 0;

                // TODO: refactoring - extract methods and/or use of regions
                // TODO: refactoring - reusing of code
                // TODO: refactoring - all possibility for one piece and in one block of code
                // TODO: better exceptions description
                switch (move.Length)
                {
                    case 2:
                        #region Move of pawn
                        newPosition = (byte)(((byte)(move[0] - 'a') << 2) + (byte)(move[1]));
                        oldPosition = (byte)(newPosition + isWhite * 1);
                        if (board[oldPosition].Type != ZNPieceType.EPawn)
                        {
                            oldPosition += (byte)(isWhite * 1);
                            if (board[oldPosition].Type != ZNPieceType.EPawn)
                            {
                                throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        break;
                    case 3:
                        #region Short castling
                        if (move.Equals("o-o"))
                        {
                            // TODO: Detection of posibility.
                            byte rookOldPosition = 56;
                            byte rookNewPosition = 40;
                            oldPosition = 32;
                            newPosition = 48;
                            if (isWhite == -1)
                            {
                                rookOldPosition += 7;
                                rookNewPosition += 7;
                                oldPosition += 7;
                                newPosition += 7;
                            }
                            board.DoMove((short)(rookOldPosition << 2 + rookNewPosition));
                        }
                        #endregion
                        #region Move of piece
                        else
                        {
                            newPosition = (byte)(((byte)(move[1] - 'a') << 2) + (byte)(move[2]));
                            int found = 0;
                            int foundPosition = 0;
                            int row = newPosition % 8;
                            int rank = newPosition / 8;
                            int count;

                            switch (move[0])
                            {
                                case 'K':
                                    int[] posiblePositionDiffs = { -1, 1, -8, 8, -7, 7, -9, 9 };
                                    foreach (int diff in posiblePositionDiffs)
                                    {
                                        foundPosition = newPosition + diff;
                                        if ((foundPosition > -1) && (foundPosition < 64) &&
                                            (Math.Abs(foundPosition / 8 - rank) < 2) && (Math.Abs(foundPosition % 8 - row) < 2) &&
                                            (board[foundPosition].Type == ZNPieceType.EKing))
                                        {
                                            found++;
                                        }
                                    }
                                    if (found == 1)
                                    {
                                        oldPosition = (byte)foundPosition;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Wrong move!");
                                    }
                                    break;
                                case 'Q':
                                    foundPosition = newPosition + 1;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                    {
                                        foundPosition += 1;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 1;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                    {
                                        foundPosition -= 1;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition + 8;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                    {
                                        foundPosition += 8;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 8;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                    {
                                        foundPosition -= 8;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition + 7;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                    {
                                        foundPosition += 7;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 7;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                    {
                                        foundPosition -= 7;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition + 9;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                    {
                                        foundPosition += 9;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 9;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                    {
                                        foundPosition -= 9;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                    {
                                        found++;
                                    }

                                    if (found == 1)
                                    {
                                        oldPosition = (byte)foundPosition;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Wrong move!");
                                    }
                                    break;
                                case 'N':
                                    int[] posiblePositionDiffs2 = { -10, -6, 6, 10 };
                                    int[] posiblePositionDiffs3 = { -17, -15, 15, 17 };
                                    foreach (int diff in posiblePositionDiffs2)
                                    {
                                        foundPosition = newPosition + diff;
                                        if ((foundPosition > -1) && (foundPosition < 64) &&
                                            (Math.Abs(foundPosition / 8 - rank) == 1) && (Math.Abs(foundPosition % 8 - row) == 2) &&
                                            (board[foundPosition].Type == ZNPieceType.EKnight))
                                        {
                                            found++;
                                        }
                                    }
                                    foreach (int diff in posiblePositionDiffs3)
                                    {
                                        foundPosition = newPosition + diff;
                                        if ((foundPosition > -1) && (foundPosition < 64) &&
                                            (Math.Abs(foundPosition / 8 - rank) == 2) && (Math.Abs(foundPosition % 8 - row) == 1) &&
                                            (board[foundPosition].Type == ZNPieceType.EKnight))
                                        {
                                            found++;
                                        }
                                    }
                                    if (found == 1)
                                    {
                                        oldPosition = (byte)foundPosition;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Wrong move!");
                                    }
                                    break;
                                case 'B':
                                    foundPosition = newPosition + 7;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                    {
                                        foundPosition += 7;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 7;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                    {
                                        foundPosition -= 7;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition + 9;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                    {
                                        foundPosition += 9;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 9;
                                    count = 0;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                        (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                    {
                                        foundPosition -= 9;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                    {
                                        found++;
                                    }

                                    if (found == 1)
                                    {
                                        oldPosition = (byte)foundPosition;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Wrong move!");
                                    }
                                    break;
                                case 'R':
                                    foundPosition = newPosition + 1;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                    {
                                        foundPosition += 1;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 1;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                    {
                                        foundPosition -= 1;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition + 8;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                    {
                                        foundPosition += 8;
                                    }
                                    if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                    {
                                        found++;
                                    }

                                    foundPosition = newPosition - 8;
                                    while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                    {
                                        foundPosition -= 8;
                                    }
                                    if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                    {
                                        found++;
                                    }

                                    if (found == 1)
                                    {
                                        oldPosition = (byte)foundPosition;
                                    }
                                    else
                                    {
                                        throw new ArgumentException("Wrong move!");
                                    }
                                    break;
                                default:
                                    throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        break;
                    case 4:
                        #region Promotion
                        if (move.IndexOf('=') > -1)
                        {
                            newPosition = (byte)(((byte)(move[0] - 'a') << 2) + (byte)(move[1]));
                            oldPosition = (byte)(newPosition + isWhite * 1);
                            if (board[oldPosition].Type != ZNPieceType.EPawn)
                            {
                                oldPosition += (byte)(isWhite * 1);
                                if (board[oldPosition].Type != ZNPieceType.EPawn)
                                {
                                    throw new ArgumentException("Wrong move!");
                                }
                            }

                            ZNPieceType type;
                            switch (move[0])
                            {
                                case 'Q':
                                    type = ZNPieceType.EQueen;
                                    break;
                                case 'R':
                                    type = ZNPieceType.ERook;
                                    break;
                                case 'B':
                                    type = ZNPieceType.EBishop;
                                    break;
                                case 'N':
                                    type = ZNPieceType.EKnight;
                                    break;
                                default:
                                    throw new ArgumentException("Wrong move!");
                            }
                            board.DoPromotion(oldPosition, type);

                            if ((board[oldPosition].Type != type) ||
    (board[newPosition].Type != ZNPieceType.EEmpty))
                            {
                                throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        else if (move.IndexOf('x') > -1)
                        {
                            #region Capture by pawn
                            // TODO: en passant
                            if (move.IndexOf('x') == 1)
                            {
                                newPosition = (byte)((byte)(move[2] - 'a') + (byte)(move[3]) * 2);
                                oldPosition = (byte)((byte)(move[0] - 'a') + isWhite * 1);
                                if ((board[oldPosition].Type != ZNPieceType.EPawn) ||
                                    (board[newPosition].Type == ZNPieceType.EEmpty))
                                {
                                    throw new ArgumentException("Wrong move!");
                                }
                            }
                            #endregion
                            #region Capture by piece
                            else
                            {
                                newPosition = (byte)(((byte)(move[1] - 'a') << 2) + (byte)(move[2]));
                                int found = 0;
                                int foundPosition = 0;
                                int row = newPosition % 8;
                                int rank = newPosition / 8;
                                int count;
                                ZNPieceType type;

                                switch (move[0])
                                {
                                    case 'K':
                                        type = ZNPieceType.EKing;
                                        int[] posiblePositionDiffs = { -1, 1, -8, 8, -7, 7, -9, 9 };
                                        foreach (int diff in posiblePositionDiffs)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) < 2) && (Math.Abs(foundPosition % 8 - row) < 2) &&
                                                (board[foundPosition].Type == ZNPieceType.EKing))
                                            {
                                                found++;
                                            }
                                        }
                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'Q':
                                        type = ZNPieceType.EQueen;
                                        foundPosition = newPosition + 1;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                        {
                                            foundPosition += 1;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 1;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                        {
                                            foundPosition -= 1;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition + 8;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                        {
                                            foundPosition += 8;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 8;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                        {
                                            foundPosition -= 8;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition + 7;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                        {
                                            foundPosition += 7;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 7;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                        {
                                            foundPosition -= 7;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition + 9;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                        {
                                            foundPosition += 9;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 9;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                        {
                                            foundPosition -= 9;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                        {
                                            found++;
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'N':
                                        type = ZNPieceType.EKnight;
                                        int[] posiblePositionDiffs2 = { -10, -6, 6, 10 };
                                        int[] posiblePositionDiffs3 = { -17, -15, 15, 17 };
                                        foreach (int diff in posiblePositionDiffs2)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 1) && (Math.Abs(foundPosition % 8 - row) == 2) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight))
                                            {
                                                found++;
                                            }
                                        }
                                        foreach (int diff in posiblePositionDiffs3)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 2) && (Math.Abs(foundPosition % 8 - row) == 1) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight))
                                            {
                                                found++;
                                            }
                                        }
                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'B':
                                        type = ZNPieceType.EBishop;
                                        foundPosition = newPosition + 7;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                        {
                                            foundPosition += 7;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 7;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                        {
                                            foundPosition -= 7;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition + 9;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                        {
                                            foundPosition += 9;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 9;
                                        count = 0;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                            (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                        {
                                            foundPosition -= 9;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                        {
                                            found++;
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'R':
                                        type = ZNPieceType.ERook;
                                        foundPosition = newPosition + 1;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                        {
                                            foundPosition += 1;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 1;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                        {
                                            foundPosition -= 1;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition + 8;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                        {
                                            foundPosition += 8;
                                        }
                                        if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                        {
                                            found++;
                                        }

                                        foundPosition = newPosition - 8;
                                        while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                        {
                                            foundPosition -= 8;
                                        }
                                        if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                        {
                                            found++;
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    default:
                                        throw new ArgumentException("Wrong move!");
                                }

                                if ((board[oldPosition].Type != type) ||
                                    (board[newPosition].Type == ZNPieceType.EEmpty))
                                {
                                    throw new ArgumentException("Wrong move!");
                                }
                            }
                            #endregion
                        }
                        #region Move of piece with one help
                        else
                        {
                            newPosition = (byte)(((byte)(move[1] - 'a') << 2) + (byte)(move[2]));
                            int found = 0;
                            int foundPosition = 0;
                            int row = newPosition % 8;
                            int rank = newPosition / 8;
                            int count;
                            char known = move[1];
                            ZNPieceType type = ZNPieceType.EEmpty;

                            if ((known <= 'h') && (known >= 'a'))
                            {
                                int knownRank = (int)known - (int)'a';

                                switch (move[0])
                                {
                                    case 'Q':
                                        if (knownRank == rank)
                                        {
                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRank > rank)
                                        {
                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'N':
                                        int[] posiblePositionDiffs2 = { -10, -6, 6, 10 };
                                        int[] posiblePositionDiffs3 = { -17, -15, 15, 17 };
                                        foreach (int diff in posiblePositionDiffs2)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 1) && (Math.Abs(foundPosition % 8 - row) == 2) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition / 8 == knownRank))
                                            {
                                                found++;
                                            }
                                        }
                                        foreach (int diff in posiblePositionDiffs3)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 2) && (Math.Abs(foundPosition % 8 - row) == 1) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition / 8 == knownRank))
                                            {
                                                found++;
                                            }
                                        }
                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'B':
                                        if (knownRank > rank)
                                        {
                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'R':
                                        if (knownRank == rank)
                                        {
                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRank > rank)
                                        {
                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    default:
                                        throw new ArgumentException("Wrong move!");
                                }
                            }
                            else if ((known <= '8') && (known >= '1'))
                            {
                                int knownRow = (int)known;

                                switch (move[0])
                                {
                                    case 'Q':
                                        if (knownRow == row)
                                        {
                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRow > row)
                                        {
                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'N':
                                        int[] posiblePositionDiffs2 = { -10, -6, 6, 10 };
                                        int[] posiblePositionDiffs3 = { -17, -15, 15, 17 };
                                        foreach (int diff in posiblePositionDiffs2)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 1) && (Math.Abs(foundPosition % 8 - row) == 2) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition % 8 == knownRow))
                                            {
                                                found++;
                                            }
                                        }
                                        foreach (int diff in posiblePositionDiffs3)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 2) && (Math.Abs(foundPosition % 8 - row) == 1) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition % 8 == knownRow))
                                            {
                                                found++;
                                            }
                                        }
                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'B':
                                        if (knownRow > row)
                                        {
                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'R':
                                        if (knownRow == row)
                                        {

                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRow > row)
                                        {
                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    default:
                                        throw new ArgumentException("Wrong move!");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Wrong move!");
                            }

                            if ((board[oldPosition].Type != type) ||
                                (board[newPosition].Type != ZNPieceType.EEmpty))
                            {
                                throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        break;
                    case 5:
                        #region Short castling
                        if (move.Equals("o-o-o"))
                        {
                            // TODO: Detection of posibility.
                            byte rookOldPosition = 0;
                            byte rookNewPosition = 24;
                            oldPosition = 32;
                            newPosition = 16;
                            if (isWhite == -1)
                            {
                                rookOldPosition += 7;
                                rookNewPosition += 7;
                                oldPosition += 7;
                                newPosition += 7;
                            }
                            board.DoMove((short)(rookOldPosition << 2 + rookNewPosition));
                        }
                        #endregion
                        #region Capture by piece with one help
                        else if (move.IndexOf('x') > -1)
                        {
                            newPosition = (byte)(((byte)(move[3] - 'a') << 2) + (byte)(move[4]));
                            int found = 0;
                            int foundPosition = 0;
                            int row = newPosition % 8;
                            int rank = newPosition / 8;
                            int count;
                            char known = move[1];
                            ZNPieceType type = ZNPieceType.EEmpty;

                            if ((known <= 'h') && (known >= 'a'))
                            {
                                int knownRank = (int)known - (int)'a';

                                switch (move[0])
                                {
                                    case 'Q':
                                        if (knownRank == rank)
                                        {
                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRank > rank)
                                        {
                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'N':
                                        int[] posiblePositionDiffs2 = { -10, -6, 6, 10 };
                                        int[] posiblePositionDiffs3 = { -17, -15, 15, 17 };
                                        foreach (int diff in posiblePositionDiffs2)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 1) && (Math.Abs(foundPosition % 8 - row) == 2) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition / 8 == knownRank))
                                            {
                                                found++;
                                            }
                                        }
                                        foreach (int diff in posiblePositionDiffs3)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 2) && (Math.Abs(foundPosition % 8 - row) == 1) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition / 8 == knownRank))
                                            {
                                                found++;
                                            }
                                        }
                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'B':
                                        if (knownRank > rank)
                                        {
                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'R':
                                        if (knownRank == rank)
                                        {
                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRank > rank)
                                        {
                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    default:
                                        throw new ArgumentException("Wrong move!");
                                }
                            }
                            else if ((known <= '8') && (known >= '1'))
                            {
                                int knownRow = (int)known;

                                switch (move[0])
                                {
                                    case 'Q':
                                        if (knownRow == row)
                                        {
                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRow > row)
                                        {
                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EQueen))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'N':
                                        int[] posiblePositionDiffs2 = { -10, -6, 6, 10 };
                                        int[] posiblePositionDiffs3 = { -17, -15, 15, 17 };
                                        foreach (int diff in posiblePositionDiffs2)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 1) && (Math.Abs(foundPosition % 8 - row) == 2) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition % 8 == knownRow))
                                            {
                                                found++;
                                            }
                                        }
                                        foreach (int diff in posiblePositionDiffs3)
                                        {
                                            foundPosition = newPosition + diff;
                                            if ((foundPosition > -1) && (foundPosition < 64) &&
                                                (Math.Abs(foundPosition / 8 - rank) == 2) && (Math.Abs(foundPosition % 8 - row) == 1) &&
                                                (board[foundPosition].Type == ZNPieceType.EKnight) && (foundPosition % 8 == knownRow))
                                            {
                                                found++;
                                            }
                                        }
                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'B':
                                        if (knownRow > row)
                                        {
                                            foundPosition = newPosition - 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 7;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 9;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition + 7;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank + (++count)) && (foundPosition < 64))
                                            {
                                                foundPosition += 7;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition - 9;
                                            count = 0;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty &&
                                                (foundPosition / 8 == rank - (++count)) && (foundPosition > -1))
                                            {
                                                foundPosition -= 9;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.EBishop))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    case 'R':
                                        if (knownRow == row)
                                        {

                                            foundPosition = newPosition - 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition > -1))
                                            {
                                                foundPosition -= 8;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }

                                            foundPosition = newPosition + 8;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition < 64))
                                            {
                                                foundPosition += 8;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else if (knownRow > row)
                                        {
                                            foundPosition = newPosition + 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition += 1;
                                            }
                                            if ((foundPosition < 64) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }
                                        else
                                        {
                                            foundPosition = newPosition - 1;
                                            while (board[foundPosition].Type == ZNPieceType.EEmpty && (foundPosition / 8 == rank))
                                            {
                                                foundPosition -= 1;
                                            }
                                            if ((foundPosition > -1) && (board[foundPosition].Type == ZNPieceType.ERook))
                                            {
                                                found++;
                                            }
                                        }

                                        if (found == 1)
                                        {
                                            oldPosition = (byte)foundPosition;
                                        }
                                        else
                                        {
                                            throw new ArgumentException("Wrong move!");
                                        }
                                        break;
                                    default:
                                        throw new ArgumentException("Wrong move!");
                                }
                            }
                            else
                            {
                                throw new ArgumentException("Wrong move!");
                            }

                            if ((board[oldPosition].Type != type) ||
                                (board[newPosition].Type == ZNPieceType.EEmpty))
                            {
                                throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        #region Move by piece with two helps
                        else
                        {
                            newPosition = (byte)(((byte)(move[3] - 'a') << 2) + (byte)(move[4]));
                            oldPosition = (byte)(((byte)(move[1] - 'a') << 2) + (byte)(move[2]));

                            ZNPieceType type;
                            switch (move[0])
                            {
                                case 'Q':
                                    type = ZNPieceType.EQueen;
                                    break;
                                case 'R':
                                    type = ZNPieceType.ERook;
                                    break;
                                case 'B':
                                    type = ZNPieceType.EBishop;
                                    break;
                                case 'N':
                                    type = ZNPieceType.EKnight;
                                    break;
                                default:
                                    throw new ArgumentException("Wrong move!");
                            }

                            if ((board[oldPosition].Type != type) ||
                            (board[newPosition].Type != ZNPieceType.EEmpty))
                            {
                                throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        break;
                    case 6:
                        #region Promotion with capture
                        if (move.IndexOf('=') > -1)
                        {
                            if (move.IndexOf('=') > -1)
                            {
                                newPosition = (byte)(((byte)(move[0] - 'a') << 2) + (byte)(move[1]));
                                oldPosition = (byte)(newPosition + isWhite * 1);
                                if (board[oldPosition].Type != ZNPieceType.EPawn)
                                {
                                    oldPosition += (byte)(isWhite * 1);
                                    if (board[oldPosition].Type != ZNPieceType.EPawn)
                                    {
                                        throw new ArgumentException("Wrong move!");
                                    }
                                }

                                ZNPieceType type;
                                switch (move[0])
                                {
                                    case 'Q':
                                        type = ZNPieceType.EQueen;
                                        break;
                                    case 'R':
                                        type = ZNPieceType.ERook;
                                        break;
                                    case 'B':
                                        type = ZNPieceType.EBishop;
                                        break;
                                    case 'N':
                                        type = ZNPieceType.EKnight;
                                        break;
                                    default:
                                        throw new ArgumentException("Wrong move!");
                                }
                                board.DoPromotion(oldPosition, type);

                                if ((board[oldPosition].Type != type) ||
                                (board[newPosition].Type == ZNPieceType.EEmpty))
                                {
                                    throw new ArgumentException("Wrong move!");
                                }
                            }
                        }
                        #endregion
                        #region Capture by piece with two helps
                        else
                        {
                            newPosition = (byte)(((byte)(move[4] - 'a') << 2) + (byte)(move[5]));
                            oldPosition = (byte)(((byte)(move[1] - 'a') << 2) + (byte)(move[2]));

                            ZNPieceType type;
                            switch (move[0])
                            {
                                case 'Q':
                                    type = ZNPieceType.EQueen;
                                    break;
                                case 'R':
                                    type = ZNPieceType.ERook;
                                    break;
                                case 'B':
                                    type = ZNPieceType.EBishop;
                                    break;
                                case 'N':
                                    type = ZNPieceType.EKnight;
                                    break;
                                default:
                                    throw new ArgumentException("Wrong move!");
                            }

                            if ((board[oldPosition].Type != type) ||
                            (board[newPosition].Type == ZNPieceType.EEmpty))
                            {
                                throw new ArgumentException("Wrong move!");
                            }
                        }
                        #endregion
                        break;
                    default:
                        throw new ArgumentException("Wrong move!");
                }
                board.DoMove((short)(oldPosition << 2 + newPosition));

                isWhite = -isWhite;
            }

            return new ZNGame(moves.ToArray(), result);
        }

    }
}
