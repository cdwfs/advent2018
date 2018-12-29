using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day25_1 {
    class Program {
        const Int64 MAX_NEIGHBOR_DISTANCE = 3;
        class Coordinate {
            public Coordinate(Int64 x, Int64 y, Int64 z, Int64 w) {
                X = x;
                Y = y;
                Z = z;
                W = w;
                Neighbors = new List<Coordinate>();
            }
            public Int64 X, Y, Z, W;
            public List<Coordinate> Neighbors;
        }
        static Int64 ManhattanDistance(Coordinate c1, Coordinate c2) {
            return Math.Abs(c2.X - c1.X) + Math.Abs(c2.Y - c1.Y) + Math.Abs(c2.Z - c1.Z) + Math.Abs(c2.W - c1.W);
        }
        static string Solve(string inputFilename, bool verbose) {
            var coords = new List<Coordinate>(1400);
            var unclaimedCoords = new HashSet<Coordinate>(coords.Capacity);
            {
                var rx = new Regex(@"^(?<x>\-?\d+),(?<y>\-?\d+),(?<z>\-?\d+),(?<w>\-?\d+)", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                string line;
                while ((line = inStream.ReadLine()) != null) {
                    var m = rx.Match(line);
                    Debug.Assert(m.Success);
                    var c = new Coordinate(
                        Int64.Parse(m.Groups["x"].Value),
                        Int64.Parse(m.Groups["y"].Value),
                        Int64.Parse(m.Groups["w"].Value),
                        Int64.Parse(m.Groups["z"].Value)
                    );
                    coords.Add(c);
                    unclaimedCoords.Add(c);
                }
                inStream.Dispose();

                // populate the Neighbors sets
                for (int i = 0; i < coords.Count; ++i) {
                    for (int j = i + 1; j < coords.Count; ++j) {
                        if (ManhattanDistance(coords[i], coords[j]) <= MAX_NEIGHBOR_DISTANCE) {
                            coords[i].Neighbors.Add(coords[j]);
                            coords[j].Neighbors.Add(coords[i]);
                        }
                    }
                }
            }

            // form constellations
            var constellations = new List<HashSet<Coordinate>>(coords.Count);
            while (unclaimedCoords.Count > 0) {
                var newConst = new HashSet<Coordinate>();
                void ProcessCoord(Coordinate c) {
                    if (newConst.Contains(c)) {
                        return;
                    }
                    newConst.Add(c);
                    unclaimedCoords.Remove(c);
                    for (int i = 0; i < c.Neighbors.Count; ++i) {
                        ProcessCoord(c.Neighbors[i]);
                    }
                }
                Coordinate root = unclaimedCoords.First();
                ProcessCoord(root);
                if (verbose) {
                    Console.WriteLine($"Constellation {constellations.Count} contains:");
                    foreach (var c in newConst) {
                        Console.WriteLine($"- [{c.X,3:D},{c.Y,3:D},{c.Z,3:D},{c.W,3:D}]");
                    }
                }
                constellations.Add(newConst);
            }
            return constellations.Count.ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day25-example0.txt", false) == "2");
            Debug.Assert(Solve(args[0] + @"\day25-example1.txt", false) == "4");
            Debug.Assert(Solve(args[0] + @"\day25-example2.txt", false) == "3");
            Debug.Assert(Solve(args[0] + @"\day25-example3.txt", false) == "8");
            Console.WriteLine(Solve(args[0] + @"\day25-input.txt", false));
        }
    }
}
