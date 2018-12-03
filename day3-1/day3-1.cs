using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day3_1 {
    class Program {
        static void Main(string[] args) {
            Regex rx = new Regex(@"^#(?<c>\d+) @ (?<x>\d+),(?<y>\d+): (?<w>\d+)x(?<h>\d+)", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var cellCounts = new Dictionary<(int,int), int>();
            int cellsInTwoOrMore = 0;
            while ((line = inStream.ReadLine()) != null) {
                var m = rx.Match(line);
                Debug.Assert(m.Success, "regex did not match line!");
                int c = Int32.Parse(m.Groups["c"].ToString());
                int x = Int32.Parse(m.Groups["x"].ToString());
                int y = Int32.Parse(m.Groups["y"].ToString());
                int w = Int32.Parse(m.Groups["w"].ToString());
                int h = Int32.Parse(m.Groups["h"].ToString());
                //Console.WriteLine($"{i}: {x},{y}: {w}x{h}");
                for (int iy = 0; iy < h; ++iy) {
                    for (int ix = 0; ix < w; ++ix) {
                        var t = (x + ix, y + iy);
                        // TODO(@cort): reduce redundant dict accesses here
                        if (cellCounts.ContainsKey(t)) {
                            int newCount = cellCounts[t] + 1;
                            if (newCount == 2) {
                                cellsInTwoOrMore += 1;
                            }
                            cellCounts[t] = newCount;
                        } else {
                            cellCounts.Add(t, 1);
                        }
                    }
                }
            }

            Console.WriteLine($"{cellsInTwoOrMore} cells in 2+ claims");
        }
    }
}
