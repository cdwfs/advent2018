### TIL

##### [Day 1](https://adventofcode.com/2018/day/1): Frequency Calibration
- Basic Visual Studio setup for C# desktop projects.
- Text file I/O using `StreamReader`.
- string-to-int conversion with `Int32.Parse()`.
- `Console.WriteLine()` with formatted $-strings.
- `HashSet` collections (`std::set` alternative).

##### [Day 2](https://adventofcode.com/2018/day/2): Box Code Scanning
- `Debug.Assert()`
- new POD allocations always (?) default to zero.
- `List.Add()` to append to `List`. `List.Append()` is totally different.
- `String.Substring()` for string slicing
- Currently O(N² * L) but O(NlogN * L) is possible:
  - Read all lines into an array
  - for each character pos C in 1..L:
    - make a copy of lines array without character C
    - sort array
    - linear scan for duplicates. A duplicate means the strings were identical
      except for character C. 

##### [Day 3](https://adventofcode.com/2018/day/3): Overlapping Fabric Patches
- The `Regex` class.
- `Dictionary` collections.
  - In a `Dictionary<(int,int), (int,int)>`, can't modify `dict[key].Item1` --
    can only replace the entire value (`dict[key] = (newItem1, dict[key].Item2`).
    Why not?

##### [Day 4](https://adventofcode.com/2018/day/4): Tracking Sleeping Guards
- Looked for a way to get a reference to Max element in a collection. Nothing
  as fast as a handwritten loop that tracks max index + value manually.
- Can't modify dictionary contents in a `foreach`.


##### [Day 5](https://adventofcode.com/2018/day/5): Polymer trimming
- Jagged arrays (arrays of arrays) vs. multi-dimensional arrays (char[5,3]).
  - Jagged arrays must allocate each sub-array individually, but they can
    have different sizes and are individually accessible.
  - Multi-dimensional arrays are only a single allocation, but you can't
    get a sub-array object. I think.

##### [Day 6](https://adventofcode.com/2018/day/6): Safe / Unsafe region mapping
- Named constants, I guess? Not much new material today.

##### [Day 7](https://adventofcode.com/2018/day/7): job scheduling
- Named tuples -- (string name, int id) foo; foo.name = "Ben"; foo.id = 7;
- Advanced $-string formatting -- $"{count,3:D} will be padded to three characters".
  - How to pad with zeroes? Floats? Hex?

##### [Day 8](https://adventofcode.com/2018/day/8): tree building
- trees / recursion.
- tried to experiment with making my own stream/generator -- read in input file
  into a string, and pop numbers off the front on demand. I tried to do this using
  C# yield and IEnumerator, but I don't think it's the correct approach here; that's
  mainly for `foreach` support. Rolling my own class would probably be sufficient.

##### [Day 9](https://adventofcode.com/2018/day/9): deterministic marble game
- `Int64` by default!
- Functions can return named tuples.
- TODO optimizations:
  - store as an array (or SoA) instead of a linked list, to save on allocation costs
    and improve caching
  - process 23 marbles per loop iteration. The first 22 can all be added at once;
    only the 23rd is weird.

##### [Day 10](https://adventofcode.com/2018/day/10): messages in the stars
- The `System.Drawing` module:
  - Create `Bitmap` objects
  - For pixel-level ops, use `Bitmap.GetPixel()` and `Bitmap.SetPixel()`.
  - For bitmap-wide ops like Clear, use `Graphics g = Graphics.FromImage(bmp)` and then `g.Clear()`.
  - `bmp.Save()` can write to a variety of standard formats (PNG, JPG).
- [tesseract](https://github.com/tesseract-ocr/tesseract) open-source OCR. Some people were using
  it to try and auto-recognize the output text and print it to the console, rather than rely on
  visually inspecting the output images. I never got it working, even with a large border and
  scaled-up pixels. Nice to have in the toolbox though, I guess!

##### [Day 11](https://adventofcode.com/2018/day/11): grid of fuel cells
- multi-dimensional array memory layout. According to the [CLI spec](http://www.ecma-international.org/publications/standards/Ecma-335.htm):
> Array elements shall be  laid out within the array object in row-major order
> (i.e., the elements associated with the rightmost array dimension shall be laid
> out contiguously from lowest to highest index).
- Semi-clever algorithmic speedup to avoid redundant sums

##### [Day 12](https://adventofcode.com/2018/day/12): cellular automata plants
- `Match.Groups[key].Value` is probably preferable to `Match.Groups[key].ToString()`.
- `StreamReader` objects should be `Dispose()`ed when they're finished.
- `Array.FindIndex()` and `FindLastIndex()`

##### [Day 13](https://adventofcode.com/2018/day/13): traffic simulator
- [Dictionary initializer syntax](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/object-and-collection-initializers#collection-initializers) (C#6+)
- `IComparable` for `Sort()`ability.
- `Array.Count(predicate)` to count the elements that satisfy a predicate without
  creating a temporary collection.
  - Requires `System.Linq`
  - In this case, I actually want the filtered collection as well, so I use
    `Array.FindAll()`.

##### [Day 14](https://adventofcode.com/2018/day/14): recipe scoring
- `const` members can't be static? (not a problem; I just wonder why that is)

##### [Day 15](https://adventofcode.com/2018/day/15): nethack combat
- Debug the scoring code as well as the program logic :(
- Linq can be useful where performance isn't the primary goal.
- Don't trust random code from the internet; the A* implementation I found
  was orders of magnitude slower (and buggier) than my brute-force
  breadth-first search.
- Make my own solid A* implementation for next time.

##### [Day 16](https://adventofcode.com/2018/day/16): CPU reverse engineering
- Can't pass foreach iterators as `ref` parameters to functions.

##### [Day 17](https://adventofcode.com/2018/day/17): minecraft water
- Settling on a template:
  - Put main puzzle logic in a `Solve()` function that returns a string
  - Include examples as unit tests & assert against their expected values
  - Make debug printout a flag to solve, so that problematic cases can be
    debugged in isolation.
  - command line argument should be the inputs directory, not the specific
    filename.
- Once again, double-check the scoring code! In this case, I wasn't counting
  water outside the range xMin..xMax, even though the rules say any x coordinate
  was allowed.

##### [Day 18](https://adventofcode.com/2018/day/18): game of life
- `GetHashCode()` for generalized arrays does not take the array contents into account.
  So, if you want to use an array as a `Dictionary` key, you need to manually accumulate
  the contents.
- `char[].ToString()` does not do what you think it does! It gives you a human-readable name
  for the variable. To convert the array into a `string` object, use `new string(char[])`.

##### [Day 19](https://adventofcode.com/2018/day/19): assembly reading
- Seriously just use 64-bit ints for everything. Overflow bugs *suck*.
  - ...or enable integer overflow detection project-wide!
    (Properties -> Build -> Advanced)
- Dictionary of anonymous delegates for each opcode:
    ```
    delegate void ProcessOp(Int64 a, Int64 b, Int64 c, Int64[] regs);
    static Dictionary<string, ProcessOp> ops = new Dictionary<string, ProcessOp> {
        ["addr"] = (a, b, c, regs) => regs[c] = (regs[a] + regs[b]),
        ["addi"] = (a, b, c, regs) => regs[c] = (regs[a] + b),
        ...
    };
    // Simulate the instruction "addr 1 2 3"
    ops["addr"](1,2,3,regs);

    ```

##### [Day 20](https://adventofcode.com/2018/day/20): mapping via regex
- Read the instructions carefully. I lost hours on this puzzle assuming that
  `()` meant the same thing here that it does in _regular_ regular expressions.
  It doesn't; it's much simpler than I thought, and turned O(days) into O(N). 

##### [Day 21](https://adventofcode.com/2018/day/21): ElfCode disassembly again
- No real language/standard library lessons today.
- I did spend longer than necessary trying to understand the code vs. plugging in
  random data / setting breakpoints, and the latter would've brought me to a solution
  faster.

##### [Day 22](https://adventofcode.com/2018/day/22): subterranean maze
- Tuple equality requires C# 7.3
- Write your own A* :)
  - I'm not completely satisfied with mine -- it seems like the ideal
    structure to store the incomplete nodes to explore is a heap-based
    priority queue (O(N) insertion and extract-minimum), but it's also
    necessary to edit the priority of arbitrary nodes in the queue as
    new paths are discovered and their costs are adjusted. Naively, that
    would be O(N) in a heap. The obvious solution would be for each node
    in the navigation graph to track the position of its corresponding
    entry in the heap so that editing becomes O(logN), but that's more
    bookkeeping than I had the patience for today.
  - In fact, my solution just uses an unsorted List instead of a
    priority queue. Maybe the pending node count just never gets high
    enough in practice for the algorithmic complexity to matter here.

##### [Day 23](https://adventofcode.com/2018/day/23): octahedron/cube intersection
- I spent _waaaay_ too long trying to cleverly intersect axis-aligned cubes
  and axis-aligned octahedrons; it was _much_ simpler to just compute the
  Manhattan distance from the bot position to the edge of the cube and compare
  against the bot radius.
- Also, my initial recursive descent subdivided each level into a 10x10x10 grid,
  which was way too many children to check. 2x2x2 was significantly more efficient;
  the tree was 20 levels deeper, but each level had 8 children instead of 1000.
- My implementation does two passes -- first to determine the max in-range
  count, and a second to determine the min-distance point with that count.
  That may not be necessary after the previous optimizations, but I was too lazy
  to remove it after getting the correct answer so quickly.
- I should've used a custom sorting function instead of negating the sort
  index, but hey.

##### [Day 24](https://adventofcode.com/2018/day/24): immune system battle royale
- [String.Split()](https://docs.microsoft.com/en-us/dotnet/api/system.string.split?f1url=https%3A%2F%2Fmsdn.microsoft.com%2Fquery%2Fdev15.query%3FappId%3DDev15IDEF1%26l%3DEN-US%26k%3Dk(System.String.Split);k(TargetFrameworkMoniker-.NETFramework,Version%3Dv4.7.2);k(DevLang-csharp)%26rd%3Dtrue&view=netframework-4.7.2) usage.
  It seems annoying that in order to use a multi-character delimiter, you need to create
  a temporary one-element array of `string`s to pass in. Is there an equivalent to the C++
  `&singleElement` trick?
- Finally experimented with custom comparison functions for `Array.Sort()`. I'm
  probably re-sorting too frequently; the initiative ordering never changes, only
  the effective power.
- Had to resort to multiple `Regex` passes to extract the weakness/immunity of each
  group. I'm okay with that.
- I should have detected stalemates for part 2, but since I was manually
  binary-searching anyway, I didn't bother.

##### [Day 25](https://adventofcode.com/2018/day/25): 4D constellations
- Nested collections!
- No real issues; thankfully the Christmas Day puzzle was a light one.
