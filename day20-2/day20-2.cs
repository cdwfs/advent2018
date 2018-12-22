using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day20_1 {
    class Program {
        const int NORTH_DOOR = 1;
        const int EAST_DOOR = 2;
        const int SOUTH_DOOR = 4;
        const int WEST_DOOR = 8;
        static string Solve(string inputFilename, bool enableLogs) {
            // Construct the facility map
            var roomDoors = new Dictionary<(int x, int y), int> {
                [(0, 0)] = 0,
            };
            int minX = Int32.MaxValue, minY = Int32.MaxValue, maxX = Int32.MinValue, maxY = Int32.MinValue;
            {
                var inStream = new StreamReader(inputFilename);
                string line = inStream.ReadLine();
                inStream.Dispose();
                Debug.Assert(line.First() == '^');
                Debug.Assert(line.Last() == '$');
                var posStack = new Stack<(int x, int y)>();
                (int x, int y) pos = (0, 0);
                for (int i = 0; i < line.Length; ++i) {
                    char c = line[i];
                    if (c == '^') {
                        Debug.Assert(i == 0);
                    } else if (c == '$') {
                        Debug.Assert(i == line.Length - 1);
                    } else if (c == 'N') {
                        roomDoors[pos] |= NORTH_DOOR;
                        pos.y += 1;
                        if (!roomDoors.ContainsKey(pos)) {
                            roomDoors[pos] = SOUTH_DOOR;
                        } else {
                            roomDoors[pos] |= SOUTH_DOOR;
                        }
                    } else if (c == 'E') {
                        roomDoors[pos] |= EAST_DOOR;
                        pos.x += 1;
                        if (!roomDoors.ContainsKey(pos)) {
                            roomDoors[pos] = WEST_DOOR;
                        } else {
                            roomDoors[pos] |= WEST_DOOR;
                        }
                    } else if (c == 'S') {
                        roomDoors[pos] |= SOUTH_DOOR;
                        pos.y -= 1;
                        if (!roomDoors.ContainsKey(pos)) {
                            roomDoors[pos] = NORTH_DOOR;
                        } else {
                            roomDoors[pos] |= NORTH_DOOR;
                        }
                    } else if (c == 'W') {
                        roomDoors[pos] |= WEST_DOOR;
                        pos.x -= 1;
                        if (!roomDoors.ContainsKey(pos)) {
                            roomDoors[pos] = EAST_DOOR;
                        } else {
                            roomDoors[pos] |= EAST_DOOR;
                        }
                    } else if (c == '(') {
                        posStack.Push(pos);
                    } else if (c == '|') {
                        pos = posStack.Peek();
                    } else if (c == ')') {
                        pos = posStack.Pop();
                    }
                    minX = Math.Min(pos.x, minX);
                    minY = Math.Min(pos.y, minY);
                    maxX = Math.Max(pos.x, maxX);
                    maxY = Math.Max(pos.y, maxY);
                }
            }

            // BFS
            var shortestRoomPath = new Dictionary<(int x, int y), int> {
            };
            var bfsQueue = new Queue<(int x, int y, int pathLen)>();
            bfsQueue.Enqueue((0, 0, 0));
            while (bfsQueue.Count() > 0) { // TODO: no quick Empty() test?
                var room = bfsQueue.Dequeue();
                shortestRoomPath[(room.x, room.y)] = room.pathLen;
                int doors = roomDoors[(room.x, room.y)];
                if ((doors & NORTH_DOOR) != 0 && !shortestRoomPath.ContainsKey((room.x, room.y + 1))) {
                    bfsQueue.Enqueue((room.x, room.y + 1, room.pathLen + 1));
                }
                if ((doors & EAST_DOOR) != 0 && !shortestRoomPath.ContainsKey((room.x + 1, room.y))) {
                    bfsQueue.Enqueue((room.x + 1, room.y, room.pathLen + 1));
                }
                if ((doors & SOUTH_DOOR) != 0 && !shortestRoomPath.ContainsKey((room.x, room.y - 1))) {
                    bfsQueue.Enqueue((room.x, room.y - 1, room.pathLen + 1));
                }
                if ((doors & WEST_DOOR) != 0 && !shortestRoomPath.ContainsKey((room.x - 1, room.y))) {
                    bfsQueue.Enqueue((room.x - 1, room.y, room.pathLen + 1));
                }
            }
            // TODO: should be a one-liner for the max value in a dictionary?
            return shortestRoomPath.Values.Count(pl => (pl >= 1000)).ToString();
        }
        static void Main(string[] args) {
            //Debug.Assert(Solve(args[0] + @"\day20-example0.txt", false) == "3");
            //Debug.Assert(Solve(args[0] + @"\day20-example1.txt", false) == "10");
            //Debug.Assert(Solve(args[0] + @"\day20-example2.txt", false) == "18");
            //Debug.Assert(Solve(args[0] + @"\day20-example3.txt", false) == "23");
            //Debug.Assert(Solve(args[0] + @"\day20-example4.txt", false) == "31");
            Console.WriteLine(Solve(args[0] + @"\day20-input.txt", false));
        }
    }
}
