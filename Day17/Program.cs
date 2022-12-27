var input = File.ReadAllText("input.txt").Trim();

long GetHeight(long nRocks, char[] directions)
{
    var scenario = new HashSet<(int x, int y)>();
    var cycles = new Dictionary<string, (long, int)>();
    var patternSize = 50;

    long calculatedHeight = 0;
    int maxY = 0;

    int dirIndex = 0;

    for (long i = 0; i < nRocks; i++)
    {
        var rock = Rock.New(i);

        var pos = (2, scenario.DefaultIfEmpty((-1, -1)).Max(p => p.Item2) + 4);

        bool isResting = false;
        do
        {
            var direction = directions[dirIndex++ % directions.Length];
            if (direction == '<') pos = rock.MoveLeft(pos, scenario);
            else if (direction == '>') pos = rock.MoveRight(pos, scenario);

            pos = rock.MoveDown(pos, scenario, out isResting);

        } while (!isResting);

        scenario.UnionWith(rock.GetAbsolutePoints(pos));

        maxY = scenario.Max(p => p.Item2);
        if (maxY >= patternSize)
        {
            var rocksInCycle = scenario.
                Where(p => maxY - p.y < patternSize).
                Select(c => ((int x, int y))(c.x, maxY - c.y)).
                OrderBy(p => p.y).
                ThenBy(p => p.x);

            var key = $"{dirIndex % directions.Length}_{string.Join("", rocksInCycle)}";

            if (cycles.ContainsKey(key))
            {
                var cycle = cycles[key];
                var nCycleRocks = i - cycle.Item1;
                var nCycles = (nRocks - i) / nCycleRocks;
                calculatedHeight += (maxY - cycle.Item2) * nCycles;
                i += nCycleRocks * nCycles;
            }

            cycles[key] = (i, maxY);
        }
    }

    return maxY + calculatedHeight + 1;
}

var res1 = GetHeight(2022, input.ToCharArray());

Console.WriteLine($"Part 1: {res1}");

var res2 = GetHeight(1000000000000, input.ToCharArray());

Console.WriteLine($"Part 2: {res2}");

abstract class Rock
{
    public static Rock New(long index) => (index % 5) switch
    {
        0 => new Line(),
        1 => new Plus(),
        2 => new L(),
        3 => new I(),
        4 => new Square(),
        _ => throw new Exception()
    };

    public abstract (int x, int y)[] Points { get; }

    public (int x, int y)[] GetAbsolutePoints((int x, int y) relative) => Points.Select(p => (relative.x + p.x, relative.y + p.y)).ToArray();

    internal (int x, int y) MoveRight((int x, int y) relativePos, HashSet<(int x, int y)> scenario) =>
        GetAbsolutePoints(relativePos).Any(p => scenario.Contains((p.x + 1, p.y)) || p.x == 6) ? relativePos : (relativePos.x + 1, relativePos.y);

    internal (int x, int y) MoveLeft((int x, int y) relativePos, HashSet<(int x, int y)> scenario) =>
        GetAbsolutePoints(relativePos).Any(p => scenario.Contains((p.x - 1, p.y)) || p.x == 0) ? relativePos : (relativePos.x - 1, relativePos.y);

    internal (int x, int y) MoveDown((int x, int y) relativePos, HashSet<(int x, int y)> scenario, out bool isResting)
    {
        isResting = GetAbsolutePoints(relativePos).Any(p => scenario.Contains((p.x, p.y - 1)) || p.y == 0);
        return isResting ? relativePos : (relativePos.x, relativePos.y - 1);
    }
}

class Line : Rock
{
    public override (int x, int y)[] Points { get; } = new[] { (0, 0), (1, 0), (2, 0), (3, 0) };
}

class Plus : Rock
{
    public override (int x, int y)[] Points { get; } = new[] { (0, 1), (1, 1), (2, 1), (1, 2), (1, 0) };
}

class L : Rock
{
    public override (int x, int y)[] Points { get; } = new[] { (0, 0), (1, 0), (2, 0), (2, 1), (2, 2) };
}

class I : Rock
{
    public override (int x, int y)[] Points { get; } = new[] { (0, 0), (0, 1), (0, 2), (0, 3) };
}

class Square : Rock
{
    public override (int x, int y)[] Points { get; } = new[] { (0, 0), (0, 1), (1, 0), (1, 1) };
}