using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Snake
{
    class Program
    {
        static string gVersion = "1.2";

        static int boardMinX = 1;
        static int boardMinY = 1;
        static int boardMaxX = 28;
        static int boardMaxY = 28;

        static int boardSizeWidth = boardMaxX - boardMinX + 1;
        static int boardSizeHeight = boardMaxY - boardMinY + 1;
        static string[,] boardTiles = new string[boardMaxX + 2, boardMaxY + 2];

        static string tileFull = "█";
        static string tileHalf = "▒";
        static string tileEmpty = " ";

        static Random rnd = new Random();

        static Stopwatch stopWatch = new Stopwatch();
        static long swTicks = 0;
        static long[] swTicksAvg = new long[50];
        static long swMs = 0;
        static int frameDelay = 66;

        static int fruitX = rnd.Next(boardMinX, boardMaxX);
        static int fruitY = rnd.Next(boardMinY, boardMaxY);

        static bool genExit = false;
        static int genTries = 0;

        static bool snakeAlive = true;

        static int snakeLength = 1;
        static int snakeMaxLength = boardSizeWidth * boardSizeHeight + 1;

        static int[] snakeX = new int[snakeMaxLength];
        static int[] snakeY = new int[snakeMaxLength];

        static int[] snakeOldX = new int[snakeMaxLength];
        static int[] snakeOldY = new int[snakeMaxLength];

        static int snakeDirection = 0;

        static void fBoardClear()
        {
            for (int i = boardMinX; i <= boardMaxX; i++)
            {
                for (int j = boardMinY; j <= boardMaxY; j++)
                {
                    boardTiles[i, j] = tileEmpty;
                    //Console.SetCursorPosition(2*i, j);
                    //Console.Write("..");
                }
            }
            Console.Clear();
        }

        static void drawFrame()
        {
            string fHor = "═";
            string fVer = "║";
            string fDR = "╔";
            string fDL = "╗";
            string fUR = "╚";
            string fUL = "╝";

            writeAt(2 * boardMinX - 1, boardMinY - 1, fDR);
            writeAt(2 * (boardMaxX + 1), boardMinY - 1, fDL);
            writeAt(2 * boardMinX - 1, boardMaxY + 1, fUR);
            writeAt(2 * (boardMaxX + 1), boardMaxY + 1, fUL);

            for (int i = 2 * boardMinX; i < 2 * (boardMaxX + 1); i++)
            {
                writeAt(i, boardMinY - 1, fHor); //Up
                writeAt(i, boardMaxY + 1, fHor); //Down
            }

            for (int i = boardMinY; i < boardMaxY + 1; i++)
            {
                writeAt(2 * boardMinX - 1, i, fVer); //Left
                writeAt(2 * (boardMaxX + 1), i, fVer); //Right
            }
        }

        static bool writeAt(int x, int y, string str, bool addToBoard = false)
        {
            // Summary: 
            //     Sets Cursor at x, y and writes given string
            if (x >= 0 && x <= Console.WindowWidth && y >= 0 && y <= Console.WindowHeight)
            {
                if (addToBoard)
                {
                    if (x >= boardMinX && x <= boardMaxX && y >= boardMinY && y <= boardMaxY)
                    {
                        Console.SetCursorPosition(2 * x, y);
                        Console.Write(str + str);
                        boardTiles[x, y] = str;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(str);
                }
                return true;
            }
            else return false;
            /*try
            {
                if (!(x >= 0 && x <= Console.WindowWidth && y >= 0 && y <= Console.WindowHeight))
                    throw new ArgumentOutOfRangeException("x, y OoR o Console.Dims");
                if (addToBoard)
                {
                    if (x >= boardMinX && x <= boardMaxX && y >= boardMinY && y <= boardMaxY)
                    {
                        Console.SetCursorPosition(2 * x, y);
                        Console.Write(str + str);
                        boardTiles[x, y] = str;
                    }
                }
                else
                {
                    Console.SetCursorPosition(x, y);
                    Console.Write(str);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return false;
            }*/
        }

        static bool isTile(int x, int y, string str)
        {
            if (boardTiles[x, y] == str)
            {
                return true;
            }
            else return false;
        }

        static void snakeRegenFruit(bool info = false)
        {
            while (!genExit)
            {
                genTries++;
                fruitX = rnd.Next(boardMinX, boardMaxX);
                fruitY = rnd.Next(boardMinY, boardMaxY);
                genExit = true;
                for (int i = 0; i <= boardMaxX; i++)
                {
                    for (int j = 0; j <= boardMaxY; j++)
                    {
                        if (boardTiles[i, j] == tileFull && i == fruitX && j == fruitY)
                        {
                            genExit = false;
                        }
                    }
                }
                if (genTries > 1000) { genExit = true; }
            }
            if (info) writeAt(2 * boardMaxX + 8, 6, "Try " + genTries + "   ");
            genExit = false;
            if (genTries > 1000)
            {
                for (int i = 0; i <= boardMaxX; i++)
                {
                    for (int j = 0; j <= boardMaxY; j++)
                    {
                        if (!isTile(i, j, tileFull) && i > boardMinX && j > boardMinY)
                        {
                            genExit = true;
                            fruitX = i;
                            fruitY = j;
                        }
                    }
                }
                if (!genExit)
                {
                    snakeAlive = false;
                    writeAt(2 * boardMaxX + 8, 12, "You're done..");
                    writeAt(2 * boardMaxX + 11, 13, "..there's no more room for fruits..");
                    writeAt(2 * boardMaxX + 14, 14, "..you win..");
                    writeAt(2 * boardMaxX + 17, 15, "..now go get a life..");
                }
            }
            genTries = 0;
        }

        static void Main(string[] args)
        {
            bool quit = false;
            Console.TreatControlCAsInput = true;
            Console.CursorVisible = false;
            Console.Title = "Snake v" + gVersion;
            Console.ForegroundColor = ConsoleColor.White;

            fBoardClear();
            drawFrame();
            
            for (int i = 0; i < snakeMaxLength; i++)
            {
                snakeX[i] = boardSizeWidth / 2;
                snakeY[i] = boardSizeHeight / 2;
            }

            while (!quit)
            {
                stopWatch.Start(); // Frame begin
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                    if ((keyInfo.Key == ConsoleKey.D || keyInfo.Key == ConsoleKey.RightArrow) && snakeDirection != 2) { snakeDirection = 0; }
                    if ((keyInfo.Key == ConsoleKey.S || keyInfo.Key == ConsoleKey.DownArrow) && snakeDirection != 3) { snakeDirection = 1; }
                    if ((keyInfo.Key == ConsoleKey.A || keyInfo.Key == ConsoleKey.LeftArrow) && snakeDirection != 0) { snakeDirection = 2; }
                    if ((keyInfo.Key == ConsoleKey.W || keyInfo.Key == ConsoleKey.UpArrow) && snakeDirection != 1) { snakeDirection = 3; }
                    if (keyInfo.Key == ConsoleKey.C) { snakeLength += 3; }
                    if (keyInfo.Key == ConsoleKey.Escape) { quit = true; }
                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        snakeDirection += 2;
                        snakeX.Reverse();
                        snakeY.Reverse();
                        snakeOldX.Reverse();
                        snakeOldY.Reverse();
                    }
                    if (keyInfo.Key == ConsoleKey.R)
                    {
                        for (int i = 0; i < snakeMaxLength; i++)
                        {
                            snakeX[i] = boardSizeWidth / 2;
                            snakeY[i] = boardSizeHeight / 2;
                            snakeOldX[i] = 0;
                            snakeOldY[i] = 0;
                        }
                        
                        snakeAlive = true;
                        snakeLength = 1;
                        fBoardClear();
                        drawFrame();
                        snakeDirection = 0;
                    }
                }
                
                writeAt(0, 0, ""); //Clears blue bars...                

                if (snakeAlive)
                {
                    if (snakeDirection > 3) { snakeDirection -= 4; }
                    if (snakeDirection < 0) { snakeDirection += 4; }

                    switch (snakeDirection)
                    {
                        case 0: snakeX[0]++; break;
                        case 1: snakeY[0]++; break;
                        case 2: snakeX[0]--; break;
                        case 3: snakeY[0]--; break;
                    }

                    if (snakeX[0] < boardMinX) { snakeX[0] = boardMaxX; }
                    if (snakeX[0] > boardMaxX) { snakeX[0] = boardMinX; }
                    if (snakeY[0] < boardMinY) { snakeY[0] = boardMaxY; }
                    if (snakeY[0] > boardMaxY) { snakeY[0] = boardMinY; }

                    if (isTile(snakeX[0], snakeY[0], tileFull)) { snakeAlive = false; } // Collision
                }
                else
                {
                    writeAt(2 * boardMaxX + 8, 7, "You lose. Press R to restart"); // End
                }

                //if (snakeX[0] == fruitX && snakeY[0] == fruitY)
                if (isTile(snakeX[0], snakeY[0], tileHalf))
                {
                    if (snakeLength < snakeMaxLength) { snakeLength++; }
                    snakeRegenFruit();
                }
                
                if (snakeAlive)
                {
                    for (int i = 1; i <= snakeLength; i++)
                    {
                        snakeX[i] = snakeOldX[i];
                        snakeY[i] = snakeOldY[i];
                        snakeOldX[i] = snakeX[i - 1];
                        snakeOldY[i] = snakeY[i - 1];
                    }
                }
                /*for (int i = 0; i <= snakeLength; i++)
                {
                    if (i != snakeLength) { writeAt(snakeX[i], snakeY[i], tileFull, true); }
                    if (i == snakeLength) { writeAt(snakeX[i], snakeY[i], tileEmpty, true); }
                }*/

                //Console.ForegroundColor = ConsoleColor.DarkGreen;
                writeAt(snakeX[0], snakeY[0], tileFull, true);
                writeAt(snakeX[snakeLength], snakeY[snakeLength], tileEmpty, true);

                if (!isTile(fruitX, fruitY, tileHalf))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    writeAt(fruitX, fruitY, tileHalf, true);
                    Console.ForegroundColor = ConsoleColor.White;
                }

                writeAt(2 * boardMaxX + 8, 2, "Length: " + snakeLength);

                stopWatch.Stop(); // Frame end

                swTicks = stopWatch.ElapsedTicks;
                swMs = stopWatch.ElapsedMilliseconds;
                
                for (int i = 0; i <= swTicksAvg.GetLength(0) - 1; i++)
                {
                    if (i == 0)
                    {
                        swTicksAvg[i] = swTicks;
                    } else
                    {
                        long l = swTicksAvg[i];
                        swTicksAvg[i] = swTicksAvg[i - 1];
                        swTicksAvg[i - 1] = l;
                    }
                }

                writeAt(2 * boardMaxX + 7, 25, "Ticks: " + swTicks + "        ");
                writeAt(2 * boardMaxX + 7, 26, "Avg: " + swTicksAvg.Average() + "        ");
                writeAt(2 * boardMaxX + 7, 27, "Ms: " + swMs + "   ");
                
                stopWatch.Reset(); // Reset stopwatch

                if (frameDelay - Convert.ToInt32(swMs) + 1 > 0)
                    Thread.Sleep(frameDelay - Convert.ToInt32(swMs) + 1);
                else
                    Thread.Sleep(33);
            }
            writeAt(boardMaxX - 5, boardMaxY/2, "Exiting...");
            Thread.Sleep(100);
        }
    }
}