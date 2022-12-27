using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");

var inputMonkeys = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

var monkeysPart1 = new Dictionary<int, Monkey>();
var monkeysPart2 = new Dictionary<int, Monkey>();

foreach (var inputMonkey in inputMonkeys)
{
    var inputMonkeyProperties = inputMonkey.Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

    var m = Regex.Match(inputMonkeyProperties[0], @"Monkey (\d+):");
    var number = Convert.ToInt32(m.Groups[1].Value);
    monkeysPart1[number] = new Monkey { Number = number };
    monkeysPart2[number] = new Monkey { Number = number };

    m = Regex.Match(inputMonkeyProperties[1], @"Starting items: (?:(\d+),? ?)+");
    monkeysPart1[number].Items = m.Groups[1].Captures.Select(c => Convert.ToInt64(c.Value)).ToList();
    monkeysPart2[number].Items = new List<long>(monkeysPart1[number].Items);

    m = Regex.Match(inputMonkeyProperties[2], @"Operation: new = old ([\+\*]) (old|\d+)");
    var op = m.Groups[1].Value;
    var right = m.Groups[2].Value;
    Func<long, long> operation;
    if (op == "+")
    {
        if (right == "old") operation = (old) => old + old;
        else operation = (old) => old + Convert.ToInt64(right);
    }
    else
    {
        if (right == "old") operation = (old) => old * old;
        else operation = (old) => old * Convert.ToInt64(right);
    }
    monkeysPart1[number].Operation = operation;
    monkeysPart2[number].Operation = operation;

    m = Regex.Match(inputMonkeyProperties[3], @"Test: divisible by (\d+)");
    monkeysPart1[number].Test = Convert.ToInt32(m.Groups[1].Value);
    monkeysPart2[number].Test = monkeysPart1[number].Test;

    m = Regex.Match(inputMonkeyProperties[4], @"If true: throw to monkey (\d+)");
    monkeysPart1[number].IfTrue = Convert.ToInt32(m.Groups[1].Value);
    monkeysPart2[number].IfTrue = monkeysPart1[number].IfTrue;

    m = Regex.Match(inputMonkeyProperties[5], @"If false: throw to monkey (\d+)");
    monkeysPart1[number].IfFalse = Convert.ToInt32(m.Groups[1].Value);
    monkeysPart2[number].IfFalse = monkeysPart1[number].IfFalse;
}

var module = 1;
foreach (var m in monkeysPart1.Values) module *= m.Test;

CalculateWorries(monkeysPart1, 20, false);
var mostActives1 = monkeysPart1.OrderByDescending(m => m.Value.N).Take(2).ToArray();
var res1 = mostActives1[0].Value.N * mostActives1[1].Value.N;
Console.WriteLine($"Part 1: {res1}");

CalculateWorries(monkeysPart2, 10000, true, module);
var mostActives2 = monkeysPart2.OrderByDescending(m => m.Value.N).Take(2).ToArray();
var res2 = mostActives2[0].Value.N * mostActives2[1].Value.N;
Console.WriteLine($"Part 2: {res2}");

void CalculateWorries(Dictionary<int, Monkey> monkeys, int n, bool isPart2, long module = 0)
{
    for (int i = 0; i < n; i++)
    {
        for (int m = 0; m < monkeys.Count; m++)
        {
            var monkey = monkeys[m];
            var itemsCopy = new List<long>(monkey.Items);
            for (var ii = 0; ii < itemsCopy.Count; ii++)
            {
                var item = itemsCopy[ii];
                var newItem = isPart2 ? (monkey.Operation(item) % module) : (monkey.Operation(item) / 3);
                if (newItem % monkey.Test == 0) monkeys[monkey.IfTrue].Items.Add(newItem);
                else monkeys[monkey.IfFalse].Items.Add(newItem);
                monkey.Items.Remove(item);
                monkey.N++;
            }
        }
    }
}

class Monkey
{
    public int Number { get; set; }
    public List<long> Items { get; set; }
    public Func<long, long> Operation { get; set; }
    public int Test { get; set; }
    public int IfTrue { get; set; }
    public int IfFalse { get; set; }
    public long N { get; set; } = 0;
}