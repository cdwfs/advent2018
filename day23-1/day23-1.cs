using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day23_1 {
    class Program {
        class Nanobot {
            public (Int64 X, Int64 Y, Int64 Z) Pos;
            public Int64 R;
        }
        static Int64 ManhattanDistance((Int64 X, Int64 Y, Int64 Z) p1, (Int64 X, Int64 Y, Int64 Z) p2) {
            return Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y) + Math.Abs(p2.Z - p1.Z);
        }
        static string Solve(string inputFilename, bool enableLogs) {
            var nanobots = new List<Nanobot>(2000);
            {
                var rx = new Regex(@"^pos=\<(?<x>\-?\d+),(?<y>\-?\d+),(?<z>\-?\d+)\>,\s+r=(?<r>\d+)", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                string line;
                while ((line = inStream.ReadLine()) != null) {
                    var m = rx.Match(line);
                    Debug.Assert(m.Success);
                    nanobots.Add(new Nanobot {
                        Pos = (Int64.Parse(m.Groups["x"].Value),
                            Int64.Parse(m.Groups["y"].Value),
                            Int64.Parse(m.Groups["z"].Value)),
                        R = Int64.Parse(m.Groups["r"].Value),
                    });
                }
                inStream.Dispose();
            }

            Int64 maxRadius = Int64.MinValue;
            int minIndex = -1;
            for (int i = 0; i < nanobots.Count; ++i) {
                if (nanobots[i].R > maxRadius) {
                    maxRadius = nanobots[i].R;
                    minIndex = i;
                }
            }
            int numInRange = 0;
            for (int i = 0; i < nanobots.Count; ++i) {
                if (ManhattanDistance(nanobots[i].Pos, nanobots[minIndex].Pos) <= maxRadius)
                    numInRange += 1;
            }
            return numInRange.ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day23-example0.txt", false) == "7");
            Console.WriteLine(Solve(args[0] + @"\day23-input.txt", false));
        }
    }
}
