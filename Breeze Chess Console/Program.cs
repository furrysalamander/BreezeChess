using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;

namespace Breeze_Chess_Console
{
    class Program
    {
        static void Main(string[] args)
        {
            UCI engine = new UCI();
            Console.Title = "Breeze Chess 2016 - Furrysalamander Studios";
            while (true)
            {
                engine.input(Console.ReadLine());
            }
        }
    }
}
