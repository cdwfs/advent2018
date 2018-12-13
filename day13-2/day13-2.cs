using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace day13_2 {
    class Program {
        static Int64 MAP_SIZE_X = -1;
        static Int64 MAP_SIZE_Y = 0;
        class Cart : IComparable {
            public Int64 X, Y;
            public char Dir;
            public bool IsAlive;
            public enum Turn {
                Left = 0,
                Straight = 1,
                Right = 2,
            }
            public Turn NextTurn;

            // TIL: dictionary initializers
            static private Dictionary<(char, Turn), char> _intersectionMap = new Dictionary<(char, Turn), char> {
                [('^', Turn.Left)] = '<',
                [('^', Turn.Straight)] = '^',
                [('^', Turn.Right)] = '>',
                [('>', Turn.Left)] = '^',
                [('>', Turn.Straight)] = '>',
                [('>', Turn.Right)] = 'v',
                [('v', Turn.Left)] = '>',
                [('v', Turn.Straight)] = 'v',
                [('v', Turn.Right)] = '<',
                [('<', Turn.Left)] = 'v',
                [('<', Turn.Straight)] = '<',
                [('<', Turn.Right)] = '^',
            };
            static private Dictionary<(char, char), char> _turnMap = new Dictionary<(char, char), char> {
                [('^', '/')] = '>',
                [('^', '\\')] = '<',
                [('>', '/')] = '^',
                [('>', '\\')] = 'v',
                [('v', '/')] = '<',
                [('v', '\\')] = '>',
                [('<', '/')] = 'v',
                [('<', '\\')] = '^',
            };
            public bool Move(char[,] track, Cart[,] carts) {
                if (!IsAlive) {
                    return false;
                }
                Int64 dx = 0, dy = 0;
                if (Dir == '<') dx = -1;
                else if (Dir == '>') dx = 1;
                else if (Dir == '^') dy = -1;
                else if (Dir == 'v') dy = 1;
                // clear old space
                Debug.Assert(carts[Y, X] == this);
                carts[Y, X] = null;
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
                if (carts[Y, X] != null) {
                    // collision!
                    Cart collidee = carts[Y, X];
                    collidee.IsAlive = false;
                    this.IsAlive = false;
                    carts[Y, X] = null;
                    return true;
                } else {
                    carts[Y, X] = this;
                    return false;
                }
            }

            // TIL: IComparable for Sort()ability.
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
            Cart[,] cartMap = new Cart[MAP_SIZE_Y, MAP_SIZE_X];
            Cart[] carts;
            {
                var cartList = new List<Cart>();
                Int64 y = 0;
                foreach (string row in lines) {
                    for (Int64 x = 0; x < row.Length; ++x) {
                        char c = row[(int)x];
                        if (c == '<' || c == '>') {
                            trackMap[y, x] = '-';
                            var cart = new Cart {
                                X = x,
                                Y = y,
                                Dir = c,
                                IsAlive = true,
                                NextTurn = Cart.Turn.Left,
                            };
                            cartList.Add(cart);
                            cartMap[y, x] = cart;
                        } else if (c == '^' || c == 'v') {
                            trackMap[y, x] = '|';
                            var cart = new Cart {
                                X = x,
                                Y = y,
                                Dir = c,
                                IsAlive = true,
                                NextTurn = Cart.Turn.Left,
                            };
                            cartList.Add(cart);
                            cartMap[y, x] = cart;
                        } else {
                            trackMap[y, x] = c;
                            cartMap[y, x] = null;
                        }
                    }
                    y += 1;
                }
                carts = cartList.ToArray();
                cartList.Clear();
            }
            lines.Clear();
            
            int time = 0;
            while (true) {
                foreach (var cart in carts) {
                    bool collision = cart.Move(trackMap, cartMap);
                    if (collision) {
                        Console.WriteLine($"Collision at {cart.X},{cart.Y}");
                    }
                }
                var liveCarts = Array.FindAll(carts, c => c.IsAlive);
                Debug.Assert(liveCarts.Length > 0);
                // TIL: Count() with a predicate. In this case I actually do want the filtered array, so FindAll() is fine.
                Debug.Assert(liveCarts.Length == carts.Count(c => c.IsAlive));
                if (liveCarts.Length == 1) {
                    Console.WriteLine($"Last surviving cart at {liveCarts[0].X},{liveCarts[0].Y}");
                    return;
                }
                carts = liveCarts;
                Array.Sort(carts);
                ++time;
            }
        }
    }
}
