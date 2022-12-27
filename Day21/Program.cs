using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

const string ROOT = "root";
const string HUMN = "humn";
var monkeys = new Dictionary<string, Monkey>();

foreach (var line in input)
{
    var m = Regex.Match(line, @"([a-z]+): ([a-z]+) ([\*\+-/]) ([a-z]+)");
    if (m.Success)
    {
        var monkey = GetOrCreate(monkeys, m.Groups[1].Value);
        monkey.Left = GetOrCreate(monkeys, m.Groups[2].Value);
        monkey.Right = GetOrCreate(monkeys, m.Groups[4].Value);
        monkey.Operation = m.Groups[3].Value[0];
        continue;
    }

    m = Regex.Match(line, @"([a-z]+): (\d+)");
    if (m.Success)
    {
        var monkey = GetOrCreate(monkeys, m.Groups[1].Value);
        monkey.Value = Convert.ToInt32(m.Groups[2].Value);
        monkey.Operation = 'v';
        continue;
    }
}

var rootMonkey = monkeys[ROOT];
var res1 = rootMonkey.Calculate();
Console.WriteLine($"Part 1: {res1}");

var value = (rootMonkey.Left.FromMonkey(HUMN) ? rootMonkey.Right : rootMonkey.Left).Calculate() * 2;
Reduce(monkeys, HUMN);
var reverseMonkeys = Reverse(monkeys, HUMN, ROOT, value);

var humnMonkey = reverseMonkeys[HUMN];
var res2 = humnMonkey.Calculate();
Console.WriteLine($"Part 2: {res2}");

Dictionary<string, Monkey> Reverse(Dictionary<string, Monkey> monkeys, string humnName, string rootName, long rootValue)
{
    var reverseMonkeys = new Dictionary<string, Monkey>();

    foreach (var monkey in monkeys.Values)
    {
        if (monkey.Name == humnName) continue;

        if (monkey.Operation == 'v')
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Operation = 'v';
            reverseMonkey.Value = monkey.Value;
        }
        else if (monkey.Operation == '+' && (monkey.Left.Operation != 'v' || monkey.Left.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Operation = '-';
        }
        else if (monkey.Operation == '-' && (monkey.Left.Operation != 'v' || monkey.Left.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Operation = '+';
        }
        else if (monkey.Operation == '*' && (monkey.Left.Operation != 'v' || monkey.Left.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Operation = '/';
        }
        else if (monkey.Operation == '/' && (monkey.Left.Operation != 'v' || monkey.Left.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Operation = '*';
        }
        else if (monkey.Operation == '+' && (monkey.Right.Operation != 'v' || monkey.Right.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Operation = '-';
        }
        else if (monkey.Operation == '-' && (monkey.Right.Operation != 'v' || monkey.Right.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Operation = '-';
        }
        else if (monkey.Operation == '*' && (monkey.Right.Operation != 'v' || monkey.Right.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Operation = '/';
        }
        else if (monkey.Operation == '/' && (monkey.Right.Operation != 'v' || monkey.Right.Name == humnName))
        {
            var reverseMonkey = GetOrCreate(reverseMonkeys, monkey.Right.Name);
            reverseMonkey.Left = GetOrCreate(reverseMonkeys, monkey.Left.Name);
            reverseMonkey.Right = GetOrCreate(reverseMonkeys, monkey.Name);
            reverseMonkey.Operation = '/';
        }
    }

    var root = reverseMonkeys[rootName];
    root.Operation = 'v';
    root.Value = rootValue;

    return reverseMonkeys;
}

void Reduce(Dictionary<string, Monkey> monkeys, string name)
{
    foreach (var monkey in monkeys.Values)
    {
        if (!monkey.Left?.FromMonkey(name) == true && !monkey.Right?.FromMonkey(name) == true)
        {
            monkey.Value = monkey.Calculate();
            monkey.Left = null;
            monkey.Right = null;
            monkey.Operation = 'v';
        }
    }
}

Monkey GetOrCreate(Dictionary<string, Monkey> monkeys, string monkeyName) =>
    !monkeys.ContainsKey(monkeyName) ?
        monkeys[monkeyName] = new Monkey { Name = monkeyName } :
        monkeys[monkeyName];

class Monkey
{
    public string Name { get; set; }
    public Monkey Left { get; set; }
    public Monkey Right { get; set; }
    public char Operation { get; set; }
    public long Value { get; set; }

    public bool FromMonkey(string name) => Name == name || Left?.FromMonkey(name) == true || Right?.FromMonkey(name) == true;

    public long Calculate() => Operation switch
    {
        '+' => Left.Calculate() + Right.Calculate(),
        '-' => Left.Calculate() - Right.Calculate(),
        '*' => Left.Calculate() * Right.Calculate(),
        '/' => Left.Calculate() / Right.Calculate(),
        'v' => Value,
        _ => throw new Exception()
    };

    public override string ToString() => Operation == 'v' ? $"{Name}: {Value}" : $"{Name}: {Left.Name} {Operation} {Right.Name}";
}
