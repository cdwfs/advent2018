using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day06_1 {
    class Program {
        const int DIM = 360+2;
        //const int DIM = 10 + 2;
        const int TIED = -2;
        const int UNKNOWN = -1;
        static void PrintCells(int[,] cells) {
            return;
            for (int y = 1; y < DIM - 1; ++y) {
                for (int x = 1; x < DIM - 1; ++x) {
                    if (cells[y, x] == TIED) {
                        Console.Write(".");
                    } else if (cells[y, x] == UNKNOWN) {
                        Console.Write("?");
                    } else {
                        Console.Write(cells[y, x]);
                    }
                }
                Console.Write("\n");
            }
            Console.Write("\n");
        }

        static void Main(string[] args) {
            Regex rx = new Regex(@"^(?<x>\d+), (?<y>\d+)", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            int[,] cellsA = new int[DIM, DIM];
            int[,] cellsB = new int[DIM, DIM];
            int[,] oldCells = cellsA;
            int[,] newCells = cellsB;
            for (int y = 0; y < DIM; ++y) {
                for (int x = 0; x < DIM; ++x) {
                    cellsA[y, x] = UNKNOWN;
                    cellsB[y, x] = UNKNOWN;
                }
            }
            int lineCount = 0;
            string[] testLines = {
                "1, 1",
                "1, 6",
                "8, 3",
                "3, 4",
                "5, 5",
                "8, 9",
            };
            while ((line = inStream.ReadLine()) != null) {
            //foreach(var line in testLines) {
                var m = rx.Match(line);
                int x = Int32.Parse(m.Groups["x"].Value);
                int y = Int32.Parse(m.Groups["y"].Value);
                oldCells[y+1, x+1] = lineCount;
                lineCount += 1;
            }
            int[] closestCounts = new int[lineCount];
            for (int i = 0; i < closestCounts.Length; ++i) {
                closestCounts[i] = 1;
            }

            PrintCells(oldCells);

            bool done = false;
            while (!done) {
                bool hasUnknownCells = false;
                for (int y = 1; y < DIM-1; ++y) {
                    for (int x = 1; x < DIM-1; ++x) {
                        // if this cell is already known, preserve its value.
                        if (oldCells[y,x] != UNKNOWN) {
                            newCells[y, x] = oldCells[y, x];
                            continue;
                        }

                        int closest = UNKNOWN;
                        int N = oldCells[y - 1, x];
                        int W = oldCells[y, x - 1];
                        int E = oldCells[y, x + 1];
                        int S = oldCells[y + 1, x];

                        // if any neighbor is TIED, its neighbors are also tied.
                        if (N == TIED || E == TIED || W == TIED || S == TIED) {
                            newCells[y, x] = TIED;
                            continue;
                        }

                        if (N >= 0) {
                            closest = N;
                        }

                        if (W >= 0) {
                            if (closest != UNKNOWN && W != closest) {
                                newCells[y, x] = TIED;
                                continue;
                            }
                            closest = W;
                        }

                        if (E >= 0) {
                            if (closest != UNKNOWN && E != closest) {
                                newCells[y, x] = TIED;
                                continue;
                            }
                            closest = E;
                        }
                        if (S >= 0) {
                            if (closest != UNKNOWN && S != closest) {
                                newCells[y, x] = TIED;
                                continue;
                            }
                            closest = S;
                        }
                        if (closest == UNKNOWN) {
                            newCells[y, x] = UNKNOWN;
                            hasUnknownCells = true;
                        } else {
                            newCells[y, x] = closest;
                            closestCounts[closest] += 1;
                        }
                    }
                }

                PrintCells(newCells);

                var tmp = oldCells;
                oldCells = newCells;
                newCells = tmp;

                if (!hasUnknownCells) {
                    done = true;
                    break;
                }
            }

            PrintCells(oldCells);

            // Figure out which regions are infinite
            bool[] isInfinite = new bool[lineCount];
            for (int i = 0; i < DIM; ++i) {
                if (oldCells[1, i] >= 0) {
                    isInfinite[oldCells[1, i]] = true;
                }
                if (oldCells[DIM - 2, i] >= 0) {
                    isInfinite[oldCells[DIM - 2, i]] = true;
                }
                if (oldCells[i, 1] >= 0) {
                    isInfinite[oldCells[i, 1]] = true;
                }
                if (oldCells[i, DIM - 2] >= 0) {
                    isInfinite[oldCells[i, DIM - 2]] = true;
                }
            }

            for (int i = 0; i < closestCounts.Length; ++i) {
                if (isInfinite[i]) {
                    Console.WriteLine($"{i} is infinite.");
                } else {
                    Console.WriteLine($"{i} is finite & closest to {closestCounts[i]} cells.");
                }
            }
        }
    }
}
