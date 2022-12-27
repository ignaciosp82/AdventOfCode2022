var input = File.ReadAllLines("input.txt");

var part1Values = new Dictionary<string, int>
{
    { "A X", 1 + 3 },
    { "A Y", 2 + 6 },
    { "A Z", 3 + 0 },
    { "B X", 1 + 0 },
    { "B Y", 2 + 3 },
    { "B Z", 3 + 6 },
    { "C X", 1 + 6 },
    { "C Y", 2 + 0 },
    { "C Z", 3 + 3 },
};

var part2Values = new Dictionary<string, int>
{
    { "A X", 3 + 0 },
    { "A Y", 1 + 3 },
    { "A Z", 2 + 6 },
    { "B X", 1 + 0 },
    { "B Y", 2 + 3 },
    { "B Z", 3 + 6 },
    { "C X", 2 + 0 },
    { "C Y", 3 + 3 },
    { "C Z", 1 + 6 },
};

var res1 = input.Sum(i => part1Values[i]);

Console.WriteLine($"Part 1: {res1}");

var res2 = input.Sum(i => part2Values[i]);

Console.WriteLine($"Part 2: {res2}");