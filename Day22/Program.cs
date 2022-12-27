using System.Text.RegularExpressions;

const char OPEN_TILE = '.';
const char SOLID_WALL = '#';
const char EMPTY = ' ';

const int LEFT = 2;
const int RIGHT = 0;
const int UP = 3;
const int DOWN = 1;

const char CLOCKWISE = 'R';
const char COUNTERCLOCKWISE = 'L';

var input = File.ReadAllText("input.txt");

var parts = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

var rows = parts[0].Split("\n", StringSplitOptions.RemoveEmptyEntries);
var maxLength = rows.Max(r => r.Length);
rows = rows.Select(r => r.PadRight(maxLength, ' ')).ToArray();

var steps = Regex.Matches(parts[1].Trim() + "X", @"(\d+)([LRX])").Select(m => new Step { N = Convert.ToInt32(m.Groups[1].Value), DirectionChange = m.Groups[2].Value[0] });

(int x, int y) MoveRight((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x + 1, newPos.y);
        if (newPos.x >= rows[newPos.y].Length || rows[newPos.y][newPos.x] == EMPTY) newPos = (Math.Min(rows[pos.y].IndexOf(OPEN_TILE), rows[pos.y].IndexOf(SOLID_WALL)), pos.y);
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return old;
    }
    return newPos;
}

(int x, int y) MoveLeft((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x - 1, newPos.y);
        if (newPos.x < 0 || rows[newPos.y][newPos.x] == EMPTY) newPos = (Math.Max(rows[pos.y].LastIndexOf(OPEN_TILE), rows[pos.y].LastIndexOf(SOLID_WALL)), pos.y);
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return old;
    }
    return newPos;
}

(int x, int y) MoveDown((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    var row = rows.Select(r => r[pos.x]).ToList();
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x, newPos.y + 1);
        if (newPos.y >= rows.Length || rows[newPos.y][newPos.x] == EMPTY) newPos = (pos.x, Math.Min(row.IndexOf(OPEN_TILE), row.IndexOf(SOLID_WALL)));
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return old;
    }
    return newPos;
}

(int x, int y) MoveUp((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    var row = rows.Select(r => r[pos.x]).ToList();
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x, newPos.y - 1);
        if (newPos.y < 0 || rows[newPos.y][newPos.x] == EMPTY) newPos = (pos.x, Math.Max(row.LastIndexOf(OPEN_TILE), row.LastIndexOf(SOLID_WALL)));
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return old;
    }
    return newPos;
}

var sectors = new Dictionary<int, (int x, int y)>
{
    { 1, (50, 0) },
    { 2, (100, 0) },
    { 3, (50, 50) },
    { 4, (0, 100) },
    { 5, (50, 100) },
    { 6, (0, 150) }
};

var map = new Dictionary<(int, int), Func<(int x, int y), ((int x, int y) pos, int dir)>>
{
    { (1, LEFT), (pos) => ((0, 149 - pos.y), RIGHT) },
    { (1, UP), (pos) => ((0, pos.x + 100), RIGHT) },
    { (2, RIGHT), (pos) => ((99, 149 - pos.y), LEFT) },
    { (2, UP), (pos) => ((pos.x - 100, 199), UP) },
    { (2, DOWN), (pos) => ((99, pos.x - 50), LEFT) },
    { (3, RIGHT), (pos) => ((pos.y + 50, 49), UP) },
    { (3, LEFT), (pos) => ((pos.y - 50, 100), DOWN) },
    { (4, LEFT), (pos) => ((50, 149 - pos.y), RIGHT) },
    { (4, UP), (pos) => ((50, pos.x + 50), RIGHT) },
    { (5, RIGHT), (pos) => ((149, 149 - pos.y), LEFT) },
    { (5, DOWN), (pos) => ((49, pos.x + 100), LEFT) },
    { (6, RIGHT), (pos) => ((pos.y - 100, 149), UP) },
    { (6, LEFT), (pos) => ((pos.y - 100, 0), DOWN) },
    { (6, DOWN), (pos) => ((pos.x + 100, 0), DOWN) }
};

int GetSector((int x, int y) pos) => sectors.Single(kv => kv.Value.x <= pos.x && kv.Value.x + 50 > pos.x && kv.Value.y <= pos.y && kv.Value.y + 50 > pos.y).Key;

((int x, int y), int dir) MoveRightCube((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x + 1, newPos.y);
        if (newPos.x >= rows[newPos.y].Length || rows[newPos.y][newPos.x] == EMPTY) {
            var cube = map[(GetSector(old), RIGHT)](newPos);
            if (rows[cube.pos.y][cube.pos.x] == SOLID_WALL) return (old, RIGHT);
            switch (cube.dir)
            {
                case RIGHT: return MoveRightCube(cube.pos, n - i - 1);
                case LEFT: return MoveLeftCube(cube.pos, n - i - 1);
                case DOWN: return MoveDownCube(cube.pos, n - i - 1);
                case UP: return MoveUpCube(cube.pos, n - i - 1);
            }
        }
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return (old, RIGHT);
    }
    return (newPos, RIGHT);
}

((int x, int y), int dir) MoveLeftCube((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x - 1, newPos.y);
        if (newPos.x < 0 || rows[newPos.y][newPos.x] == EMPTY)
        {
            var cube = map[(GetSector(old), LEFT)](newPos);
            if (rows[cube.pos.y][cube.pos.x] == SOLID_WALL) return (old, LEFT);
            switch (cube.dir)
            {
                case RIGHT: return MoveRightCube(cube.pos, n - i - 1);
                case LEFT: return MoveLeftCube(cube.pos, n - i - 1);
                case DOWN: return MoveDownCube(cube.pos, n - i - 1);
                case UP: return MoveUpCube(cube.pos, n - i - 1);
            }
        }
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return (old, LEFT);
    }
    return (newPos, LEFT);
}

((int x, int y), int dir) MoveDownCube((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x, newPos.y + 1);
        if (newPos.y >= rows.Length || rows[newPos.y][newPos.x] == EMPTY)
        {
            var cube = map[(GetSector(old), DOWN)](newPos);
            if (rows[cube.pos.y][cube.pos.x] == SOLID_WALL) return (old, DOWN);
            switch (cube.dir)
            {
                case RIGHT: return MoveRightCube(cube.pos, n - i - 1);
                case LEFT: return MoveLeftCube(cube.pos, n - i - 1);
                case DOWN: return MoveDownCube(cube.pos, n - i - 1);
                case UP: return MoveUpCube(cube.pos, n - i - 1);
            }
        }
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return (old, DOWN);
    }
    return (newPos, DOWN);
}

((int x, int y), int dir) MoveUpCube((int x, int y) pos, int n)
{
    (int x, int y) newPos = pos;
    for (int i = 0; i < n; i++)
    {
        var old = newPos;
        newPos = (newPos.x, newPos.y - 1);
        if (newPos.y < 0 || rows[newPos.y][newPos.x] == EMPTY)
        {
            var cube = map[(GetSector(old), UP)](newPos);
            if (rows[cube.pos.y][cube.pos.x] == SOLID_WALL) return (old, UP);
            switch (cube.dir)
            {
                case RIGHT: return MoveRightCube(cube.pos, n - i - 1);
                case LEFT: return MoveLeftCube(cube.pos, n - i - 1);
                case DOWN: return MoveDownCube(cube.pos, n - i - 1);
                case UP: return MoveUpCube(cube.pos, n - i - 1);
            }
        }
        if (rows[newPos.y][newPos.x] == SOLID_WALL) return (old, UP);
    }
    return (newPos, UP);
}

(int x, int y) pos = (rows[0].IndexOf(OPEN_TILE), 0);

int direction = RIGHT;

foreach (var step in steps)
{
    switch (direction)
    {
        case RIGHT:
            pos = MoveRight(pos, step.N);
            if (step.DirectionChange == CLOCKWISE) direction = DOWN;
            else if (step.DirectionChange == COUNTERCLOCKWISE) direction = UP;
            break;
        case LEFT:
            pos = MoveLeft(pos, step.N);
            if (step.DirectionChange == CLOCKWISE) direction = UP;
            else if (step.DirectionChange == COUNTERCLOCKWISE) direction = DOWN;
            break;
        case DOWN:
            pos = MoveDown(pos, step.N);
            if (step.DirectionChange == CLOCKWISE) direction = LEFT;
            else if (step.DirectionChange == COUNTERCLOCKWISE) direction = RIGHT;
            break;
        case UP:
            pos = MoveUp(pos, step.N);
            if (step.DirectionChange == CLOCKWISE) direction = RIGHT;
            else if (step.DirectionChange == COUNTERCLOCKWISE) direction = LEFT;
            break;
    }
}

var res1 = (pos.y + 1) * 1000 + (pos.x + 1) * 4 + direction;

Console.WriteLine($"Part 1: {res1}");

pos = (rows[0].IndexOf(OPEN_TILE), 0);

direction = RIGHT;

foreach (var step in steps)
{
    switch (direction)
    {
        case RIGHT:
            (pos, direction) = MoveRightCube(pos, step.N);
            break;
        case LEFT:
            (pos, direction) = MoveLeftCube(pos, step.N);
            break;
        case DOWN:
            (pos, direction) = MoveDownCube(pos, step.N);
            break;
        case UP:
            (pos, direction) = MoveUpCube(pos, step.N);
            break;
    }

    if (step.DirectionChange == CLOCKWISE) direction = (direction + 5) % 4;
    else if (step.DirectionChange == COUNTERCLOCKWISE) direction = (direction + 3) % 4;
}

var res2 = (pos.y + 1) * 1000 + (pos.x + 1) * 4 + direction;

Console.WriteLine($"Part 2: {res2}");

class Step
{
    public int N { get; set; }
    public char DirectionChange { get; set; }
    public override string ToString() => $"{DirectionChange}{N}";
}

