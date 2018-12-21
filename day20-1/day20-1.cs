using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day20_1 {
    class Program {
        class RegexNode {
            public RegexNode(RegexNode parent) {
                Directions = new List<char>();
                GroupParent = parent;
            }
            public List<char> Directions;
            public RegexNode GroupParent;
            public List<RegexNode> Group;
            public RegexNode Next;
        }
        static RegexNode ParseRegex(string regex) {
            RegexNode root = new RegexNode(null);
            RegexNode targetNode = root;
            for(int nextIndex = 0; nextIndex < regex.Length; ++nextIndex) {
                char c = regex[nextIndex];
                if (c == '^') {
                    Debug.Assert(nextIndex == 0, "Hit ^ in the middle of a regex");
                } else if (c == 'N' || c == 'E' || c == 'W' || c == 'S') {
                    Debug.Assert(targetNode.Group == null, "data before groups plz!");
                    Debug.Assert(targetNode.Next == null, "shouldn't know what's next if we're processing data");
                    targetNode.Directions.Add(c);
                } else if (c == '(') { // start new group
                    Debug.Assert(targetNode.Next == null, "starting a new group but there's already a next node?");
                    targetNode.Next = new RegexNode(targetNode.GroupParent);
                    Debug.Assert(targetNode.Group == null, "starting a group but there's already a group?");
                    RegexNode child = new RegexNode(targetNode);
                    targetNode.Group = new List<RegexNode> { child };
                    targetNode = child;
                } else if (c == '|') { // begin new sibling in existing group
                    Debug.Assert(targetNode.GroupParent != null, "new sibling but not in a group!");
                    Debug.Assert(targetNode.GroupParent.Next != null, "group parent should know its next node");
                    Debug.Assert(targetNode.GroupParent.Group != null, "new sibling but parent's group is null");
                    Debug.Assert(targetNode.GroupParent.Group.Count > 0, "new sibling but parent's group is empty");
                    Debug.Assert(targetNode.Group == null, "shouldn't hit | while parsing a parent");
                    Debug.Assert(targetNode.Next == null, "shouldn't hit | if current node has a next");
                    targetNode.Next = targetNode.GroupParent.Next;
                    RegexNode sibling = new RegexNode(targetNode.GroupParent);
                    targetNode.GroupParent.Group.Add(sibling);
                    targetNode = sibling;
                } else if (c == ')') { // end of current group
                    Debug.Assert(targetNode.GroupParent != null, "ending group, but not in a group!");
                    Debug.Assert(targetNode.GroupParent.Next != null, "group parent should know its next node");
                    Debug.Assert(targetNode.GroupParent.Group != null, "ending group, but parent's group is null");
                    Debug.Assert(targetNode.GroupParent.Group.Count > 0, "ending group, but parent's group is empty");
                    Debug.Assert(targetNode.Group == null, "shouldn't hit ) while parsing a parent");
                    Debug.Assert(targetNode.Next == null, "shouldn't hit ) if current node has a next");
                    targetNode.Next = targetNode.GroupParent.Next;
                    targetNode = targetNode.GroupParent.Next;
                } else if (c == '$') { // end of regex
                    Debug.Assert(targetNode.GroupParent == null, "regex ended while in a group!");
                    Debug.Assert(targetNode.Next == null, "hit end of regex with unterminated target");
                    break;
                }
            }
            return root;
        }
        static void DebugTraverse(RegexNode node, string currentPath) {
            string newPath = currentPath + new string(node.Directions.ToArray());
            if (node.Group != null) {
                foreach (var c in node.Group) {
                    DebugTraverse(c, newPath);
                }
            } else if (node.Next != null) {
                DebugTraverse(node.Next, newPath);
            } else if (node.Next == null) {
                Console.WriteLine(newPath);
            } else {
                Debug.Fail("How'd I get here?");
            }
        }
        static int pathCount = 0;
        static void Traverse(RegexNode node, int pathLen, (int x, int y) loc, Dictionary<(int x, int y), int> shortestPathToRoom) {
            if (pathLen == 0) {
                pathCount = 0;
            }
            foreach (char d in node.Directions) {
                pathLen += 1;
                if (d == 'N') {
                    loc.y += 1;
                } else if (d == 'E') {
                    loc.x += 1;
                } else if (d == 'W') {
                    loc.x -= 1;
                } else if (d == 'S') {
                    loc.y -= 1;
                }
                int currentShortest = shortestPathToRoom.ContainsKey(loc) ? shortestPathToRoom[loc] : Int32.MaxValue;
                shortestPathToRoom[loc] = Math.Min(currentShortest, pathLen);
            }
            if (node.Group != null) {
                foreach (var c in node.Group) {
                    Traverse(c, pathLen, loc, shortestPathToRoom);
                }
            } else if (node.Next != null) {
                Traverse(node.Next, pathLen, loc, shortestPathToRoom);
            } else if (node.Next == null) {
                pathCount += 1;
                if ((pathCount % 1000) == 0) {
                    Console.Write(".");
                }
            } else {
                Debug.Fail("How'd I get here?");
            }
        }
        static void BuildMap(RegexNode node, (int x, int y) loc, HashSet<(int x, int y)> roomSet) {
            foreach (char d in node.Directions) {
                if (d == 'N') {
                    loc.y += 1;
                } else if (d == 'E') {
                    loc.x += 1;
                } else if (d == 'W') {
                    loc.x -= 1;
                } else if (d == 'S') {
                    loc.y -= 1;
                }
                roomSet.Add(loc);
            }
            if (node.Group != null) {
                foreach (var c in node.Group) {
                    BuildMap(c, loc, roomSet);
                }
            } else if (node.Next != null) {
                BuildMap(node.Next, loc, roomSet);
            } else if (node.Next == null) {
                pathCount += 1;
                if ((pathCount % 1000) == 0) {
                    Console.Write(".");
                }
            } else {
                Debug.Fail("How'd I get here?");
            }
        }
        static string Solve(string inputFilename, bool enableLogs) {
            // Parse the regex
            RegexNode root;
            {
                var inStream = new StreamReader(inputFilename);
                string line = inStream.ReadLine();
                inStream.Dispose();
                Debug.Assert(line.First() == '^');
                Debug.Assert(line.Last() == '$');
                root = ParseRegex(line);
            }
            // Traverse all possible paths through the regex
            var roomSet = new HashSet<(int x, int y)> {
                (0, 0),
            };
            BuildMap(root, (x: 0, y: 0), roomSet);
            var shortestPathToRoom = new Dictionary<(int x, int y), int> {
                [(0, 0)] = 0,
            };
            Traverse(root, 0, (x:0,y:0), shortestPathToRoom);

            // We want to traverse all paths through the regex,
            // keeping a log of all rooms we visit and the shortest path to that room.
            // Then we just iterate over all rooms and look for the largest shortest path.
            int longest = Int32.MinValue;
            foreach(var room in shortestPathToRoom) {
                if (enableLogs) {
                    Console.WriteLine($"{room.Key.x},{room.Key.y} = {room.Value}");
                }
                longest = Math.Max(longest, room.Value);
            }
            return longest.ToString();
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
