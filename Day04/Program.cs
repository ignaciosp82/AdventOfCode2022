using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var ranges = input.Select(x =>
{
    var m = Regex.Match(x, @"(\d+)-(\d+),(\d+)-(\d+)");
    return (
        Convert.ToInt32(m.Groups[1].Value),
        Convert.ToInt32(m.Groups[2].Value),
        Convert.ToInt32(m.Groups[3].Value),
        Convert.ToInt32(m.Groups[4].Value)
    );
});

var fullyContains = ranges.Where(FullyContains).Count();

Console.WriteLine($"Part 1: {fullyContains}");

var overlap = ranges.Where(Overlap).Count();

Console.WriteLine($"Part 2: {overlap}");

static bool Overlap((int a1, int b1, int a2, int b2) r) => !(r.b1 < r.a2 || r.a1 > r.b2);

static bool FullyContains((int a1, int b1, int a2, int b2) r) => r.a1 >= r.a2 && r.b1 <= r.b2 || r.a2 >= r.a1 && r.b2 <= r.b1;