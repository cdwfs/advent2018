using System;
using System.Diagnostics;
using System.IO;

namespace day02_1 {
    class Program {
        static void Main(string[] args) {
            var inStream = new StreamReader(args[0]);
            string line = "";
            int twoCount = 0;
            int threeCount = 0;
            while ((line = inStream.ReadLine()) != null) {
                int[] letterCounts = new int[26]; // defaults to zero for all entries
                foreach (char c in line) {
                    int letterIndex = c - 'a';
                    Debug.Assert(letterIndex >= 0 && letterIndex < 26, $"non-lowercase char {c} in string!");
                    letterCounts[letterIndex] += 1;
                }
                bool hasTwo = false;
                bool hasThree = false;
                foreach (int count in letterCounts) {
                    if (count == 2) {
                        hasTwo = true;
                    } else if (count == 3) {
                        hasThree = true;
                    }
                }
                if (hasTwo) {
                    //Console.WriteLine($"{line} has 2x!");
                    twoCount += 1;
                } if (hasThree) {
                    //Console.WriteLine($"{line} has 3x!");
                    threeCount += 1;
                }
            }
            Console.WriteLine($"twos: {twoCount}, threes: {threeCount}, product: {twoCount*threeCount}");
        }
    }
}
