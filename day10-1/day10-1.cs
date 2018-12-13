using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day10_1 {
    class Program {
        struct Point {
            public Int64 x0, y0, dx, dy;
        }
        static void Main(string[] args) {
            Regex rx = new Regex(@"^position=<\s*(?<px>-?\d+),\s+(?<py>-?\d+)> velocity=<\s*(?<vx>-?\d+),\s+(?<vy>-?\d+)>", RegexOptions.Compiled);
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var points = new List<Point>();
            while ((line = inStream.ReadLine()) != null) {
                var match = rx.Match(line);
                Int64 px = Int64.Parse(match.Groups["px"].Value);
                Int64 py = Int64.Parse(match.Groups["py"].Value);
                Int64 vx = Int64.Parse(match.Groups["vx"].Value);
                Int64 vy = Int64.Parse(match.Groups["vy"].Value);
                points.Add(new Point { x0 = px, y0 = py, dx = vx, dy = vy });
            }
            inStream.Dispose();

            for (Int64 step = 10453; step < 10466; ++step) {
                Int64 xMin = Int32.MaxValue, xMax = Int32.MinValue;
                Int64 yMin = Int32.MaxValue, yMax = Int32.MinValue;
                foreach (var pt in points) {
                    Int64 x = pt.x0 + pt.dx * step;
                    Int64 y = pt.y0 + pt.dy * step;
                    xMin = Math.Min(xMin, x);
                    yMin = Math.Min(yMin, y);
                    xMax = Math.Max(xMax, x);
                    yMax = Math.Max(yMax, y);
                }
                Int64 area = (xMax - xMin) * (yMax - yMin);
                int w = (int)(xMax - xMin) + 1;
                int h = (int)(yMax - yMin) + 1;
                Bitmap bmp = new Bitmap(w, h);
                Graphics g = Graphics.FromImage(bmp);
                g.Clear(Color.White);
                foreach (var pt in points) {
                    Int64 x = pt.x0 + pt.dx * step;
                    Int64 y = pt.y0 + pt.dy * step;
                    bmp.SetPixel((int)(x-xMin), (int)(y-yMin), Color.Black);
                }
                bmp.Save($@"..\..\..\inputs\day10-{step}.png");
                Console.WriteLine($"step {step,4:D} area={area,8:D}");
            }

            //Console.WriteLine($"Winner is player {winner} (zero-based) with a score of {maxScore}");
        }
    }
}
