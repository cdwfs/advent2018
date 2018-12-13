using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace day08_2 {
    class Program {
        class TreeNode {
            public TreeNode(ref StreamReader stream) {
                int childCount = Int32.Parse(stream.ReadLine());
                int metadataCount = Int32.Parse(stream.ReadLine());
                Children = new TreeNode[childCount];
                Metadata = new int[metadataCount];
                for (int c = 0; c < childCount; ++c) {
                    Children[c] = new TreeNode(ref stream);
                }
                for (int m = 0; m < metadataCount; ++m) {
                    Metadata[m] = Int32.Parse(stream.ReadLine());
                }
            }
            public int Value() {
                if (Children.Length == 0) {
                    return Metadata.Sum();
                } else {
                    int value = 0;
                    foreach (int m in Metadata) {
                        if (m - 1 < Children.Length) {
                            value += Children[m - 1].Value();
                        }
                    }
                    return value;
                }
            }
            public int[] Metadata;
            public TreeNode[] Children;
        }
        static void Main(string[] args) {
            StreamReader inStream = new StreamReader(args[0]);
            TreeNode root = new TreeNode(ref inStream);
            inStream.Dispose();
            Console.WriteLine($"value is {root.Value()}");
        }
    }
}