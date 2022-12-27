using System.Text.RegularExpressions;

var input = File.ReadAllText("input.txt");

var inputParts = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

var inputRows = inputParts[0].Split("\n").SkipLast(1).Reverse().ToList();

var nColunmns = (inputRows[0].Length + 1) / 4;

var moves = inputParts[1].
    Split("\n", StringSplitOptions.RemoveEmptyEntries).
    Select(move =>
    {
        var m = Regex.Match(move, @"move (\d+) from (\d+) to (\d+)");
        return (
            Convert.ToInt32(m.Groups[1].Value),
            Convert.ToInt32(m.Groups[2].Value),
            Convert.ToInt32(m.Groups[3].Value)
        );
    }).
    ToList();

var part1Stacks = GetStacks(nColunmns, inputRows);

moves.ForEach(m => OneByOne(part1Stacks, m));

var res1 = new string(part1Stacks.Select(y => y.Peek()).ToArray());

Console.WriteLine($"Part 1: {res1}");

var part2Stacks = GetStacks(nColunmns, inputRows);

moves.ForEach(m => StackByStack(part2Stacks, m));

var res2 = new string(part2Stacks.Select(y => y.Peek()).ToArray());

Console.WriteLine($"Part 2: {res2}");

List<Stack<char>> GetStacks(int nColumns, List<string> inputRows)
{
    var stacks = Enumerable.Range(0, nColunmns).Select(i => new Stack<char>()).ToList();

    foreach (var inputRow in inputRows)
    {
        for (int i = 0; i < nColunmns; i++)
        {
            var stack = stacks[i];
            var c = inputRow[(i * 4) + 1];
            if (c != ' ') stack.Push(c);
        }
    }

    return stacks;
}

void OneByOne(List<Stack<char>> stacks, (int n, int from, int to) move)
{
    var aux = new List<char>();
    for (int i = 0; i < move.n; i++)
    {
        aux.Add(stacks[move.from - 1].Pop());
    }
    for (int i = 0; i < move.n; i++)
    {
        stacks[move.to - 1].Push(aux[i]);
    }
}

void StackByStack(List<Stack<char>> stacks, (int n, int from, int to) move)
{
    var aux = new List<char>();
    for (int i = 0; i < move.n; i++)
    {
        aux.Add(stacks[move.from - 1].Pop());
    }
    for (int i = move.n - 1; i >= 0; i--)
    {
        stacks[move.to - 1].Push(aux[i]);
    }
}

