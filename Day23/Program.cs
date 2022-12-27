var input = File.ReadAllLines("input.txt");

const char ELF = '#';

var initialElves = new HashSet<(int x, int y)>();

for (int y = 0; y < input.Length; y++)
{
    var line = input[y];
    for (int x = 0; x < input.Length; x++)
    {
        var position = line[x];
        if (position == ELF)
        {
            initialElves.Add((x, y));
        }
    }
}

var elves = new HashSet<(int x, int y)>(initialElves);

MoveElves(elves, 10);

var minX = elves.Min(e => e.x);
var maxX = elves.Max(e => e.x);
var minY = elves.Min(e => e.y);
var maxY = elves.Max(e => e.y);

var res1 = (maxX - minX + 1) * (maxY - minY + 1) - elves.Count;

Console.WriteLine($"Part 1: {res1}");

int res2 = MoveElves(initialElves);

Console.WriteLine($"Part 2: {res2}");

int MoveElves(HashSet<(int x, int y)> elves, int rounds = int.MaxValue)
{
    for (int i = 0; i < rounds; i++)
    {

        var movesCounter = new Dictionary<(int x, int y), int>();
        var moves = new Dictionary<(int x, int y), (int x, int y)>();

        foreach (var elf in elves)
        {
            var adyacents = new[] {
                (elf.x - 1, elf.y - 1),
                (elf.x - 1, elf.y),
                (elf.x - 1, elf.y + 1),
                (elf.x, elf.y - 1),
                (elf.x, elf.y + 1),
                (elf.x + 1, elf.y - 1),
                (elf.x + 1, elf.y),
                (elf.x + 1, elf.y + 1)
            };

            if (adyacents.Intersect(elves).Any())
            {
                var dIndex = i % 4;
                var exit = false;

                for (int d = dIndex; !exit && d < dIndex + 4; d++)
                {
                    switch (d % 4)
                    {
                        case 0:
                            if (!elves.Contains((elf.x - 1, elf.y - 1)) && !elves.Contains((elf.x, elf.y - 1)) && !elves.Contains((elf.x + 1, elf.y - 1)))
                            {
                                var move = (elf.x, elf.y - 1);
                                if (movesCounter.ContainsKey(move)) movesCounter[move]++; else movesCounter[move] = 1;
                                moves[elf] = move;
                                exit = true;
                            }
                            break;
                        case 1:
                            if (!elves.Contains((elf.x - 1, elf.y + 1)) && !elves.Contains((elf.x, elf.y + 1)) && !elves.Contains((elf.x + 1, elf.y + 1)))
                            {
                                var move = (elf.x, elf.y + 1);
                                if (movesCounter.ContainsKey(move)) movesCounter[move]++; else movesCounter[move] = 1;
                                moves[elf] = move;
                                exit = true;
                            }
                            break;
                        case 2:
                            if (!elves.Contains((elf.x - 1, elf.y - 1)) && !elves.Contains((elf.x - 1, elf.y)) && !elves.Contains((elf.x - 1, elf.y + 1)))
                            {
                                var move = (elf.x - 1, elf.y);
                                if (movesCounter.ContainsKey(move)) movesCounter[move]++; else movesCounter[move] = 1;
                                moves[elf] = move;
                                exit = true;
                            }
                            break;
                        case 3:
                            if (!elves.Contains((elf.x + 1, elf.y - 1)) && !elves.Contains((elf.x + 1, elf.y)) && !elves.Contains((elf.x + 1, elf.y + 1)))
                            {
                                var move = (elf.x + 1, elf.y);
                                if (movesCounter.ContainsKey(move)) movesCounter[move]++; else movesCounter[move] = 1;
                                moves[elf] = move;
                                exit = true;
                            }
                            break;
                    }
                }
            }
        }

        if (!moves.Any()) return i + 1;

            foreach (var move in moves)
        {
            if (movesCounter[move.Value] == 1)
            {
                elves.Remove(move.Key);
                elves.Add(move.Value);
            }
        }
    }

    return -1;
}