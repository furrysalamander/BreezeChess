using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Debugging
using System.Diagnostics;

//Multithreading
using System.Collections.Concurrent;

namespace Breeze_Chess_Console
{
    class BreezeEngine
    {
        // Declares Piece Values
        public const int pawn = 100, knight = 320, bishop = 330, rook = 500, queen = 900, king = 1000000, pawnMove = 2;
        public static int toDepth = 5;
        public static UInt64 nodes = 0;

        public static Board BoardGen()
        {
            // ulong whitePawn = 0L, whiteKnight = 0L, whiteBishop = 0L, whiteRook = 0L, whiteQueen = 0L, whiteKing = 0L, blackPawn = 0L, blackKnight = 0L, blackBishop = 0L, blackRook = 0L, blackQueen = 0L, blackKing = 0L;
            Board chessBoard = new Board(new int[8, 8] {
                { rook, knight, bishop, queen, king, bishop, knight, rook },
                { pawn, pawn, pawn, pawn, pawn, pawn, pawn, pawn },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 0 },
                { -pawn, -pawn, -pawn, -pawn, -pawn, -pawn, -pawn, -pawn },
                { -rook, -knight, -bishop, -queen, -king, -bishop, -knight, -rook }
                }, 0, true, true, true);

            return chessBoard;
            // BreezeBitboard.StringToBitboard(chessBoard, whitePawn, whiteKnight, whiteBishop, whiteRook, whiteQueen, whiteKing, blackPawn, blackKnight, blackBishop, blackRook, blackQueen, blackKing);
        }

        public static void GoMode(string mode, Board chessBoard)
        {
            Stopwatch benchmark = new Stopwatch();
            switch (mode)
            {
                case "debug":
                    benchmark.Start();
                    int[] bestMove = Evaluate(chessBoard);
                    benchmark.Stop();
                    Console.WriteLine(bestMove[0] + " " + bestMove[1] + " " + bestMove[2] + " " + bestMove[3]);
                    Console.WriteLine(benchmark.ElapsedMilliseconds);
                    break;
                case "play":
                    PlayGame(chessBoard, false);
                    break;
                case "bench":
                    benchmark.Start();
                    int times;
                    Console.Write("How many iterations? ");
                    times = Convert.ToInt32(Console.ReadLine());
                    for (int i = 0; i < times; i++)
                    {
                        Evaluate(chessBoard);
                    }
                    benchmark.Stop();
                    Console.WriteLine(benchmark.ElapsedMilliseconds);
                    break;
                case "demo":
                    PlayGame(chessBoard, true);
                    break;
            }
        }

        public static List<int[]> CalcFutures(Board chessBoard)
        {
            List<int[]> futures = new List<int[]>();
            for (int i = 0; i < 64; i++)
            {
                int x = i % 8; int y = i / 8;
                if (chessBoard.IsTurn())
                    switch (chessBoard.Pieces()[i / 8, i % 8])
                    {
                        case -pawn: // Pawn
                                    // Need a way to check for En Passant. 
                            if (x < 7) if (y > 1)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y - 1, x + 1]) == 1)
                                    {
                                        futures.Add(new int[5] { x, y, x + 1, y - 1, chessBoard.Pieces()[y - 1, x + 1] });
                                    }
                                }
                                else
                                {
                                    if (y == 1) if (Math.Sign(chessBoard.Pieces()[y - 1, x + 1]) == 1) futures.Add(new int[5] { x, y, x + 1, y - 1, chessBoard.Pieces()[y - 1, x + 1] + queen });
                                }
                            if (x > 0) if (y > 1)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y - 1, x - 1]) == 1)
                                    {
                                        futures.Add(new int[5] { x, y, x - 1, y - 1, chessBoard.Pieces()[y - 1, x - 1] });
                                    }
                                }
                                else
                                {
                                    if (y == 1) if (Math.Sign(chessBoard.Pieces()[y - 1, x - 1]) == 1) futures.Add(new int[5] { x, y, x - 1, y - 1, chessBoard.Pieces()[y - 1, x - 1] + queen });
                                }
                            if (y == 6 && chessBoard.Pieces()[y - 2, x] == 0 && chessBoard.Pieces()[y - 1, x] == 0)
                            {
                                futures.Add(new int[5] { x, y, x, y - 2, pawnMove });
                            }
                            if (y > 0)
                            {
                                if (chessBoard.Pieces()[y - 1, x] == 0)
                                {
                                    if (y == 1) futures.Add(new int[5] { x, y, x, y - 1, queen });
                                    else futures.Add(new int[5] { x, y, x, y - 1, pawnMove });
                                }
                            }
                            break;
                        case -knight: // Knight
                            if (x > 0 && y > 1) if (Math.Sign(chessBoard.Pieces()[y - 2, x - 1]) != -1) futures.Add(new int[5] { x, y, x - 1, y - 2, chessBoard.Pieces()[y - 2, x - 1] });
                            if (x < 7 && y > 1) if (Math.Sign(chessBoard.Pieces()[y - 2, x + 1]) != -1) futures.Add(new int[5] { x, y, x + 1, y - 2, chessBoard.Pieces()[y - 2, x + 1] });
                            if (x > 0 && y < 6) if (Math.Sign(chessBoard.Pieces()[y + 2, x - 1]) != -1) futures.Add(new int[5] { x, y, x - 1, y + 2, chessBoard.Pieces()[y + 2, x - 1] });
                            if (x < 7 && y < 6) if (Math.Sign(chessBoard.Pieces()[y + 2, x + 1]) != -1) futures.Add(new int[5] { x, y, x + 1, y + 2, chessBoard.Pieces()[y + 2, x + 1] });
                            if (x > 1 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x - 2]) != -1) futures.Add(new int[5] { x, y, x - 2, y - 1, chessBoard.Pieces()[y - 1, x - 2] });
                            if (x > 1 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x - 2]) != -1) futures.Add(new int[5] { x, y, x - 2, y + 1, chessBoard.Pieces()[y + 1, x - 2] });
                            if (x < 6 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x + 2]) != -1) futures.Add(new int[5] { x, y, x + 2, y - 1, chessBoard.Pieces()[y - 1, x + 2] });
                            if (x < 6 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x + 2]) != -1) futures.Add(new int[5] { x, y, x + 2, y + 1, chessBoard.Pieces()[y + 1, x + 2] });
                            break;
                        case -bishop: // Bishop
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y + (q - x);
                                if (s <= 7 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y + (q - x);
                                if (s >= 0 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y - (q - x);
                                if (s >= 0 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y - (q - x);
                                if (s <= 7 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            break;
                        case -rook: // Rook
                            for (int q = x + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == 1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == 1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == 1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == 1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            break;
                        case -queen: // Queen
                            for (int q = x + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == 1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == 1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == 1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == 1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y + (q - x);
                                if (s <= 7 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y + (q - x);
                                if (s >= 0 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y - (q - x);
                                if (s >= 0 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y - (q - x);
                                if (s <= 7 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == 1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            break;
                        case -king: // King
                            if (x > 0 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x - 1]) != -1) futures.Add(new int[5] { x, y, x - 1, y - 1, chessBoard.Pieces()[y - 1, x - 1] });
                            if (y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x]) != -1) futures.Add(new int[5] { x, y, x, y - 1, chessBoard.Pieces()[y - 1, x] });
                            if (x < 7 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x + 1]) != -1) futures.Add(new int[5] { x, y, x + 1, y - 1, chessBoard.Pieces()[y - 1, x + 1] });
                            if (x > 0) if (Math.Sign(chessBoard.Pieces()[y, x - 1]) != -1) futures.Add(new int[5] { x, y, x - 1, y, chessBoard.Pieces()[y, x - 1] });
                            if (x < 7) if (Math.Sign(chessBoard.Pieces()[y, x + 1]) != -1) futures.Add(new int[5] { x, y, x + 1, y, chessBoard.Pieces()[y, x + 1] });
                            if (x > 0 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x - 1]) != -1) futures.Add(new int[5] { x, y, x - 1, y + 1, chessBoard.Pieces()[y + 1, x - 1] });
                            if (y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x]) != -1) futures.Add(new int[5] { x, y, x, y + 1, chessBoard.Pieces()[y + 1, x] });
                            if (x < 7 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x + 1]) != -1) futures.Add(new int[5] { x, y, x + 1, y + 1, chessBoard.Pieces()[y + 1, x + 1] });
                            break;
                    }
                else
                    switch (chessBoard.Pieces()[i / 8, i % 8])
                    {
                        case pawn: // Pawn
                                   // Need a way to check for En Passant. 
                            if (x < 7) if (y < 6)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y + 1, x + 1]) == -1)
                                    {
                                        futures.Add(new int[5] { x, y, x + 1, y + 1, chessBoard.Pieces()[y + 1, x + 1] });
                                    }
                                }
                                else
                                {
                                    if (y == 6) if (Math.Sign(chessBoard.Pieces()[y + 1, x + 1]) == -1) futures.Add(new int[5] { x, y, x + 1, y + 1, chessBoard.Pieces()[y + 1, x + 1] - queen });
                                }
                            if (x > 0) if (y < 6)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y + 1, x - 1]) == -1)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[y + 1, x - 1]) == -1) futures.Add(new int[5] { x, y, x - 1, y + 1, chessBoard.Pieces()[y + 1, x - 1] });
                                    }
                                }
                                else
                                {
                                    if (y == 6) if (Math.Sign(chessBoard.Pieces()[y + 1, x - 1]) == -1) futures.Add(new int[5] { x, y, x - 1, y + 1, chessBoard.Pieces()[y + 1, x - 1] - queen });
                                }
                            if (y == 1 && chessBoard.Pieces()[y + 2, x] == 0 && chessBoard.Pieces()[y + 1, x] == 0)
                            {
                                futures.Add(new int[5] { x, y, x, y + 2, -pawnMove });
                            }
                            if (y < 7)
                            {
                                if (chessBoard.Pieces()[y + 1, x] == 0)
                                {
                                    if (y == 6) futures.Add(new int[5] { x, y, x, y + 1, -queen });
                                    else futures.Add(new int[5] { x, y, x, y + 1, -pawnMove });
                                }
                            }
                            break;
                        case knight: // Knight
                            if (x > 0 && y > 1) if (Math.Sign(chessBoard.Pieces()[y - 2, x - 1]) != 1) futures.Add(new int[5] { x, y, x - 1, y - 2, chessBoard.Pieces()[y - 2, x - 1] });
                            if (x < 7 && y > 1) if (Math.Sign(chessBoard.Pieces()[y - 2, x + 1]) != 1) futures.Add(new int[5] { x, y, x + 1, y - 2, chessBoard.Pieces()[y - 2, x + 1] });
                            if (x > 0 && y < 6) if (Math.Sign(chessBoard.Pieces()[y + 2, x - 1]) != 1) futures.Add(new int[5] { x, y, x - 1, y + 2, chessBoard.Pieces()[y + 2, x - 1] });
                            if (x < 7 && y < 6) if (Math.Sign(chessBoard.Pieces()[y + 2, x + 1]) != 1) futures.Add(new int[5] { x, y, x + 1, y + 2, chessBoard.Pieces()[y + 2, x + 1] });
                            if (x > 1 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x - 2]) != 1) futures.Add(new int[5] { x, y, x - 2, y - 1, chessBoard.Pieces()[y - 1, x - 2] });
                            if (x > 1 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x - 2]) != 1) futures.Add(new int[5] { x, y, x - 2, y + 1, chessBoard.Pieces()[y + 1, x - 2] });
                            if (x < 6 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x + 2]) != 1) futures.Add(new int[5] { x, y, x + 2, y - 1, chessBoard.Pieces()[y - 1, x + 2] });
                            if (x < 6 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x + 2]) != 1) futures.Add(new int[5] { x, y, x + 2, y + 1, chessBoard.Pieces()[y + 1, x + 2] });
                            break;
                        case bishop: // Bishop
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y + (q - x);
                                if (s <= 7 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y + (q - x);
                                if (s >= 0 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y - (q - x);
                                if (s >= 0 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y - (q - x);
                                if (s <= 7 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            break;
                        case rook: // Rook
                            for (int q = x + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == -1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == -1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == -1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == -1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            break;
                        case queen: // Queen
                            for (int q = x + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == -1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y + 1; q <= 7; q++)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == -1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[y, q] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[y, q]) == -1) futures.Add(new int[5] { x, y, q, y, chessBoard.Pieces()[y, q] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, q, y, 0 });
                            }
                            for (int q = y - 1; q >= 0; q--)
                            {
                                if (chessBoard.Pieces()[q, x] != 0)
                                {
                                    if (Math.Sign(chessBoard.Pieces()[q, x]) == -1) futures.Add(new int[5] { x, y, x, q, chessBoard.Pieces()[q, x] });
                                    break;
                                }
                                else futures.Add(new int[5] { x, y, x, q, 0 });
                            }
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y + (q - x);
                                if (s <= 7 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y + (q - x);
                                if (s >= 0 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x + 1; q <= 7; q++)
                            {
                                int s;
                                s = y - (q - x);
                                if (s >= 0 && q <= 7) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            for (int q = x - 1; q >= 0; q--)
                            {
                                int s;
                                s = y - (q - x);
                                if (s <= 7 && q >= 0) if (chessBoard.Pieces()[s, q] != 0)
                                    {
                                        if (Math.Sign(chessBoard.Pieces()[s, q]) == -1)
                                        {
                                            futures.Add(new int[5] { x, y, q, s, chessBoard.Pieces()[s, q] });
                                        }
                                        break;
                                    }
                                    else futures.Add(new int[5] { x, y, q, s, 0 });
                                else break;
                            }
                            break;
                        case king: // King
                            if (x > 0 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x - 1]) != 1) futures.Add(new int[5] { x, y, x - 1, y - 1, chessBoard.Pieces()[y - 1, x - 1] });
                            if (y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x]) != 1) futures.Add(new int[5] { x, y, x, y - 1, chessBoard.Pieces()[y - 1, x] });
                            if (x < 7 && y > 0) if (Math.Sign(chessBoard.Pieces()[y - 1, x + 1]) != 1) futures.Add(new int[5] { x, y, x + 1, y - 1, chessBoard.Pieces()[y - 1, x + 1] });
                            if (x > 0) if (Math.Sign(chessBoard.Pieces()[y, x - 1]) != 1) futures.Add(new int[5] { x, y, x - 1, y, chessBoard.Pieces()[y, x - 1] });
                            if (x < 7) if (Math.Sign(chessBoard.Pieces()[y, x + 1]) != 1) futures.Add(new int[5] { x, y, x + 1, y, chessBoard.Pieces()[y, x + 1] });
                            if (x > 0 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x - 1]) != 1) futures.Add(new int[5] { x, y, x - 1, y + 1, chessBoard.Pieces()[y + 1, x - 1] });
                            if (y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x]) != 1) futures.Add(new int[5] { x, y, x, y + 1, chessBoard.Pieces()[y + 1, x] });
                            if (x < 7 && y < 7) if (Math.Sign(chessBoard.Pieces()[y + 1, x + 1]) != 1) futures.Add(new int[5] { x, y, x + 1, y + 1, chessBoard.Pieces()[y + 1, x + 1] });
                            break;
                    }

            }
            return futures;
        }

        public static int[] Evaluate(Board chessBoard)
        {
            List<int[]> futures = CalcFutures(chessBoard);
            int[] bestMove = new int[5];
            //Random rndMove = new Random();
            //Array.Copy(futures[rndMove.Next(futures.Count)], bestMove, 5);
            if (chessBoard.Depth() == toDepth)
            {
                if (chessBoard.IsTurn())
                {
                    // Maximum
                    bestMove[4] = -2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        if (move[4] > bestMove[4])
                        {
                            Array.Copy(move, bestMove, 5);
                        }
                    }
                }
                else
                {
                    // Minimum
                    bestMove[4] = 2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        if (move[4] < bestMove[4])
                        {
                            Array.Copy(move, bestMove, 5);
                        }
                    }

                }
            }
            else
            {

                if (chessBoard.IsTurn())
                {
                    // Maximum
                    bestMove[4] = -2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) == king) return move;
                        Board newBoard = MoveToBoard(move, chessBoard);
                        move[4] += Evaluate(newBoard)[4];
                        if (move[4] > bestMove[4])
                        {
                            Array.Copy(move, bestMove, 5);
                        }
                    }
                }
                else
                {
                    // Minimum
                    bestMove[4] = 2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) == king) return move;
                        Board newBoard = MoveToBoard(move, chessBoard);
                        move[4] += Evaluate(newBoard)[4];
                        if (move[4] < bestMove[4])
                        {
                            Array.Copy(move, bestMove, 5);
                        }
                    }
                }
            }
            if (chessBoard.Depth() == 1) Console.WriteLine("info nodes " + nodes);
            return bestMove;
        }

        public static int[] ParallelEval(Board chessBoard)
        {
            List<int[]> futures = CalcFutures(chessBoard);
            ConcurrentBag<int[]> possibleBest = new ConcurrentBag<int[]>();
            Parallel.ForEach(futures, move =>
            {
                if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) != king)
                {
                    Board newBoard = MoveToBoard(move, chessBoard);
                    move[4] += Evaluate(newBoard)[4];
                    possibleBest.Add(move);
                }
            });
            int[] bestMove = new int[5];
            // possibleBest.TryPeek(out bestMove);
            if (chessBoard.IsTurn())
            {
                // Maximum
                bestMove[4] = -2147483647;
                foreach (int[] move in possibleBest)
                {
                    if (move[4] > bestMove[4])
                    {
                        Array.Copy(move, bestMove, 5);
                    }
                }
            }
            else
            {
                // Minimum
                bestMove[4] = 2147483647;
                foreach (int[] move in possibleBest)
                {
                    if (move[4] < bestMove[4])
                    {
                        Array.Copy(move, bestMove, 5);
                    }
                }
            }
            return bestMove;
        }

        public static int[] AlphaBeta(Board chessBoard, int alpha, int beta, int inWeight)
        {
            List<int[]> futures = CalcFutures(chessBoard);
            int[] bestMove = new int[5];
            if (chessBoard.Depth() == toDepth)
            {
                if (chessBoard.IsTurn())
                {
                    // Maximum
                    bestMove[4] = -2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        move[4] += inWeight;
                            if (move[4] > bestMove[4])
                        {
                            Array.Copy(move, bestMove, 5);
                        }
                    }
                }
                else
                {
                    // Minimum
                    bestMove[4] = 2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        move[4] += inWeight;
                            if (move[4] < bestMove[4])
                        {
                            Array.Copy(move, bestMove, 5);
                        }
                    }

                }
            }
            else
            {
                if (chessBoard.IsTurn())
                {
                    bestMove[4] = -2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) == king)
                            /*if (chessBoard.Depth() == 1)
                            {
                                move[4] *= 2;
                                return move;
                            }
                            else /* */ return move;
                        move[4] += inWeight;
                        Board newBoard = MoveToBoard(move, chessBoard);
                        move[4] = AlphaBeta(newBoard, alpha, beta, move[4])[4];
                        if (move[4] > bestMove[4])
                            Array.Copy(move, bestMove, 5);
                        if (bestMove[4] > alpha)
                            alpha = bestMove[4];
                        if (beta <= alpha)
                            break;
                    }
                }
                else
                {
                    bestMove[4] = 2147483647;
                    foreach (int[] move in futures)
                    {
                        nodes++;
                        if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) == king)
                            if (chessBoard.Depth() == 1)
                            {
                                move[4] *= 2;
                                return move;
                            }
                            else return move;
                        move[4] += inWeight;
                        Board newBoard = MoveToBoard(move, chessBoard);
                        move[4] = AlphaBeta(newBoard, alpha, beta, move[4])[4];
                        if (move[4] < bestMove[4])
                            Array.Copy(move, bestMove, 5);
                        if (bestMove[4] < beta)
                            beta = bestMove[4];
                        if (beta <= alpha)
                            break;
                    }
                }
            }
            return bestMove;
        }

        public static int[] IterativeDeepen(Board chessBoard)
        {
            List<int[]> futures = CalcFutures(chessBoard);
            int saveDepth = toDepth;
            for (int i = 1; i < saveDepth; i++)
            {
                toDepth = i;
                futures = futures.OrderBy(move => SortVal(MoveToBoard(move, chessBoard))).ToList<int[]>();
                Console.WriteLine("info depth " + toDepth + " currmove " + UCI.moveToCoord(futures[0]) + " score cp " + futures[0][4]);
                Console.WriteLine("info nodes " + nodes);
            }
            toDepth = saveDepth;
            return AlphaBeta(chessBoard, -2147483647, 2147483647, 0);
        }

        private static int SortVal(Board chessBoard)
        {
            List<int[]> futures = CalcFutures(chessBoard);
            int weight = 0;
            chessBoard.SetDepth(0);
            if (chessBoard.IsTurn())
                weight = AlphaBeta(chessBoard, int.MinValue, int.MaxValue, 0)[4];
            else
                weight = -AlphaBeta(chessBoard, int.MinValue, int.MaxValue, 0)[4];
            return weight;
        }

        public static int[] ParallelAlphaBeta(Board chessBoard)
        {
            List<int[]> futures = CalcFutures(chessBoard);
            ConcurrentBag<int[]> possibleBest = new ConcurrentBag<int[]>();
            Parallel.ForEach(futures, move =>
            {
                if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) != king)
                {
                    Board newBoard = MoveToBoard(move, chessBoard);
                    move[4] = AlphaBeta(newBoard, int.MinValue, int.MaxValue, move[4])[4];
                    possibleBest.Add(move);
                }
            });
            int[] bestMove = new int[5];
            // possibleBest.TryPeek(out bestMove);
            if (chessBoard.IsTurn())
            {
                // Maximum
                bestMove[4] = -2147483647;
                foreach (int[] move in possibleBest)
                {
                    if (move[4] > bestMove[4])
                    {
                        Array.Copy(move, bestMove, 5);
                    }
                }
            }
            else
            {
                // Minimum
                bestMove[4] = 2147483647;
                foreach (int[] move in possibleBest)
                {
                    if (move[4] < bestMove[4])
                    {
                        Array.Copy(move, bestMove, 5);
                    }
                }
            }
            return bestMove;
        }

        public static Board MoveToBoard(int[] move, Board board)
        {
            int[,] newBoard = new int[8, 8];
            Array.Copy(board.Pieces(), newBoard, 64);
            newBoard[move[3], move[2]] = newBoard[move[1], move[0]];
            newBoard[move[1], move[0]] = 0;
            if (Math.Abs(newBoard[move[3], move[2]]) == king)
            {
                if (move[0] == 4)
                {
                    if (move[2] == 2)
                    {
                        if (Math.Sign(newBoard[move[3], move[2]]) == 1)
                        {
                            newBoard[0, 3] = rook;
                            newBoard[0, 0] = 0;
                        }
                        else
                        {
                            newBoard[7, 3] = -rook;
                            newBoard[7, 0] = 0;
                        }
                    }
                    if (move[2] == 6)
                    {
                        if (Math.Sign(newBoard[move[3], move[2]]) == 1)
                        {
                            newBoard[0, 5] = rook;
                            newBoard[0, 7] = 0;
                        }
                        else
                        {
                            newBoard[7, 5] = -rook;
                            newBoard[7, 7] = 0;
                        }
                    }
                }
                //Console.WriteLine("break");
            }
            if (newBoard[move[3], move[2]] == pawn && move[3] == 7) newBoard[move[3], move[2]] = queen;
            if (newBoard[move[3], move[2]] == -pawn && move[3] == 0) newBoard[move[3], move[2]] = -queen;
            return new Board(newBoard, board.Depth() + 1, !board.IsTurn(), true, true);
        }

        private static void PlayGame(Board chessBoard, bool demo)
        {
            int[] move = new int[5];
            if (!(chessBoard.IsTurn() || demo))
            {
                Console.WriteLine("Type in your move in coordinate format (a2a4, etc.)");
                move = UCI.coordToMove(Console.ReadLine());
            }
            else
            {
                //Console.WriteLine("Press Enter to Continue");
                //Console.ReadLine();
                Console.WriteLine("Calculating Move..." + chessBoard.IsTurn());
                move = IterativeDeepen(chessBoard);
            }
            if (Math.Abs(chessBoard.Pieces()[move[3], move[2]]) == king)
            {
                if (chessBoard.IsTurn()) Console.WriteLine("White Wins");
                else Console.WriteLine("Black Wins");
                return;
            }
            chessBoard = MoveToBoard(move, chessBoard);
            chessBoard.SetDepth(0);

            Console.Clear();
            Console.WriteLine(move[0] + " " + move[1] + " " + move[2] + " " + move[3] + " " + move[4]);
            //string print = "";
            int count = 0;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  a b c d e f g h");

            for (int i = 0; i < 8; i++)
            {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(8 - i + " ");
                for (int j = 0; j < 8; j++)
                {
                    if ((count % 2) == 0)
                        Console.BackgroundColor = ConsoleColor.DarkMagenta;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkBlue;

                    switch (chessBoard.Pieces()[i, j])
                    {
                        case pawn:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("p");
                            break;
                        case knight:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("n");
                            break;
                        case bishop:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("b");
                            break;
                        case rook:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("r");
                            break;
                        case queen:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("q");
                            break;
                        case king:
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write("k");
                            break;
                        case -pawn:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("P");
                            break;
                        case -knight:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("N");
                            break;
                        case -bishop:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("B");
                            break;
                        case -rook:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("R");
                            break;
                        case -queen:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("Q");
                            break;
                        case -king:
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.Write("K");
                            break;
                        case 0:
                            Console.Write(" ");
                            break;
                    }
                    count++;
                    Console.ResetColor();
                    Console.Write(" ");
                }
                count++;
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(8 - i + " ");
                Console.Write("\n");
            }
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("  a b c d e f g h");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            //Console.Write(print);
            //Console.ReadLine();
            PlayGame(chessBoard, demo);
        }
    }
}