using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day13_1 {
    class Program {
        static Int64 MAP_SIZE_X = -1;
        static Int64 MAP_SIZE_Y = 0;
        class Cart : IComparable {
            public Int64 X, Y;
            public char Dir;
            public enum Turn {
                Left = 0,
                Straight = 1,
                Right = 2,
            }
            public Turn NextTurn;

            static private Dictionary<(char, Turn), char> _intersectionMap = new Dictionary<(char, Turn), char> {
                [('^',Turn.Left)] = '<', [('^', Turn.Straight)] = '^', [('^', Turn.Right)] = '>',
                [('>',Turn.Left)] = '^', [('>', Turn.Straight)] = '>', [('>', Turn.Right)] = 'v',
                [('v',Turn.Left)] = '>', [('v', Turn.Straight)] = 'v', [('v', Turn.Right)] = '<',
                [('<',Turn.Left)] = 'v', [('<', Turn.Straight)] = '<', [('<', Turn.Right)] = '^',
            };
            static private Dictionary<(char, char), char> _turnMap = new Dictionary<(char, char), char> {
                [('^', '/')] = '>', [('^', '\\')] = '<',
                [('>', '/')] = '^', [('>', '\\')] = 'v',
                [('v', '/')] = '<', [('v', '\\')] = '>',
                [('<', '/')] = 'v', [('<', '\\')] = '^',
            };
            public bool Move(ref char[,] track, ref char[,] carts) {
                Int64 dx = 0, dy = 0;
                if (Dir == '<') dx = -1;
                else if (Dir == '>') dx = 1;
                else if (Dir == '^') dy = -1;
                else if (Dir == 'v') dy = 1;
                // clear old space
                Debug.Assert(carts[Y, X] == Dir);
                carts[Y, X] = '.';
                // Determine new direction
                char newT = track[Y + dy, X + dx];
                if (newT == '+') {
                    Dir = _intersectionMap[(Dir, NextTurn)];
                    NextTurn = (Turn)(((int)NextTurn + 1) % 3);
                } else if (newT == '/' || newT == '\\') {
                    Dir = _turnMap[(Dir, newT)];
                }
                // Update cart position and map
                Y += dy;
                X += dx;
                if (carts[Y, X] != '.') {
                    // collision!
                    carts[Y, X] = 'X';
                    return true;
                } else {
                    carts[Y, X] = Dir;
                    return false;
                }
            }

            public int CompareTo(object obj) {
                Cart rhs = (Cart)obj;
                Int64 lhsI = Y * MAP_SIZE_X + X;
                Int64 rhsI = rhs.Y * MAP_SIZE_X + rhs.X;
                if (lhsI > rhsI) {
                    return 1;
                } else if (lhsI < rhsI) {
                    return -1;
                } else {
                    return 0;
                }
            }
        }

        static void Main(string[] args) {
            // Get map dimensions.
            StreamReader inStream = new StreamReader(args[0]);
            string line;
            var lines = new List<string>();
            while ((line = inStream.ReadLine()) != null) {
                if (MAP_SIZE_X == -1) {
                    MAP_SIZE_X = line.Length;
                } else {
                    Debug.Assert(line.Length == MAP_SIZE_X, "row length mismatch");
                }
                MAP_SIZE_Y += 1;
                lines.Add(line);
            }
            inStream.Dispose();

            char[,] trackMap = new char[MAP_SIZE_Y, MAP_SIZE_X];
            char[,] cartMap = new char[MAP_SIZE_Y, MAP_SIZE_X];
            Cart[] carts;
            {
                var cartList = new List<Cart>();
                Int64 y = 0;
                foreach (string row in lines) {
                    for (Int64 x = 0; x < row.Length; ++x) {
                        char c = row[(int)x];
                        if (c == '<' || c == '>') {
                            trackMap[y, x] = '-';
                            cartList.Add(new Cart {
                                X = x,
                                Y = y,
                                Dir = c,
                                NextTurn = Cart.Turn.Left,
                            });
                            cartMap[y, x] = c;
                        } else if (c == '^' || c == 'v') {
                            trackMap[y, x] = '|';
                            cartList.Add(new Cart {
                                X = x,
                                Y = y,
                                Dir = c,
                                NextTurn = Cart.Turn.Left,
                            });
                            cartMap[y, x] = c;
                        } else {
                            trackMap[y, x] = c;
                            cartMap[y, x] = '.';
                        }
                    }
                    y += 1;
                }
                carts = cartList.ToArray();
                cartList.Clear();
            }
            lines.Clear();

            //Array.Sort(carts);

            int time = 0;
            while (true) {
                foreach (var cart in carts) {
                    bool collision = cart.Move(ref trackMap, ref cartMap);
                    if (collision) {
                        Console.WriteLine($"Collision at {cart.X},{cart.Y}");
                        return;
                    }
                }
                Array.Sort(carts);
                ++time;
            }
        }
    }
}
