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