using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day18_2 {
    class Program {
        const Int64 TICK_COUNT = 1000;
        const Int64 TICK_TARGET = 1000000000;
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
        static int CountAdjacent(int x, int y, char t, char[,] map) {
            int count = 0;
            for (int ay = y - 1; ay <= y + 1; ++ay) {
                if (ay < 0 || ay >= MAP_Y_SIZE)
                    continue;
                for (int ax = x - 1; ax <= x + 1; ++ax) {
                    if (ax == x && ay == y)
                        continue;
                    if (ax < 0 || ax >= MAP_X_SIZE)
                        continue;
                    if (map[ay, ax] == t)
                        count += 1;
                }
            }
            return count;
        }
        static int MapHash(char[,] map) {
            char[] flattened = new char[MAP_Y_SIZE * MAP_X_SIZE];
            for (int y = 0; y < MAP_Y_SIZE; ++y) {
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    flattened[y * MAP_X_SIZE + x] = map[y, x];
                }
            }
            return new string(flattened).GetHashCode();
        }
        static int MapValue(char[,] map) {
            int forestCount = 0, lumberyardCount = 0;
            for (int y = 0; y < MAP_Y_SIZE; ++y) {
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    if (map[y, x] == '|')
                        forestCount += 1;
                    else if (map[y, x] == '#')
                        lumberyardCount += 1;
                }
            }
            return forestCount * lumberyardCount;
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
                foreach (var row in lines) {
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
            Int64 elapsedTicks = 0;
            var states = new Dictionary<int, (Int64 ticks, int value)>();
            states[MapHash(currentMap)] = (0, MapValue(currentMap));
            var tickValues = new Dictionary<Int64, int>();
            tickValues[elapsedTicks] = MapValue(currentMap);
            Int64 cycleLength = 0;
            while (elapsedTicks < TICK_COUNT) {
                for (int y = 0; y < MAP_Y_SIZE; ++y) {
                    for (int x = 0; x < MAP_X_SIZE; ++x) {
                        if (currentMap[y, x] == '.') {
                            newMap[y, x] = CountAdjacent(x, y, '|', currentMap) >= 3 ? '|' : '.';
                        } else if (currentMap[y, x] == '|') {
                            newMap[y, x] = CountAdjacent(x, y, '#', currentMap) >= 3 ? '#' : '|';
                        } else if (currentMap[y, x] == '#') {
                            newMap[y, x] = (CountAdjacent(x, y, '#', currentMap) >= 1 && CountAdjacent(x, y, '|', currentMap) >= 1) ? '#' : '.';
                        }
                    }
                }
                (currentMap, newMap) = (newMap, currentMap);
                elapsedTicks += 1;
                if (enableLogs) {
                    PrintMap(currentMap, $"After {elapsedTicks} minutes:");
                }

                // TIL: hash of an array does not take into account array contents.
                // TIL: char[].ToString() does not do what you think it does -- you get a name of the variable, not a string of its contents.
                //      use new string(char[]) instead.
                int value = MapValue(currentMap);
                tickValues[elapsedTicks] = value;
                int currentHash = MapHash(currentMap);
                if (states.ContainsKey(currentHash)) {
                    var oldState = states[currentHash];
                    Debug.Assert(oldState.value == value);
                    Console.WriteLine($"Cycle detected: turn {oldState.ticks} = turn {elapsedTicks} = value {oldState.value} (delta {elapsedTicks - oldState.ticks})");
                    cycleLength = elapsedTicks - oldState.ticks;
                    break;
                } else {
                    states[currentHash] = (elapsedTicks, value);
                }
            }
            Debug.Assert(cycleLength > 0, $"No cycle detected after {TICK_COUNT} ticks -- try more?");
            Int64 cycleStart = elapsedTicks - cycleLength;
            Int64 targetOffset = (TICK_TARGET - cycleStart) % cycleLength;
            return tickValues[cycleStart + targetOffset].ToString();
        }
        static void Main(string[] args) {
            Console.WriteLine(Solve(args[0] + @"\day18-input.txt", false));
        }
    }
}
