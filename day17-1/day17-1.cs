using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day17_1 {
    class Program {
        const int SPRING_X = 500;
        const int SPRING_Y = 0;
        static int MAP_X_SIZE = -1;
        static int MAP_Y_SIZE = -1;
        static int MAP_X_MIN = SPRING_X;
        static int MAP_X_MAX = SPRING_X;
        static int MAP_Y_MIN = SPRING_Y;
        static int MAP_Y_MAX = SPRING_Y;
        struct ClayVein {
            public char FixedAxis; // x or y
            public int FixedValue; // fixed value of non-Dir axis
            public int Start, End;  // first and last values along Dir (inclusive)
        }
        static void PrintMap(char[,] map) {
            for (int i = 0; i < 3; ++i) {
                Console.Write("".PadLeft(3) + " ");
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    Console.Write(((MAP_X_MIN + x) / (int)Math.Pow(10, 2-i)) % 10);
                }
                Console.WriteLine("");
            }
            for (int y = 0; y < MAP_Y_SIZE; ++y) {
                Console.Write((MAP_Y_MIN + y).ToString().PadLeft(3) + " ");
                for (int x = 0; x < MAP_X_SIZE; ++x) {
                    Console.Write(map[y, x]);
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        static string Solve(string inputFilename) {
            int X_MIN = Int32.MaxValue;
            int Y_MIN = Int32.MaxValue;
            int X_MAX = Int32.MinValue;
            int Y_MAX = Int32.MinValue;
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
                        End  = end,
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
                char[,] map = new char[MAP_Y_SIZE, MAP_X_SIZE];
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

                PrintMap(map);
            }
            return "TBD";
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(@"..\..\..\inputs\day17-example0.txt") =="57");
            Console.WriteLine(Solve(args[0]));
        }
    }
}
