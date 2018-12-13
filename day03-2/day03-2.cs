using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace day03_2 {
    class Program {
        static void Main(string[] args) {
            Regex rx = new Regex(@"^#(?<c>\d+) @ (?<x>\d+),(?<y>\d+): (?<w>\d+)x(?<h>\d+)", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var cellCounts = new Dictionary<(int, int), (int, int)>();
            var nonOverlappingClaims = new HashSet<int>();
            while ((line = inStream.ReadLine()) != null) {
                var m = rx.Match(line);
                Debug.Assert(m.Success, "regex did not match line!");
                int c = Int32.Parse(m.Groups["c"].Value);
                int x = Int32.Parse(m.Groups["x"].Value);
                int y = Int32.Parse(m.Groups["y"].Value);
                int w = Int32.Parse(m.Groups["w"].Value);
                int h = Int32.Parse(m.Groups["h"].Value);
                bool hasOverlap = false;
                for (int iy = 0; iy < h; ++iy) {
                    for (int ix = 0; ix < w; ++ix) {
                        var t = (x + ix, y + iy);
                        // TODO(@cort): reduce redundant dict accesses here
                        if (cellCounts.ContainsKey(t)) {
                            hasOverlap = true;
                            nonOverlappingClaims.Remove(cellCounts[t].Item2);
                            int newCount = cellCounts[t].Item1 + 1;
                            cellCounts[t] = (newCount, c);
                        } else {
                            cellCounts.Add(t, (1,c));
                        }
                    }
                }
                if (!hasOverlap) {
                    nonOverlappingClaims.Add(c);
                }
            }
            inStream.Dispose();

            foreach (var c in nonOverlappingClaims) {
                Console.WriteLine($"claim {c} does not overlap");
            }
        }
    }
}
