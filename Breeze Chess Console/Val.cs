using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze_Chess_Console
{
    class Val
    {
        public static readonly int[] pawn = new int[64]
        {
           900,900,900,900,900,900,900,900,
            50, 50, 50, 50, 50, 50, 50, 50,
            10, 10, 20, 30, 30, 20, 10, 10,
             5,  5, 10, 25, 25, 10,  5,  5,
             0,  0,  0, 20, 20,  0,  0,  0,
             5, -5,-10,  0,  0,-10, -5,  5,
             5, 10, 10,-20,-20, 10, 10,  5,
             0,  0,  0,  0,  0,  0,  0,  0
        };
        public static readonly int[] knight = new int[64]
        {
            -50,-40,-30,-30,-30,-30,-40,-50,
            -40,-20,  0,  0,  0,  0,-20,-40,
            -30,  0, 10, 15, 15, 10,  0,-30,
            -30,  5, 15, 20, 20, 15,  5,-30,
            -30,  0, 15, 20, 20, 15,  0,-30,
            -30,  5, 10, 15, 15, 10,  5,-30,
            -40,-20,  0,  5,  5,  0,-20,-40,
            -50,-40,-30,-30,-30,-30,-40,-50,
        };
        public static readonly int[] bishop = new int[64]
        {
            -20,-10,-10,-10,-10,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5, 10, 10,  5,  0,-10,
            -10,  5,  5, 10, 10,  5,  5,-10,
            -10,  0, 10, 10, 10, 10,  0,-10,
            -10, 10, 10, 10, 10, 10, 10,-10,
            -10,  5,  0,  0,  0,  0,  5,-10,
            -20,-10,-10,-10,-10,-10,-10,-20,
        };
        public static readonly int[] rook = new int[64]
        {
              0,  0,  0,  0,  0,  0,  0,  0,
              5, 10, 10, 10, 10, 10, 10,  5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
             -5,  0,  0,  0,  0,  0,  0, -5,
              0,  0,  0,  5,  5,  0,  0,  0
        };
        public static readonly int[] queen = new int[64]
        {
            -20,-10,-10, -5, -5,-10,-10,-20,
            -10,  0,  0,  0,  0,  0,  0,-10,
            -10,  0,  5,  5,  5,  5,  0,-10,
             -5,  0,  5,  5,  5,  5,  0, -5,
              0,  0,  5,  5,  5,  5,  0, -5,
            -10,  5,  5,  5,  5,  5,  0,-10,
            -10,  0,  5,  0,  0,  0,  0,-10,
            -20,-10,-10, -5, -5,-10,-10,-20
        };
        public static readonly int[] king = new int[64]
        {
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -30,-40,-40,-50,-50,-40,-40,-30,
            -20,-30,-30,-40,-40,-30,-30,-20,
            -10,-20,-20,-20,-20,-20,-20,-10,
             20, 20,  0,  0,  0,  0, 20, 20,
             20, 30, 10,  0,  0, 10, 30, 20
        };
        public static readonly int[] kingEnd = new int[64]
        {
            -50,-40,-30,-20,-20,-30,-40,-50,
            -30,-20,-10,  0,  0,-10,-20,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 30, 40, 40, 30,-10,-30,
            -30,-10, 20, 30, 30, 20,-10,-30,
            -30,-30,  0,  0,  0,  0,-30,-30,
            -50,-30,-30,-30,-30,-30,-30,-50
        };
        public static int PosWeight(int piece, int pos)
        {
            int weight = 0;
            switch (piece)
            {
                case -BreezeEngine.pawn: // Pawn
                    weight += -100 + pawn[pos];
                    break;
                case -BreezeEngine.knight: // Knight
                    weight += -320 + knight[pos];
                    break;
                case -BreezeEngine.bishop: // Bishop
                    weight += -330 + bishop[pos];
                    break;
                case -BreezeEngine.rook: // Rook
                    weight += -500 + rook[pos];
                    break;
                case -BreezeEngine.queen: // Queen
                    weight += -900 + queen[pos];
                    break;
                case -BreezeEngine.king: // King
                    weight += -40000 + king[pos];
                    break;

                case BreezeEngine.pawn: // Pawn
                    weight += 100 - pawn[64-pos];
                    break;
                case BreezeEngine.knight: // Knight
                    weight += 320 - knight[64-pos];
                    break;
                case BreezeEngine.bishop: // Bishop
                    weight += 330 - bishop[64-pos];
                    break;
                case BreezeEngine.rook: // Rook
                    weight += 500 - rook[64-pos];
                    break;
                case BreezeEngine.queen: // Queen
                    weight += 900 - queen[64-pos];
                    break;
                case BreezeEngine.king: // King
                    weight += 40000 - king[64-pos];
                    break;
            }
            return weight;
        }
    }
}
