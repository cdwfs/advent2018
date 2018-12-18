using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day15_2 {
    class Program {
        static int MAP_X = 0, MAP_Y = 0;
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
            if (c.Y > 0 && m[c.Y - 1, c.X] != null) {
                Creature t = m[c.Y - 1, c.X];
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
                Console.Write("  ");
                bool hasCreature = false;
                for (int x = 0; x < MAP_X; ++x) {
                    if (creatureMap[y, x] != null) {
                        hasCreature = true;
                        var c = creatureMap[y, x];
                        Console.Write($" {c.Health}");
                    }
                }
                if (!hasCreature) {
                    Console.Write(" ");
                }
                Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        struct Coordinate : IComparable {
            public int X, Y;

            public int CompareTo(object obj) {
                Coordinate rhs = (Coordinate)obj;
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
                                AttackPower = c == 'G' ? ATTACK_POWER : 25, // binary searched between 3 and 200 to find this value
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
                PrintMap(map, creatureMap);
            }

            // Simulate!
            int completeRounds = 0;
            bool combatDone = false;
            var turnOrder = new Creature[goblins.Length + elves.Length];
            Array.Copy(goblins, 0, turnOrder, 0, goblins.Length);
            Array.Copy(elves, 0, turnOrder, goblins.Length, elves.Length);
            while (!combatDone) {
                // Determine turn order. sort all goblins & elves together by reading order in a separate array.
                Array.Sort(turnOrder);

                // Each unit takes its turn
                foreach (Creature c in turnOrder) {
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
                        var inRangeTileList = new HashSet<Coordinate>(targets.Length * 4);
                        bool hasInRangeTile = false;
                        foreach (var t in targets) {
                            if (t.Y > 0 && map[t.Y - 1, t.X] == '.') {
                                inRangeTileList.Add(new Coordinate { X = t.X, Y = t.Y - 1 });
                                hasInRangeTile = true;
                            }
                            if (t.X > 0 && map[t.Y, t.X - 1] == '.') {
                                inRangeTileList.Add(new Coordinate { X = t.X - 1, Y = t.Y });
                                hasInRangeTile = true;
                            }
                            if (t.X < MAP_X - 1 && map[t.Y, t.X + 1] == '.') {
                                inRangeTileList.Add(new Coordinate { X = t.X + 1, Y = t.Y });
                                hasInRangeTile = true;
                            }
                            if (t.Y < MAP_Y - 1 && map[t.Y + 1, t.X] == '.') {
                                inRangeTileList.Add(new Coordinate { X = t.X, Y = t.Y + 1 });
                                hasInRangeTile = true;
                            }
                        }
                        if (!hasInRangeTile) {
                            continue;
                        }
                        var inRangeTiles = inRangeTileList.ToArray();
                        Array.Sort(inRangeTiles);

                        if (completeRounds == 5 && c.Y == 11 && c.X == 11) {
                            PrintMap(map, creatureMap);
                        }

                        // Get the shortest path from c to all reachable tiles in the map.
                        // Breadth first search FTW.
                        // The dir field of each cell in pathMap only stores the direction of the first move from the start
                        // to reach that cell (1=north, 2=west, 3=east, 4=south)
                        var pathMap = new (int distance, int dir)[MAP_Y, MAP_X];
                        pathMap[c.Y, c.X] = (0, -1);
                        var visitList = new List<(Int64 x, Int64 y)>();
                        visitList.Add((c.X, c.Y));
                        int visitListSize = 1;
                        while (visitListSize != 0) {
                            var newList = new List<(Int64 x, Int64 y)>();
                            int newListSize = 0;
                            foreach (var (x, y) in visitList) {
                                (int distance, int dir) p = pathMap[y, x];
                                if (y > 0 && map[y - 1, x] == '.' && pathMap[y - 1, x].dir == 0) {
                                    if (!newList.Contains((x, y - 1))) {
                                        newList.Add((x, y - 1));
                                        newListSize += 1;
                                        pathMap[y - 1, x] = (p.distance + 1, p.dir == -1 ? 1 : p.dir);
                                    }
                                }
                                if (x > 0 && map[y, x - 1] == '.' && pathMap[y, x - 1].dir == 0) {
                                    if (!newList.Contains((x - 1, y))) {
                                        newList.Add((x - 1, y));
                                        newListSize += 1;
                                        pathMap[y, x - 1] = (p.distance + 1, p.dir == -1 ? 2 : p.dir);
                                    }
                                }
                                if (x < MAP_X - 1 && map[y, x + 1] == '.' && pathMap[y, x + 1].dir == 0) {
                                    if (!newList.Contains((x + 1, y))) {
                                        newList.Add((x + 1, y));
                                        newListSize += 1;
                                        pathMap[y, x + 1] = (p.distance + 1, p.dir == -1 ? 3 : p.dir);
                                    }
                                }
                                if (y < MAP_Y - 1 && map[y + 1, x] == '.' && pathMap[y + 1, x].dir == 0) {
                                    if (!newList.Contains((x, y + 1))) {
                                        newList.Add((x, y + 1));
                                        newListSize += 1;
                                        pathMap[y + 1, x] = (p.distance + 1, p.dir == -1 ? 4 : p.dir);
                                    }
                                }
                            }
                            visitList = newList;
                            visitListSize = newListSize;
                        }

                        // - Find a path to each open tile in range, tracking the shortest path
                        //   (breaking ties by reading order of the destination)
                        //   - If no open tiles are reachable, this creature's turn is over.
                        int minDistance = Int32.MaxValue;
                        bool foundReachableTile = false;
                        Coordinate moveTarget = new Coordinate();
                        foreach (var t in inRangeTiles) {
                            int distance = pathMap[t.Y, t.X].distance;
                            Debug.Assert(distance >= 0);
                            if (distance > 0 && distance < minDistance) {
                                moveTarget = t;
                                minDistance = distance;
                                foundReachableTile = true;
                            }
                        }
                        if (!foundReachableTile) {
                            continue;
                        }
                        // - Take one step along the shortest path towards the target tile.
                        //   - If two paths of equal length exist, the step taken should be the own lower
                        //     in reading order. So, we need to know not just the shortest path but the
                        //     path length from each adjacent tile.
                        int pathDir = pathMap[moveTarget.Y, moveTarget.X].dir;
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
                    PrintMap(map, creatureMap);
                }
            }
            Console.WriteLine("Final state:");
            PrintMap(map, creatureMap);

            // Report results
            Console.WriteLine($"Combat ends after {completeRounds} full rounds");
            string winnerSpecies;
            int winnerTotalHealth = 0;
            if (goblins.Count(g => g.IsAlive) > 0) {
                winnerSpecies = "Goblins";
                winnerTotalHealth = goblins.Sum(g => Math.Max(g.Health, 0));
                Console.WriteLine("Goblins won; who cares?");
                return;
            } else {
                winnerSpecies = "Elves";
                winnerTotalHealth = elves.Sum(e => Math.Max(e.Health, 0));
                if (elves.Any(e => !e.IsAlive)) {
                    Console.WriteLine("Elves won, but some died; who cares?");
                    return;
                }
            }
            Console.WriteLine($"{winnerSpecies} win with {winnerTotalHealth} total hit points left");
            Console.WriteLine($"Outcome: {completeRounds} * {winnerTotalHealth} = {completeRounds * winnerTotalHealth}");
        }
    }
}
