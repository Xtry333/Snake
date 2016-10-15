using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testo1
{
    class Program
    {
        static void Main(string[] args)
        {
            string[,] boardTiles = new string[15, 15];
            boardTiles[1, 1] = "A";
            boardTiles[1, 7] = "B";
            boardTiles[7, 1] = "C";
            boardTiles[7, 7] = "D";
            boardTiles[4, 4] = "█";
            //if (boardTiles[1,1] == "A") { Console.WriteLine("True"); }
            for (int i = 0; i < boardTiles.GetLength(1); i++)
            {
                for (int j = 0; j < boardTiles.GetLength(0); j++)
                {
                    if (boardTiles[i, j] == null) { Console.Write(" "); }
                    Console.Write(boardTiles[i, j]);
                }
                Console.Write("\n");
            }
            Console.ReadKey();
        }
    }
}
