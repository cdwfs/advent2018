using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day18_1 {
    class Program {
        const int TICK_COUNT = 10;
        static int MAP_X_SIZE;
        static int MAP_Y_SIZE;
        static void PrintMap(char[,] map, string label) {
            Console.WriteLine(label);
            for (int y = 0; y < MAP_Y_SIZE; ++y) {
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    Console.Write(map[y, x]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        static string Solve(string inputFilename, bool enableLogs) {
            MAP_X_SIZE = 0;
            MAP_Y_SIZE = 0;
            char[,] mapA, mapB;
            char[,] currentMap, newMap;
            {
                var inStream = new StreamReader(inputFilename);
                string line;
                List<string> lines = new List<string>();
                while ((line = inStream.ReadLine()) != null) {
                    if (MAP_X_SIZE == 0) {
                        MAP_X_SIZE = line.Length;
                    } else {
                        Debug.Assert(line.Length == MAP_X_SIZE, "wrong line size");
                    }
                    lines.Add(line);
                    MAP_Y_SIZE += 1;
                }
                inStream.Dispose();
                mapA = new char[MAP_Y_SIZE, MAP_X_SIZE];
                mapB = new char[MAP_Y_SIZE, MAP_X_SIZE];
                currentMap = mapA;
                newMap = mapB;
                int y = 0;
                foreach(var row in lines) {
                    for (int x = 0; x < MAP_X_SIZE; ++x) {
                        currentMap[y, x] = row[x];
                    }
                    y += 1;
                }
                if (enableLogs) {
                    PrintMap(currentMap, "Initial state:");
                }
            }

            // Simulate
            int elapsedTicks = 0;
            while (elapsedTicks < TICK_COUNT) {
                for (int y = 0; y < MAP_Y_SIZE; ++y) {
                    for (int x = 0; x < MAP_X_SIZE; ++x) {
                        newMap[y, x] = currentMap[y, x]; // TEMP: just copy for now
                    }
                }
                (currentMap, newMap) = (newMap, currentMap);
                elapsedTicks += 1;
                if (enableLogs) {
                    PrintMap(currentMap, $"After {elapsedTicks} minutes:");
                }
            }

            int forestCount = 0, lumberyardCount = 0;
            for (int y = 0; y < MAP_Y_SIZE; ++y) {
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    if (currentMap[y, x] == '|')
                        forestCount += 1;
                    else if (currentMap[y, x] == '#')
                        lumberyardCount += 1;
                }
            }
            return (forestCount*lumberyardCount).ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day18-example0.txt", true) == "1147");
            Console.WriteLine(Solve(args[0] + @"\day18-input.txt", true));
        }
    }
}
