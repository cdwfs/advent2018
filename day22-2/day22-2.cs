using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day22_2 {
    class Program {
        const int MAP_SIZE_X = 1000;
        const int MAP_SIZE_Y = 1000;
        const int START_X = 0;
        const int START_Y = 0;
        const int START_TOOL = TOOL_TORCH;
        const int COST_TOOL = 7;
        const int COST_MOVE = 1;
        struct Region {
            public Int64 GeologicIndex;
            public Int64 ErosionLevel;
            public char Type;
        };
        struct MazeCell {
            public bool Wall;
            public int DistanceTo;
            public int DistanceFromGuess;
            public bool Complete;
            public int DistanceTotal { get => DistanceTo + DistanceFromGuess; }
            public bool Visited { get => DistanceTo < Int32.MaxValue; }
        };
        const char ROCKY = '.';
        const char WET = '=';
        const char NARROW = '|';
        const int TOOL_NONE = 0;
        const int TOOL_TORCH = 1;
        const int TOOL_GEAR = 2;
        static int DistanceHeuristic((int x, int y, int t) current, (int x, int y, int t) target) {
            return Math.Abs(target.y - current.y) + Math.Abs(target.x - current.x) + (target.t == current.t ? 0 : 7);
        }
        static string Solve(int depth, (int x, int y, int t) target, bool enableLogs) {
            var geoMap = new Region[MAP_SIZE_Y, MAP_SIZE_X];
            MazeCell[,,] maze = new MazeCell[3, MAP_SIZE_Y, MAP_SIZE_X];
            for (int t = 0; t < 3; ++t) {
                for (int y = 0; y < MAP_SIZE_Y; ++y) {
                    for (int x = 0; x < MAP_SIZE_X; ++x) {
                        maze[t, y, x].Wall = false;
                        maze[t, y, x].Complete = false;
                        maze[t, y, x].DistanceTo = Int32.MaxValue;
                        maze[t, y, x].DistanceFromGuess = DistanceHeuristic((x, y, t), target);
                    }
                }
            }
            for (int y = 0; y < MAP_SIZE_Y; ++y) {
                for (int x = 0; x < MAP_SIZE_X; ++x) {
                    Int64 geologicIndex;
                    if ((x, y) == (0, 0)) { // TIL: tuple equality requires C# 7.3
                        geologicIndex = 0;
                    } else if ((x, y) == (target.x, target.y)) {
                        geologicIndex = 0;
                    } else if (y == 0) {
                        geologicIndex = x * 16807;
                    } else if (x == 0) {
                        geologicIndex = y * 48271;
                    } else {
                        geologicIndex = geoMap[y, x - 1].ErosionLevel * geoMap[y - 1, x].ErosionLevel;
                    }
                    Int64 erosionLevel = (geologicIndex + depth) % 20183;
                    Int64 riskLevel = erosionLevel % 3;
                    char type;
                    if (riskLevel == 0) {
                        type = ROCKY;
                    } else if (riskLevel == 1) {
                        type = WET;
                    } else {
                        type = NARROW;
                    }
                    geoMap[y, x] = new Region {
                        GeologicIndex = geologicIndex,
                        ErosionLevel = erosionLevel,
                        Type = type,
                    };
                    if (type == ROCKY) {
                        maze[TOOL_NONE, y, x].Wall = true;
                    } else if (type == WET) {
                        maze[TOOL_TORCH, y, x].Wall = true;
                    } else if (type == NARROW) {
                        maze[TOOL_GEAR, y, x].Wall = true;
                    }
                }
            }

            // Path the find!
            (int x, int y, int t) start = (START_X, START_Y, START_TOOL);
            Debug.Assert(!maze[start.t, start.y, start.x].Wall);
            Debug.Assert(!maze[target.t, target.y, target.x].Wall);
            var nextList = new List<(int X, int Y, int T)>();
            maze[start.t, start.y, start.x].DistanceTo = 0;
            nextList.Add(start);
            while (nextList.Count > 0) {
                // Retrieve the next node with the smallest total distance
                int closestIndex = -1;
                int minDistance = Int32.MaxValue;
                int nextListLen = nextList.Count;
                (int X, int Y, int T) c = start;
                for (int i = 0; i < nextListLen; ++i) {
                    c = nextList[i];
                    Debug.Assert(maze[c.T, c.Y, c.X].Visited);
                    Debug.Assert(!maze[c.T, c.Y, c.X].Complete);
                    int distance = maze[c.T, c.Y, c.X].DistanceTotal;
                    if (distance < minDistance) {
                        closestIndex = i;
                        minDistance = distance;
                    }
                }
                Debug.Assert(closestIndex >= 0);
                c = nextList[closestIndex];
                nextList.RemoveAt(closestIndex);
                maze[c.T, c.Y, c.X].Complete = true;
                if (c == target) {
                    break;
                }
                // Process neighbors
                (int X, int Y, int T) n = (c.X, c.Y - 1, c.T);
                (int X, int Y, int T) w = (c.X - 1, c.Y, c.T);
                (int X, int Y, int T) e = (c.X + 1, c.Y, c.T);
                (int X, int Y, int T) s = (c.X, c.Y + 1, c.T);
                (int X, int Y, int T) t1 = (c.X, c.Y, (c.T + 1) % 3);
                (int X, int Y, int T) t2 = (c.X, c.Y, (c.T + 2) % 3);
                if (n.Y >= 0 && !maze[n.T, n.Y, n.X].Wall){
                    if (!maze[n.T, n.Y, n.X].Visited) {
                        nextList.Add(n);
                    }
                    maze[n.T, n.Y, n.X].DistanceTo =
                        Math.Min(maze[n.T, n.Y, n.X].DistanceTo, maze[c.T, c.Y, c.X].DistanceTo + COST_MOVE);
                }
                if (w.X >= 0 && !maze[w.T, w.Y, w.X].Wall) {
                    if (!maze[w.T, w.Y, w.X].Visited) {
                        nextList.Add(w);
                    }
                    maze[w.T, w.Y, w.X].DistanceTo =
                        Math.Min(maze[w.T, w.Y, w.X].DistanceTo, maze[c.T, c.Y, c.X].DistanceTo + COST_MOVE);
                }
                if (e.X < MAP_SIZE_X && !maze[e.T, e.Y, e.X].Wall) {
                    if (!maze[e.T, e.Y, e.X].Visited) {
                        nextList.Add(e);
                    }
                    maze[e.T, e.Y, e.X].DistanceTo =
                        Math.Min(maze[e.T, e.Y, e.X].DistanceTo, maze[c.T, c.Y, c.X].DistanceTo + COST_MOVE);
                }
                if (s.Y < MAP_SIZE_Y && !maze[s.T, s.Y, s.X].Wall) {
                    if (!maze[s.T, s.Y, s.X].Visited) {
                        nextList.Add(s);
                    }
                    maze[s.T, s.Y, s.X].DistanceTo =
                        Math.Min(maze[s.T, s.Y, s.X].DistanceTo, maze[c.T, c.Y, c.X].DistanceTo + COST_MOVE);
                }
                if (!maze[t1.T, t1.Y, t1.X].Wall) {
                    if (!maze[t1.T, t1.Y, t1.X].Visited) {
                        nextList.Add(t1);
                    }
                    maze[t1.T, t1.Y, t1.X].DistanceTo =
                        Math.Min(maze[t1.T, t1.Y, t1.X].DistanceTo, maze[c.T, c.Y, c.X].DistanceTo + COST_TOOL);
                }
                if (!maze[t2.T, t2.Y, t2.X].Wall) {
                    if (!maze[t2.T, t2.Y, t2.X].Visited) {
                        nextList.Add(t2);
                    }
                    maze[t2.T, t2.Y, t2.X].DistanceTo =
                        Math.Min(maze[t2.T, t2.Y, t2.X].DistanceTo, maze[c.T, c.Y, c.X].DistanceTo + COST_TOOL);
                }
            }

            Debug.Assert(maze[target.t, target.y, target.x].Complete);
            return maze[target.t, target.y, target.x].DistanceTo.ToString();
        }
        static void Main(string[] args) {
            var foo = new List<char>();
            foo.Add('a');
            foo.Add('b');
            foo.Add('c');
            foo.RemoveAt(1);

            Debug.Assert(Solve(510, (10, 10, TOOL_TORCH), true) == "45");
            Console.WriteLine(Solve(4002, (5, 746, TOOL_TORCH), false));
        }
    }
}
