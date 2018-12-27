using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day23_2 {
    class Program {
        struct Int3 {
            public Int3(Int64 x, Int64 y, Int64 z) {
                X = x;
                Y = y;
                Z = z;
            }
            public Int64 X, Y, Z;
        };
        class Nanobot {
            public Int3 Pos;
            public Int64 R;
            public bool IsCubeInRange(Int3 cubeMin, Int64 sideLenMinus1) {
                Int64 distanceToBox = 0;
                Int3 cubeMax = new Int3(
                    cubeMin.X + sideLenMinus1,
                    cubeMin.Y + sideLenMinus1,
                    cubeMin.Z + sideLenMinus1);
                distanceToBox += (cubeMin.X > Pos.X) ? (cubeMin.X - Pos.X) : 0;
                distanceToBox += (cubeMin.Y > Pos.Y) ? (cubeMin.Y - Pos.Y) : 0;
                distanceToBox += (cubeMin.Z > Pos.Z) ? (cubeMin.Z - Pos.Z) : 0;
                distanceToBox += (cubeMax.X < Pos.X) ? (Pos.X - cubeMax.X) : 0;
                distanceToBox += (cubeMax.Y < Pos.Y) ? (Pos.Y - cubeMax.Y) : 0;
                distanceToBox += (cubeMax.Z < Pos.Z) ? (Pos.Z - cubeMax.Z) : 0;
                return (distanceToBox <= R);
            }
        }
        static Int64 ManhattanDistance(Int3 p1, Int3 p2) {
            return Math.Abs(p2.X - p1.X) + Math.Abs(p2.Y - p1.Y) + Math.Abs(p2.Z - p1.Z);
        }

        class SearchWorld {
            public List<Nanobot> nanobots;
            public Int3 origin;
            public bool checkDistance;
            public int bestInRangeCount;
            public Int3 bestCoords;
            public Int64 bestDistanceToOrigin;
        }
        static void FindMaxCell(SearchWorld world, List<int> searchBotIndices, Int3 searchMin, Int64 searchDim, bool verbose) {
            if (verbose) {
                Console.WriteLine($"searching for {searchBotIndices.Count} bots in {searchDim}x{searchDim}x{searchDim} at [{searchMin.X},{searchMin.Y},{searchMin.Z}]...");
            }
            Debug.Assert(searchBotIndices.Count <= world.nanobots.Count);
            const int GRID_DIM = 2;
            var cellResults = new (Int3 GlobalCoords, List<int> BotIndices)[GRID_DIM * GRID_DIM * GRID_DIM];
            var cellBotCounts = new int[cellResults.Length];
            Debug.Assert((searchDim % GRID_DIM) == 0);
            Int64 cellDim = (searchDim / GRID_DIM);
            int cellIndex = 0;
            for (Int64 z = 0; z < GRID_DIM; ++z) {
                Int64 cellMinZ = searchMin.Z + (z * cellDim);
                for (Int64 y = 0; y < GRID_DIM; ++y) {
                    Int64 cellMinY = searchMin.Y + (y * cellDim);
                    for (Int64 x = 0; x < GRID_DIM; ++x) {
                        Int64 cellMinX = searchMin.X + (x * cellDim);
                        Int3 cellMin = new Int3(cellMinX, cellMinY, cellMinZ);
                        cellResults[cellIndex].GlobalCoords = cellMin;
                        cellResults[cellIndex].BotIndices = new List<int>(1);
                        foreach(int bi in searchBotIndices) {
                            if (bi == 3) {
                                Console.Write("");
                            }
                            if (world.nanobots[bi].IsCubeInRange(cellMin, cellDim - 1)) {
                                cellResults[cellIndex].BotIndices.Add(bi);
                            }
                        }
                        Debug.Assert(cellResults[cellIndex].BotIndices.Count <= searchBotIndices.Count);
                        // Negate cell count so we get results sorted in descending order
                        cellBotCounts[cellIndex] = -cellResults[cellIndex].BotIndices.Count;
                        cellIndex++;
                    }
                }
            }
            Array.Sort(cellBotCounts, cellResults);
            if (verbose) {
                foreach (var c in cellResults) {
                    Console.WriteLine($"[{c.GlobalCoords.X},{c.GlobalCoords.Y},{c.GlobalCoords.Z}]: {c.BotIndices.Count} bots in range");
                }
            }

            if (cellDim == 1) {
                // Find the cell with count == maxCount that's closest to the origin. Potentially
                // update the global best.
                for (int i = 0; i < cellResults.Length; ++i) {
                    if (cellResults[i].BotIndices.Count < world.bestInRangeCount) {
                        return; // nothing else in the list will be better than the global best.
                    }
                    Int64 distanceToOrigin = ManhattanDistance(cellResults[i].GlobalCoords, world.origin);
                    if (cellResults[i].BotIndices.Count > world.bestInRangeCount) {
                        world.bestInRangeCount = cellResults[i].BotIndices.Count;
                        world.bestCoords = cellResults[i].GlobalCoords;
                        world.bestDistanceToOrigin = distanceToOrigin;
                        if (true || verbose) {
                            Console.WriteLine($"current best guess: [{world.bestCoords.X},{world.bestCoords.Y},{world.bestCoords.Z}], {world.bestDistanceToOrigin} to origin, {world.bestInRangeCount} bots in range");
                        }
                    } else if (cellResults[i].BotIndices.Count == world.bestInRangeCount) {
                        if (!world.checkDistance) {
                            return;
                        }
                        // same in-range count, but may be closer to the origin
                        if (distanceToOrigin < world.bestDistanceToOrigin) {
                            world.bestCoords = cellResults[i].GlobalCoords;
                            world.bestDistanceToOrigin = distanceToOrigin;
                            if (true || verbose) {
                                Console.WriteLine($"current best guess: [{world.bestCoords.X},{world.bestCoords.Y},{world.bestCoords.Z}], {world.bestDistanceToOrigin} to origin, {world.bestInRangeCount} bots in range");
                            }
                        }
                    }
                }
            } else {
                // recurse on all cells whose upper-bound in-range count is better than the current best
                for (int i = 0; i < cellResults.Length; ++i) {
                    if (cellResults[i].BotIndices.Count < world.bestInRangeCount) {
                        return; // nothing else in the list will be better than the global best.
                    } else if (cellResults[i].BotIndices.Count == world.bestInRangeCount) {
                        if (!world.checkDistance) {
                            return; // don't care about distance yet, just the count.
                        }
                        // if the corners of this cell are all further from the origin than the current closest,
                        // there's no need to check its contents for a closer point.
                        var cubeMin = cellResults[i].GlobalCoords;
                        Int64 sideLenMinus1 = cellDim - 1;
                        var cubeCorners = new Int3[8]{
                            new Int3(cubeMin.X, cubeMin.Y, cubeMin.Z),
                            new Int3(cubeMin.X+sideLenMinus1, cubeMin.Y, cubeMin.Z),
                            new Int3(cubeMin.X, cubeMin.Y+sideLenMinus1, cubeMin.Z),
                            new Int3(cubeMin.X+sideLenMinus1, cubeMin.Y+sideLenMinus1, cubeMin.Z),
                            new Int3(cubeMin.X, cubeMin.Y, cubeMin.Z+sideLenMinus1),
                            new Int3(cubeMin.X+sideLenMinus1, cubeMin.Y, cubeMin.Z+sideLenMinus1),
                            new Int3(cubeMin.X, cubeMin.Y+sideLenMinus1, cubeMin.Z+sideLenMinus1),
                            new Int3(cubeMin.X+sideLenMinus1, cubeMin.Y+sideLenMinus1, cubeMin.Z+sideLenMinus1),
                        };
                        if (!cubeCorners.Any(c => (ManhattanDistance(c, world.origin) < world.bestDistanceToOrigin))) {
                            return;
                        }
                    }
                    FindMaxCell(world, cellResults[i].BotIndices, cellResults[i].GlobalCoords, searchDim / GRID_DIM, verbose);
                }
            }
        }
        static string Solve(string inputFilename, bool verbose) {
            var nanobots = new List<Nanobot>(1100);
            {
                var rx = new Regex(@"^pos=\<(?<x>\-?\d+),(?<y>\-?\d+),(?<z>\-?\d+)\>,\s+r=(?<r>\d+)", RegexOptions.Compiled);
                var inStream = new StreamReader(inputFilename);
                string line;
                while ((line = inStream.ReadLine()) != null) {
                    var m = rx.Match(line);
                    Debug.Assert(m.Success);
                    nanobots.Add(new Nanobot {
                        Pos = new Int3(Int64.Parse(m.Groups["x"].Value),
                                       Int64.Parse(m.Groups["y"].Value),
                                       Int64.Parse(m.Groups["z"].Value)),
                        R = Int64.Parse(m.Groups["r"].Value),
                    });
                }
                inStream.Dispose();
            }

            Int64 minX = Int64.MaxValue, minY = Int64.MaxValue, minZ = Int64.MaxValue, minR = Int64.MaxValue;
            Int64 maxX = Int64.MinValue, maxY = Int64.MinValue, maxZ = Int64.MinValue, maxR = Int64.MinValue;
            for (int i = 0; i < nanobots.Count; ++i) {
                minX = Math.Min(minX, nanobots[i].Pos.X);
                minY = Math.Min(minY, nanobots[i].Pos.Y);
                minZ = Math.Min(minZ, nanobots[i].Pos.Z);
                minR = Math.Min(minR, nanobots[i].R);
                maxX = Math.Max(maxX, nanobots[i].Pos.X);
                maxY = Math.Max(maxY, nanobots[i].Pos.Y);
                maxZ = Math.Max(maxZ, nanobots[i].Pos.Z);
                maxR = Math.Max(maxR, nanobots[i].R);
            }
            // bias all the coordinates
            var allBotIndices = new List<int>(nanobots.Count);
            for (int i = 0; i < nanobots.Count; ++i) {
                nanobots[i].Pos.X -= minX;
                nanobots[i].Pos.Y -= minY;
                nanobots[i].Pos.Z -= minZ;
                allBotIndices.Add(i);
            }
            if (verbose) {
                Console.WriteLine($"global offset is [{minX}, {minY}, {minZ}]");
            }
            // Round up the world bounds to the next power of 2
            Int64 maxDim = Math.Max(maxX - minX, Math.Max(maxY - minY, maxZ - minZ));
            Int64 worldDim = maxDim - 1;
            worldDim |= (worldDim >> 1);
            worldDim |= (worldDim >> 2);
            worldDim |= (worldDim >> 4);
            worldDim |= (worldDim >> 6);
            worldDim |= (worldDim >> 8);
            worldDim |= (worldDim >> 16);
            worldDim |= (worldDim >> 32);
            worldDim += 1;

            Int3 searchMin = new Int3(0, 0, 0);
            var world = new SearchWorld {
                nanobots = nanobots,
                origin = new Int3(-minX, -minY, -minZ),
                bestDistanceToOrigin = Int64.MaxValue,
                bestCoords = new Int3(Int64.MaxValue, Int64.MaxValue, Int64.MaxValue),
                bestInRangeCount = 0,
            };
            // Find once to determine the max in range count
            world.checkDistance = false;
            FindMaxCell(world, allBotIndices, searchMin, worldDim, verbose);
            int count = 0;
            foreach (var bot in nanobots) {
                if (bot.IsCubeInRange(world.bestCoords, 0)) {
                    ++count;
                }
            }
            Debug.Assert(count == world.bestInRangeCount, "Inconsistent count after first pass");
            // check again to determine the closest cell with that count
            world.checkDistance = true;
            FindMaxCell(world, allBotIndices, searchMin, worldDim, verbose);
            // Validate
            Debug.Assert(ManhattanDistance(world.bestCoords, world.origin) == world.bestDistanceToOrigin, "Inconsistent best distance");
            count = 0;
            foreach (var bot in nanobots) {
                if (bot.IsCubeInRange(world.bestCoords, 0)) {
                    ++count;
                }
            }
            Debug.Assert(count == world.bestInRangeCount, "Inconsistent count after second pass");
            
            return world.bestDistanceToOrigin.ToString();
        }
        static void Main(string[] args) {
            Debug.Assert(Solve(args[0] + @"\day23-example1.txt", false  ) == "36");
            Console.WriteLine(Solve(args[0] + @"\day23-input.txt", false));
        }
    }
}
