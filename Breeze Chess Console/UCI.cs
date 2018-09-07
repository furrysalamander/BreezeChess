using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Breeze_Chess_Console
{
    class UCI
    {
        Board gameBoard;
        int[] bestMove = new int[5];

        public void input(string inString)
        {
            //inString = inString.ToLower();
            string[] inCommand = inString.Split(' ');
            switch (inCommand[0])
            {
                case "uci":
                    Console.WriteLine("id name Breeze Chess");
                    Console.WriteLine("id author Mike");
                    Console.WriteLine("uciok");
                    break;
                case "isready":
                    Console.WriteLine("readyok");
                    break;
                case "position":
                    int f = 0;
                    if (inCommand[1] == "startpos")
                    {
                        gameBoard = BreezeEngine.BoardGen();
                    }
                    else if (inCommand[1] == "fen")
                    {
                        gameBoard = fenToBoard(inCommand[2], inCommand[3], inCommand[4], inCommand[5]);
                        f = 6;
                    }
                    if (inCommand.Count() > 2 + f)
                        if (inCommand[2 + f] == "moves")
                        {
                            for (int i = 3 + f; i < inCommand.Count(); i++)
                            {
                                gameBoard = BreezeEngine.MoveToBoard(coordToMove(inCommand[i]), gameBoard);
                            }
                            // add code for side detection
                            gameBoard.SetDepth(0);
                            /*int[] lastMove = coordToMove(inCommand[inCommand.Count() - 1]);
                                if (sideSet == false)
                                {
                                    if (Math.Sign(gameBoard.Pieces()[lastMove[3], lastMove[2]]) == -1 && gameBoard.IsTurn())
                                    {
                                        gameBoard.ToggleTurn();
                                        sideSet = true;
                                        side = gameBoard.IsTurn();
                                    }
                                }
                                else if (gameBoard.IsTurn() != side) gameBoard.ToggleTurn();*/
                        }
                    break;
                case "go":
                    BreezeEngine.nodes = 0;
                    if (inCommand.Count() > 1) if (inCommand[1] == "depth")
                        {
                            BreezeEngine.toDepth = Convert.ToInt32(inCommand[2]);
                        }
                    Stopwatch timer = new Stopwatch();
                    //BreezeEngine.toDepth = 0;
                    timer.Start();
                    //if (BreezeEngine.toDepth > 2)
                    //    bestMove = BreezeEngine.IterativeDeepen(gameBoard);
                    //else
                        bestMove = BreezeEngine.AlphaBeta(gameBoard, -2147483647, 2147483647, 0);
                    //bestMove = BreezeEngine.ParallelAlphaBeta(gameBoard);
                    // */
                    // bestMove = BreezeEngine.Evaluate(BreezeEngine.CalcFutures(gameBoard), gameBoard);
                    // bestMove = BreezeEngine.ParallelEval(BreezeEngine.CalcFutures(gameBoard), gameBoard);
                    timer.Stop();
                    double nodesPerSec = timer.ElapsedMilliseconds / 1000;
                    nodesPerSec = Math.Round(BreezeEngine.nodes / nodesPerSec, 2);
                    int weight = (bestMove[4] + gameBoard.Weight());
                    if (gameBoard.IsTurn())
                    {
                        weight *= -1;
                    }
                    Console.WriteLine("info nodes " + BreezeEngine.nodes);
                    Console.WriteLine("info depth " + BreezeEngine.toDepth + " score cp " + weight + " nps " + nodesPerSec + " milliseconds " + timer.ElapsedMilliseconds);
                    Console.WriteLine("bestmove " + moveToCoord(bestMove));
                    break;
                case "ucinewgame":
                    gameBoard = BreezeEngine.BoardGen();
                    Console.WriteLine("readyok");
                    break;
                case "debug":
                    Console.WriteLine("Debug, Demo, Bench, or Play?");
                    string debugMode = Console.ReadLine();
                    Console.WriteLine("Loading...");
                    BreezeEngine.GoMode(debugMode, BreezeEngine.BoardGen());
                    break;
            }
        }
        public static int[] coordToMove(string input)
        {
            int[] move = new int[5];
            for (int i = 0; i < 4; i++)
            {
                switch (input[i])
                {
                    case 'a':
                        move[i] = 0;
                        break;
                    case 'b':
                        move[i] = 1;
                        break;
                    case 'c':
                        move[i] = 2;
                        break;
                    case 'd':
                        move[i] = 3;
                        break;
                    case 'e':
                        move[i] = 4;
                        break;
                    case 'f':
                        move[i] = 5;
                        break;
                    case 'g':
                        move[i] = 6;
                        break;
                    case 'h':
                        move[i] = 7;
                        break;
                    case '1':
                        move[i] = 7;
                        break;
                    case '2':
                        move[i] = 6;
                        break;
                    case '3':
                        move[i] = 5;
                        break;
                    case '4':
                        move[i] = 4;
                        break;
                    case '5':
                        move[i] = 3;
                        break;
                    case '6':
                        move[i] = 2;
                        break;
                    case '7':
                        move[i] = 1;
                        break;
                    case '8':
                        move[i] = 0;
                        break;
                }
            }
            return move;
        }
        public static string moveToCoord(int[] input)
        {
            string coord = "";
            for (int i = 0; i < 2; i++)
            {
                switch (input[0 + (2 * i)])
                {
                    case 0:
                        coord += "a";
                        break;
                    case 1:
                        coord += "b";
                        break;
                    case 2:
                        coord += "c";
                        break;
                    case 3:
                        coord += "d";
                        break;
                    case 4:
                        coord += "e";
                        break;
                    case 5:
                        coord += "f";
                        break;
                    case 6:
                        coord += "g";
                        break;
                    case 7:
                        coord += "h";
                        break;
                }
                switch (input[1 + (2 * i)])
                {
                    case 0:
                        coord += 8;
                        break;
                    case 1:
                        coord += 7;
                        break;
                    case 2:
                        coord += 6;
                        break;
                    case 3:
                        coord += 5;
                        break;
                    case 4:
                        coord += 4;
                        break;
                    case 5:
                        coord += 3;
                        break;
                    case 6:
                        coord += 2;
                        break;
                    case 7:
                        coord += 1;
                        break;
                }
            }
            return coord;
        }
        public static Board fenToBoard(string fen, string turn, string castle, string enPassant)
        {
            //string[] fenString = fen.Split('/');
            int x = 0;
            int y = 0;
            int[,] pieces = new int[8, 8];
            foreach (char piece in fen)
                switch (piece)
                {
                    case 'r':
                        pieces[y, x] = BreezeEngine.rook;
                        x++;
                        break;
                    case 'n':
                        pieces[y, x] = BreezeEngine.knight;
                        x++;
                        break;
                    case 'b':
                        pieces[y, x] = BreezeEngine.bishop;
                        x++;
                        break;
                    case 'q':
                        pieces[y, x] = BreezeEngine.queen;
                        x++;
                        break;
                    case 'k':
                        pieces[y, x] = BreezeEngine.king;
                        x++;
                        break;
                    case 'p':
                        pieces[y, x] = BreezeEngine.pawn;
                        x++;
                        break;
                    case 'R':
                        pieces[y, x] = -BreezeEngine.rook;
                        x++;
                        break;
                    case 'N':
                        pieces[y, x] = -BreezeEngine.knight;
                        x++;
                        break;
                    case 'B':
                        pieces[y, x] = -BreezeEngine.bishop;
                        x++;
                        break;
                    case 'Q':
                        pieces[y, x] = -BreezeEngine.queen;
                        x++;
                        break;
                    case 'K':
                        pieces[y, x] = -BreezeEngine.king;
                        x++;
                        break;
                    case 'P':
                        pieces[y, x] = -BreezeEngine.pawn;
                        x++;
                        break;
                    case '/':
                        y++;
                        x = 0;
                        break;
                    default:
                        x += Convert.ToInt32(piece.ToString());
                        break;
                }
            bool Bcastle = false, Wcastle = false;
            if (castle == "")
            {

            }
            else
            {

            }
            bool isTurn = (turn == "w");
            return new Board(pieces, 0, isTurn, Bcastle, Wcastle);
        }
    }
}