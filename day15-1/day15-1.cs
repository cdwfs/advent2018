using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day15_1 {
    class Program {
        static int MAP_X = 0, MAP_Y = 0;
        // A* implementation from http://gigi.nullneuron.net/gigilabs/a-pathfinding-example-in-c/
        class Location {
            public int X;
            public int Y;
            public int F;
            public int G;
            public int H;
            public Location Parent;
        }
        static List<Location> GetWalkableAdjacentSquares(int x, int y, int targetX, int targetY, char[,] map) {
            var proposedLocations = new List<Location>()
            {
                new Location { X = x, Y = y - 1 },
                new Location { X = x, Y = y + 1 },
                new Location { X = x - 1, Y = y },
                new Location { X = x + 1, Y = y },
            };

            return proposedLocations.Where(l => map[l.Y,l.X] == '.' || (l.X == targetX && l.Y == targetY)).ToList();
        }
        static int FindPath(int fromX, int fromY, int toX, int toY, char[,] map) {
            Debug.Assert(map[toY, toX] == '.', "pathing to occupied cell");
            if (map[fromY, fromX] != '.') {
                return -1;
            }
            Location current = null;
            var start = new Location { X = fromX, Y = fromY };
            var target = new Location { X = toX, Y = toY };
            var openList = new List<Location>();
            var closedList = new List<Location>();
            int g = 0;

            // start by adding the original position to the open list
            openList.Add(start);

            bool foundPath = false;
            while (openList.Count > 0) {
                // get the square with the lowest F score
                var lowest = openList.Min(l => l.F);
                current = openList.First(l => l.F == lowest);

                // add the current square to the closed list
                closedList.Add(current);

                // remove it from the open list
                openList.Remove(current);

                // if we added the destination to the closed list, we've found a path
                if (closedList.FirstOrDefault(l => l.X == target.X && l.Y == target.Y) != null) {
                    foundPath = true;
                    break;
                }

                var adjacentSquares = GetWalkableAdjacentSquares(current.X, current.Y, toX, toY, map);
                g++;

                foreach (var adjacentSquare in adjacentSquares) {
                    // if this adjacent square is already in the closed list, ignore it
                    if (closedList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) != null)
                        continue;

                    // if it's not in the open list...
                    if (openList.FirstOrDefault(l => l.X == adjacentSquare.X
                            && l.Y == adjacentSquare.Y) == null) {
                        // compute its score, set the parent
                        adjacentSquare.G = g;
                        adjacentSquare.H = ComputeHScore(adjacentSquare.X, adjacentSquare.Y, target.X, target.Y);
                        adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                        adjacentSquare.Parent = current;

                        // and add it to the open list
                        openList.Insert(0, adjacentSquare);
                    } else {
                        // test if using the current G score makes the adjacent square's F score
                        // lower, if yes update the parent because it means it's a better path
                        if (g + adjacentSquare.H < adjacentSquare.F) {
                            adjacentSquare.G = g;
                            adjacentSquare.F = adjacentSquare.G + adjacentSquare.H;
                            adjacentSquare.Parent = current;
                        }
                    }
                }
            }
            if (foundPath) {
                int length = 0;
                while (current != null) {
                    length += 1;
                    current = current.Parent;
                }
                return length;
            } else {
                return -1;
            }
        }

        static int ComputeHScore(int x, int y, int targetX, int targetY) {
            return Math.Abs(targetX - x) + Math.Abs(targetY - y);
        }

        // Creature class
        const int ATTACK_POWER = 3;
        const int STARTING_HEALTH = 200;
        class Creature : IComparable {
            public int X, Y;
            public char Species;
            public int Health;
            public int AttackPower;
            public bool IsAlive { get => Health > 0; }

            public int CompareTo(object obj) {
                Creature rhs = (Creature)obj;
                int lhsIndex = Y * MAP_X + X;
                int rhsIndex = rhs.Y * MAP_X + rhs.X;
                if (lhsIndex < rhsIndex) {
                    return -1;
                } else if (lhsIndex > rhsIndex) {
                    return 1;
                }
                return 0;
            }
        }
        // Check adjacent tiles and return the enemy creature that should be attacked, or null if no such target exists.
        static Creature FindAdjacentTarget(Creature c, Creature[,] m) {
            Creature target = null;
            int minTargetHealth = Int32.MaxValue;
            if (c.Y > 0 && m[c.Y-1, c.X] != null) {
                Creature t = m[c.Y-1, c.X];
                if (t.IsAlive && t.Species != c.Species && t.Health < minTargetHealth) {
                    target = t;
                    minTargetHealth = t.Health;
                }
            }
            if (c.X > 0 && m[c.Y, c.X - 1] != null) {
                Creature t = m[c.Y, c.X - 1];
                if (t.IsAlive && t.Species != c.Species && t.Health < minTargetHealth) {
                    target = t;
                    minTargetHealth = t.Health;
                }
            }
            if (c.X < MAP_X - 1 && m[c.Y, c.X + 1] != null) {
                Creature t = m[c.Y, c.X + 1];
                if (t.IsAlive && t.Species != c.Species && t.Health < minTargetHealth) {
                    target = t;
                    minTargetHealth = t.Health;
                }
            }
            if (c.Y < MAP_Y - 1 && m[c.Y + 1, c.X] != null) {
                Creature t = m[c.Y + 1, c.X];
                if (t.IsAlive && t.Species != c.Species && t.Health < minTargetHealth) {
                    target = t;
                    minTargetHealth = t.Health;
                }
            }
            return target;
        }
        // Print map state
        static void PrintMap(char[,] map, Creature[,] creatureMap) {
            return;
            for (int y = 0; y < MAP_Y; ++y) {
                for (int x = 0; x < MAP_X; ++x) {
                    Console.Write(map[y, x]);
                }
                for (int x = 0; x < MAP_X; ++x) {
                    if (creatureMap[y, x] != null) {
                        var c = creatureMap[y, x];
                        Console.Write($" {c.Species}({c.Health})");
                    }
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        static void Main(string[] args) {
            // Read map
            char[,] map;
            Creature[,] creatureMap;
            Creature[] goblins;
            Creature[] elves;
            {
                var inStream = new StreamReader(args[0]);
                var lines = new List<string>();
                string line;
                while ((line = inStream.ReadLine()) != null) {
                    if (MAP_Y == 0) {
                        MAP_X = line.Length;
                    } else {
                        Debug.Assert(line.Length == MAP_X, $"incorrect line length (expected {MAP_X}, got {line.Length}");
                    }
                    lines.Add(line);
                    MAP_Y += 1;
                }
                inStream.Dispose();
                map = new char[MAP_Y, MAP_X];
                creatureMap = new Creature[MAP_Y, MAP_X];
                int y = 0;
                var goblinList = new List<Creature>();
                var elfList = new List<Creature>();
                foreach (var row in lines) {
                    for (int x = 0; x < MAP_X; ++x) {
                        char c = row[x];
                        if (c == 'G' || c == 'E') {
                            var creature = new Creature {
                                X = x,
                                Y = y,
                                Species = c,
                                Health = STARTING_HEALTH,
                                AttackPower = ATTACK_POWER,
                            };
                            if (c == 'G') {
                                goblinList.Add(creature);
                            } else {
                                elfList.Add(creature);
                            }
                            creatureMap[y, x] = creature;
                        }
                        map[y, x] = row[x];
                    }
                    y += 1;
                }
                goblins = goblinList.ToArray();
                elves = elfList.ToArray();
            }

            // Simulate!
            int completeRounds = 0;
            bool combatDone = false;
            var turnOrder = new Creature[goblins.Length + elves.Length];
            Array.Copy(goblins, 0, turnOrder, 0, goblins.Length);
            Array.Copy(elves, 0, turnOrder, goblins.Length, elves.Length);
            while (!combatDone) {
                PrintMap(map, creatureMap);

                // Determine turn order. sort all goblins & elves together by reading order in a separate array.
                Array.Sort(turnOrder);

                // Each unit takes its turn
                foreach(Creature c in turnOrder) {
                    // If this creature has health = 0, it is skipped (units can die mid-round)
                    if (!c.IsAlive) {
                        continue;
                    }

                    // Find all living targets
                    var targets = turnOrder.Where(t => t.IsAlive && t.Species != c.Species).ToArray();
                    // If none left, early out; combat is over
                    if (targets.Length == 0) {
                        combatDone = true;
                        break;
                    }

                    // If not adjacent to a target:
                    Creature target = FindAdjacentTarget(c, creatureMap);
                    if (target == null) {
                        // - Identify all open tiles in range (not occupied, and adjacent to an enemy)
                        //   - If no open tiles are in range, this creature's turn is over.
                        var inRangeTiles = new HashSet<(int x, int y)>(targets.Length * 4);
                        bool hasInRangeTile = false;
                        foreach (var t in targets) {
                            if (t.Y > 0 && map[t.Y - 1, t.X] == '.') {
                                inRangeTiles.Add((t.X, t.Y - 1));
                                hasInRangeTile = true;
                            }
                            if (t.X > 0 && map[t.Y, t.X - 1] == '.') {
                                inRangeTiles.Add((t.X - 1, t.Y));
                                hasInRangeTile = true;
                            }
                            if (t.X < MAP_X - 1 && map[t.Y, t.X + 1] == '.') {
                                inRangeTiles.Add((t.X + 1, t.Y));
                                hasInRangeTile = true;
                            }
                            if (t.Y < MAP_Y - 1 && map[t.Y + 1, t.X] == '.') {
                                inRangeTiles.Add((t.X, t.Y + 1));
                                hasInRangeTile = true;
                            }
                        }
                        if (!hasInRangeTile) {
                            continue;
                        }

                        // - Find a path to each open tile in range, tracking the shortest path
                        //   (breaking ties by reading order of the destination)
                        //   - If no open tiles are reachable, this creature's turn is over.
                        bool foundReachableTile = false;
                        int minDistance = Int32.MaxValue;
                        int pathDir = 0;
                        foreach (var t in inRangeTiles) {
                            if (c.Y > 0) {
                                int distance = FindPath(c.X, c.Y-1, t.x, t.y, map);
                                if (distance > 0 && distance < minDistance) {
                                    foundReachableTile = true;
                                    minDistance = distance;
                                    pathDir = 1;
                                }
                            }
                            if (c.X > 0) {
                                int distance = FindPath(c.X-1, c.Y, t.x, t.y, map);
                                if (distance > 0 && distance < minDistance) {
                                    foundReachableTile = true;
                                    minDistance = distance;
                                    pathDir = 2;
                                }
                            }
                            if (c.X <= MAP_X - 1) {
                                int distance = FindPath(c.X+1, c.Y, t.x, t.y, map);
                                if (distance > 0 && distance < minDistance) {
                                    foundReachableTile = true;
                                    minDistance = distance;
                                    pathDir = 3;
                                }
                            }
                            if (c.Y < MAP_Y - 1) {
                                int distance = FindPath(c.X, c.Y+1, t.x, t.y, map);
                                if (distance > 0 && distance < minDistance) {
                                    foundReachableTile = true;
                                    minDistance = distance;
                                    pathDir = 4;
                                }
                            }
                        }
                        if (!foundReachableTile) {
                            continue;
                        }
                        // - Take one step along the shortest path towards the target tile.
                        //   - If two paths of equal length exist, the step taken should be the own lower
                        //     in reading order. So, we need to know not just the shortest path but the
                        //     path length from each adjacent tile.
                        (int newX, int newY) = (c.X, c.Y);
                        if (pathDir == 1) {
                            newY -= 1;
                        } else if (pathDir == 2) {
                            newX -= 1;
                        } else if (pathDir == 3) {
                            newX += 1;
                        } else if (pathDir == 4) {
                            newY += 1;
                        }
                        Debug.Assert(newX >= 0 && newX <= MAP_X - 1);
                        Debug.Assert(newY >= 0 && newY <= MAP_Y - 1);
                        Debug.Assert(newX != c.X || newY != c.Y, "invalid path direction?");
                        Debug.Assert(map[newY, newX] == '.', $"map cell {newX},{newY} is occupied");
                        Debug.Assert(creatureMap[newY, newX] == null, $"creature map cell {newX},{newY} is occupied");
                        map[c.Y, c.X] = '.';
                        map[newY, newX] = c.Species;
                        creatureMap[c.Y, c.X] = null;
                        creatureMap[newY, newX] = c;
                        c.X = newX;
                        c.Y = newY;

                        // try to find an adjacent target again after moving
                        target = FindAdjacentTarget(c, creatureMap);
                    }

                    // - Consider all targets adjacent to this creature, and attack the one lowest in
                    //   hit points (breaking ties by reading order)
                    //   - If no targets are adjacent, turn is over
                    if (target == null) {
                        continue;
                    }
                    // - Subtract attacker's attack power from the target's health. If it's <= 0, the
                    //   target dies, and its tile becomes empty.
                    target.Health -= c.AttackPower;
                    if (target.Health <= 0) {
                        map[target.Y, target.X] = '.';
                        creatureMap[target.Y, target.X] = null;
                    }
                }
                if (!combatDone) {
                    completeRounds += 1;
                    Console.WriteLine($"{completeRounds} rounds complete");
                }
            }
            PrintMap(map, creatureMap);

            // Report results
            Console.WriteLine($"Combat ends after {completeRounds} full rounds");
            string winnerSpecies;
            int winnerTotalHealth = 0;
            if (goblins.Count(g => g.IsAlive) > 0) {
                winnerSpecies = "Goblins";
                winnerTotalHealth = goblins.Sum(g => g.Health);
            } else {
                winnerSpecies = "Elves";
                winnerTotalHealth = elves.Sum(e => Math.Max(e.Health, 0));
            }
            Console.WriteLine($"{winnerSpecies} win with {winnerTotalHealth} total hit points left");
            Console.WriteLine($"Outcome: {completeRounds} * {winnerTotalHealth} = {completeRounds * winnerTotalHealth}");
        }
    }
}
