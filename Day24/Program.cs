var input = File.ReadAllLines("input.txt");

const char LEFT = '<';
const char RIGHT = '>';
const char UP = '^';
const char DOWN = 'v';
const char GROUND = '.';

var scenario = new Scenario();

scenario.Width = input[0].Length;
scenario.Height = input.Length;

scenario.StartPos = (input.First().IndexOf(GROUND), 0);
scenario.EndPos = (input.Last().IndexOf(GROUND), input.Length - 1);

for (int y = 0; y < input.Length; y++)
{
    var line = input[y];
    for (int x = 0; x < line.Length; x++)
    {
        var tile = line[x];
        switch (tile)
        {
            case LEFT:
                scenario.Lefts.Add((x, y));
                break;
            case RIGHT:
                scenario.Rights.Add((x, y));
                break;
            case UP:
                scenario.Ups.Add((x, y));
                break;
            case DOWN:
                scenario.Downs.Add((x, y));
                break;
        }
    }
}

var res = GetMinMins(scenario);

Console.WriteLine($"Part 1: {res.part1}");
Console.WriteLine($"Part 2: {res.part2}");

(int part1, int part2) GetMinMins(Scenario scenario)
{
    var blizzardsCache = new Dictionary<int, (HashSet<(int x, int y)> lefts, HashSet<(int x, int y)> rights, HashSet<(int x, int y)> ups, HashSet<(int x, int y)> downs)>
    {
        {0, (new HashSet<(int x, int y)>(scenario.Lefts), new HashSet<(int x, int y)>(scenario.Rights), new HashSet<(int x, int y)>(scenario.Ups), new HashSet<(int x, int y)>(scenario.Downs)) }
    };

    var pending = new Queue<State>();
    var visited = new HashSet<string>();

    pending.Enqueue(new State { Min = 0, Pos = scenario.StartPos });

    int phase = 0;
    int part1 = 0;

    while (pending.Any())
    {
        var state = pending.Dequeue();

        if (state.Pos == scenario.EndPos)
        {
            if (phase == 0) part1 = state.Min;
            if (phase++ < 2)
            {
                var lastStart = scenario.StartPos;
                scenario.StartPos = scenario.EndPos;
                scenario.EndPos = lastStart;
                visited.Clear();
                pending.Clear();
                pending.Enqueue(state);
                continue;
            }
            else return (part1, state.Min);
        }

        var blizzards = blizzardsCache[state.Min];
        if (blizzards.lefts.Contains(state.Pos) || blizzards.rights.Contains(state.Pos) || blizzards.ups.Contains(state.Pos) || blizzards.downs.Contains(state.Pos)) continue;

        var key = state.Key;
        if (visited.Contains(key)) continue;
        visited.Add(key);

        if (!blizzardsCache.ContainsKey(state.Min + 1))
        {
            var auxLeft = new HashSet<(int x, int y)>();
            foreach (var left in blizzards.lefts)
            {
                if (left.x - 1 != 0) auxLeft.Add((left.x - 1, left.y));
                else auxLeft.Add((scenario.Width - 2, left.y));
            }

            var auxRight = new HashSet<(int x, int y)>();
            foreach (var right in blizzards.rights)
            {
                if (right.x + 1 != scenario.Width - 1) auxRight.Add((right.x + 1, right.y));
                else auxRight.Add((1, right.y));
            }

            var auxUp = new HashSet<(int x, int y)>();
            foreach (var up in blizzards.ups)
            {
                if (up.y - 1 != 0) auxUp.Add((up.x, up.y - 1));
                else auxUp.Add((up.x, scenario.Height - 2));
            }

            var auxDown = new HashSet<(int x, int y)>();
            foreach (var down in blizzards.downs)
            {
                if (down.y + 1 != scenario.Height - 1) auxDown.Add((down.x, down.y + 1));
                else auxDown.Add((down.x, 1));
            }

            blizzardsCache[state.Min + 1] = (auxLeft, auxRight, auxUp, auxDown);
        }

        var newBlizzards = blizzardsCache[state.Min + 1];

        var canMove = CanMove(state.Pos, newBlizzards, scenario.Width, scenario.Height, scenario.StartPos, scenario.EndPos);

        if (canMove.left) pending.Enqueue(new State { Min = state.Min + 1, Pos = (state.Pos.x - 1, state.Pos.y) });
        if (canMove.right) pending.Enqueue(new State { Min = state.Min + 1, Pos = (state.Pos.x + 1, state.Pos.y) });
        if (canMove.up) pending.Enqueue(new State { Min = state.Min + 1, Pos = (state.Pos.x, state.Pos.y - 1) });
        if (canMove.down) pending.Enqueue(new State { Min = state.Min + 1, Pos = (state.Pos.x, state.Pos.y + 1) });
        pending.Enqueue(new State { Min = state.Min + 1, Pos = state.Pos });
    }
    return (-1, -1);
}

(bool left, bool right, bool up, bool down) CanMove((int x, int y) pos, (HashSet<(int x, int y)> lefts, HashSet<(int x, int y)> rights, HashSet<(int x, int y)> ups, HashSet<(int x, int y)> downs) blizzards, int width, int height, (int x, int y) startPos, (int x, int y) endPos)
{
    (int x, int y) toLeft = (pos.x - 1, pos.y);
    (int x, int y) toRight = (pos.x + 1, pos.y);
    (int x, int y) toUp = (pos.x, pos.y - 1);
    (int x, int y) toDown = (pos.x, pos.y + 1);
    return (
        ValidPos(toLeft, startPos, endPos, width, height) && !blizzards.lefts.Contains(toLeft) && !blizzards.rights.Contains(toLeft) && !blizzards.ups.Contains(toLeft) && !blizzards.downs.Contains(toLeft),
        ValidPos(toRight, startPos, endPos, width, height) && !blizzards.lefts.Contains(toRight) && !blizzards.rights.Contains(toRight) && !blizzards.ups.Contains(toRight) && !blizzards.downs.Contains(toRight),
        ValidPos(toUp, startPos, endPos, width, height) && !blizzards.lefts.Contains(toUp) && !blizzards.rights.Contains(toUp) && !blizzards.ups.Contains(toUp) && !blizzards.downs.Contains(toUp),
        ValidPos(toDown, startPos, endPos, width, height) && !blizzards.lefts.Contains(toDown) && !blizzards.rights.Contains(toDown) && !blizzards.ups.Contains(toDown) && !blizzards.downs.Contains(toDown)
    );
}

bool ValidPos((int x, int y) pos, (int x, int y) start, (int x, int y) end, int width, int height) => pos == start || pos == end || (pos.x > 0 && pos.y > 0 && pos.x < width - 1 && pos.y < height - 1);

class State
{
    public int Min { get; set; }
    public (int x, int y) Pos { get; set; }
    public string Key => $"{Min}{Pos}";
}

class Scenario
{
    public HashSet<(int x, int y)> Lefts { get; set; } = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> Rights { get; set; } = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> Ups { get; set; } = new HashSet<(int x, int y)>();
    public HashSet<(int x, int y)> Downs { get; set; } = new HashSet<(int x, int y)>();
    public int Width { get; set; }
    public int Height { get; set; }
    public (int x, int y) StartPos { get; set; }
    public (int x, int y) EndPos { get; set; }
}
