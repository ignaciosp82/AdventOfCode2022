var input = File.ReadAllText("input.txt");

var elves = input.
    Split("\n\n", StringSplitOptions.RemoveEmptyEntries).
    Select(s => s.
        Split("\n", StringSplitOptions.RemoveEmptyEntries).
        Select(c => Convert.ToInt32(c)).
        Sum());

int max = elves.
    OrderByDescending(s => s).
    First();

Console.WriteLine($"Part 1: {max}");

int max3 = elves.
    OrderByDescending(s => s).
    Take(3).
    Sum();

Console.WriteLine($"Part 2: {max3}");
