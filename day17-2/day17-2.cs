using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day17_2 {
    class Program {
        const int SPRING_X = 500;
        const int SPRING_Y = 0;
        static int MAP_X_SIZE = -1;
        static int MAP_Y_SIZE = -1;
        static int MAP_X_MIN = SPRING_X;
        static int MAP_X_MAX = SPRING_X;
        static int MAP_Y_MIN = SPRING_Y;
        static int MAP_Y_MAX = SPRING_Y;
        static int X_MIN = Int32.MaxValue;
        static int Y_MIN = Int32.MaxValue;
        static int X_MAX = Int32.MinValue;
        static int Y_MAX = Int32.MinValue;
        static int score = 0;
        struct ClayVein {
            public char FixedAxis; // x or y
            public int FixedValue; // fixed value of non-Dir axis
            public int Start, End;  // first and last values along Dir (inclusive)
        }
        static void PrintMap(char[,] map) {
            for (int i = 0; i < 4; ++i) {
                Console.Write("".PadLeft(4) + " ");
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    Console.Write(((MAP_X_MIN + x) / (int)Math.Pow(10, 3 - i)) % 10);
                }
                Console.WriteLine("");
            }
            for (int y = 0; y < MAP_Y_SIZE; ++y) {
                Console.Write((MAP_Y_MIN + y).ToString().PadLeft(4) + " ");
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    Console.Write(map[y, x]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        static void SetCell(int x, int y, char c, char[,] map) {
            Debug.Assert(map[y, x] != '#', "Can't change clay to water");
            int my = y + MAP_Y_MIN;
            int mx = x + MAP_X_MIN;
            if (my >= Y_MIN && my <= Y_MAX && c == '~') {
                score += 1;
            }
            map[y, x] = c;
        }
        static void ProcessRow(int fx, int fy, char[,] map) {
            // scan left
            int lx = fx - 1;
            while (lx >= 0) {
                if (map[fy, lx] == '#') {
                    // hit a clay wall to the left.
                    break;
                } else if (map[fy, lx] == '|') {
                    // hit an existing stream to the left.
                    if (map[fy + 1, lx] == '|' || map[fy + 1, lx] == '.') {
                        break; // join this stream
                    } else if (map[fy + 1, lx] == '#' || map[fy + 1, lx] == '~') {
                        lx -= 1; // plow through it
                    }
                } else if (map[fy, lx] == '.') {
                    // empty space...
                    if (map[fy + 1, lx] == '#' || map[fy + 1, lx] == '~') {
                        // ...with water/clay below it
                        lx -= 1;
                    } else if (map[fy + 1, lx] == '.' || map[fy + 1, lx] == '|') {
                        // ...with nothing below it.
                        break;
                    }
                } else {
                    Debug.Fail("Unhandled case!");
                }
            }
            // scan right
            int rx = fx + 1;
            while (rx < MAP_X_SIZE) {
                if (map[fy, rx] == '#') {
                    // hit a clay wall to the right.
                    break;
                } else if (map[fy, rx] == '|') {
                    // hit an existing stream to the right.
                    if (map[fy + 1, rx] == '|' || map[fy + 1, rx] == '.') {
                        break; // join this stream
                    } else if (map[fy + 1, rx] == '#' || map[fy + 1, rx] == '~') {
                        rx += 1; // plow through it
                    }
                } else if (map[fy, rx] == '.') {
                    // empty space...
                    if (map[fy + 1, rx] == '#' || map[fy + 1, rx] == '~') {
                        // ...with water/clay below it
                        rx += 1;
                    } else if (map[fy + 1, rx] == '.' || map[fy + 1, rx] == '|') {
                        // ...with nothing below it.
                        break;
                    }
                } else {
                    Debug.Fail("Unhandled case!");
                }
            }
            // resolve this row
            if (map[fy, lx] == '#' && map[fy, rx] == '#') {
                // fill row with standing water, decrement fy, and try another row.
                for (int x = lx + 1; x < rx; ++x) {
                    SetCell(x, fy, '~', map);
                }
                ProcessRow(fx, fy - 1, map);
            } else {
                // fill row with flowing water and spawn streams at the end if necessary
                for (int x = lx + 1; x < rx; ++x) {
                    SetCell(x, fy, '|', map);
                }
                bool spawnedStream = false;
                if (map[fy, lx] == '.') {
                    spawnedStream = true;
                    SpawnStream(lx, fy, map);
                }
                if (map[fy, rx] == '.') {
                    spawnedStream = true;
                    SpawnStream(rx, fy, map);
                }
                if (spawnedStream) {
                    // spawning these streams may have filled up the current row, so check again
                    for (; lx >= 0 && map[fy, lx] == '|'; --lx) { }
                    for (; rx < MAP_X_SIZE && map[fy, rx] == '|'; ++rx) { }
                    if (lx >= 0 && map[fy, lx] == '#' && rx < MAP_X_SIZE && map[fy, rx] == '#') {
                        // fill row with standing water, decrement fy, and try another row.
                        for (int x = lx + 1; x < rx; ++x) {
                            SetCell(x, fy, '~', map);
                        }
                        ProcessRow(fx, fy - 1, map);
                    }
                }
            }
        }
        static void SpawnStream(int sx, int sy, char[,] map) {
            Debug.Assert(map[sy, sx] == '.');
            // fall until we hit clay or water
            while (map[sy, sx] == '.') {
                SetCell(sx, sy, '|', map);
                if (++sy >= MAP_Y_SIZE) {
                    return; // this stream falls off the map
                }
                if (map[sy, sx] == '|') {
                    return; // stream falls into an existing river; nothing else to do
                }
            }

            int fy = sy - 1, fx = sx; // fill point
            ProcessRow(fx, fy, map);
        }
        static string Solve(string inputFilename, bool printMap) {
            char[,] map;
            MAP_X_SIZE = -1;
            MAP_Y_SIZE = -1;
            MAP_X_MIN = SPRING_X;
            MAP_X_MAX = SPRING_X;
            MAP_Y_MIN = SPRING_Y;
            MAP_Y_MAX = SPRING_Y;
            X_MIN = Int32.MaxValue;
            Y_MIN = Int32.MaxValue;
            X_MAX = Int32.MinValue;
            Y_MAX = Int32.MinValue;
            score = 0;
            {
                Regex rx = new Regex(@"^(?<dim0>[xy])=(?<v>\d+),\s+(?<dim1>[xy])=(?<start>\d+)\.\.(?<end>\d+)", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                string line;
                var clayVeins = new List<ClayVein>();
                while ((line = inStream.ReadLine()) != null) {
                    var m = rx.Match(line);
                    char dim0 = Char.Parse(m.Groups["dim0"].Value);
                    char dim1 = Char.Parse(m.Groups["dim1"].Value);
                    int v = Int32.Parse(m.Groups["v"].Value);
                    int start = Int32.Parse(m.Groups["start"].Value);
                    int end = Int32.Parse(m.Groups["end"].Value);

                    if (dim0 == 'y') {
                        X_MIN = Math.Min(X_MIN, start);
                        X_MAX = Math.Max(X_MAX, end);
                        Y_MIN = Math.Min(Y_MIN, v);
                        Y_MAX = Math.Max(Y_MAX, v);
                    } else if (dim1 == 'y') {
                        X_MIN = Math.Min(X_MIN, v);
                        X_MAX = Math.Max(X_MAX, v);
                        Y_MIN = Math.Min(Y_MIN, start);
                        Y_MAX = Math.Max(Y_MAX, end);
                    }
                    clayVeins.Add(new ClayVein {
                        FixedAxis = dim0,
                        FixedValue = v,
                        Start = start,
                        End = end,
                    });
                }
                inStream.Dispose();
                // Allocate and populate map
                // broaden map bounds by one square to allow water to flow off the sides & see the bottom
                MAP_X_MIN = X_MIN - 1;
                MAP_X_MAX = X_MAX + 1;
                MAP_Y_MIN = Math.Min(Y_MIN, SPRING_Y);
                MAP_Y_MAX = Math.Max(Y_MAX, Y_MAX + 3);
                MAP_X_SIZE = MAP_X_MAX + 1 - MAP_X_MIN;
                MAP_Y_SIZE = MAP_Y_MAX + 1 - MAP_Y_MIN;
                map = new char[MAP_Y_SIZE, MAP_X_SIZE];
                for (int y = 0; y < MAP_Y_SIZE; ++y) {
                    for (int x = 0; x < MAP_X_SIZE; ++x) {
                        map[y, x] = '.';
                    }
                }
                foreach (var vein in clayVeins) {
                    if (vein.FixedAxis == 'x') {
                        for (int i = vein.Start; i <= vein.End; ++i) {
                            map[i - MAP_Y_MIN, vein.FixedValue - MAP_X_MIN] = '#';
                        }
                    } else if (vein.FixedAxis == 'y') {
                        for (int i = vein.Start; i <= vein.End; ++i) {
                            map[vein.FixedValue - MAP_Y_MIN, i - MAP_X_MIN] = '#';
                        }
                    }
                }
                map[SPRING_Y - MAP_Y_MIN, SPRING_X - MAP_X_MIN] = '+';

                if (printMap) {
                    PrintMap(map);
                }
            }

            SpawnStream(SPRING_X - MAP_X_MIN, SPRING_Y - MAP_Y_MIN + 1, map);
            if (printMap) {
                PrintMap(map);
            }

            return score.ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day17-example0.txt", true) == "29");
            Console.WriteLine(Solve(args[0] + @"\day17-input.txt", false));
        }
    }
}
