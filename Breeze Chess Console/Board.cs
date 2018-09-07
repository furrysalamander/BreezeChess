using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Breeze_Chess_Console
{
    class Board
    {
        bool isTurn;
        bool castling;
        bool enemyCastling;
        int[,] pieces = new int[8, 8];
        int weight = 0;
        int depth;
        public Board(int[,] inBoard, int inDepth, bool inIsTurn, bool inCastling, bool inEnemyCastling)
        {
            Array.Copy(inBoard, pieces, 64);
            isTurn = inIsTurn;
            castling = inCastling;
            enemyCastling = inEnemyCastling;
            depth = inDepth;
        }
        public int Weight()
        {
            int sum = 0;
            foreach (int i in pieces)
                sum += i;
            weight = sum;
            return weight;
        }
        public int[,] Pieces() { return pieces; }
        public bool IsTurn() { return isTurn; }
        public void ToggleTurn() { isTurn = !isTurn; return; }
        public int Depth() { return depth; }
        public void SetDepth(int inDepth)
        {
            depth = inDepth;
        }
    }
}
