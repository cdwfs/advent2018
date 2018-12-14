using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace day14_1 {
    class Program {
        const Int64 TARGET_RECIPE_COUNT = 170641; // TIL: consts can't be static
        const Int64 FINAL_RECIPE_COUNT = 10;
        static void Main(string[] args) {
            int[] scores = new int[TARGET_RECIPE_COUNT + FINAL_RECIPE_COUNT + 1];
            scores[0] = 3;
            scores[1] = 7;
            int scoreCount = 2;
            int currentA = 0;
            int currentB = 1;
            while (scoreCount < TARGET_RECIPE_COUNT + FINAL_RECIPE_COUNT) {
                int sum = scores[currentA] + scores[currentB];
                if (sum >= 10) {
                    scores[scoreCount++] = 1;
                    scores[scoreCount++] = sum - 10;
                } else {
                    scores[scoreCount++] = sum;
                }
                currentA = (currentA + 1 + scores[currentA]) % scoreCount;
                currentB = (currentB + 1 + scores[currentB]) % scoreCount;
            }
            for (int i = 0; i < FINAL_RECIPE_COUNT; ++i) {
                Console.Write(scores[TARGET_RECIPE_COUNT + i]);
            }
            Console.WriteLine("");
        }
    }
}
