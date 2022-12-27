var input = File.ReadAllLines("input.txt");

var scenario = new HashSet<(int, int)>();
(int, int) sandInitPos = (500, 0);

foreach (var line in input)
{
    var coordinates = line.Split(" -> ").Select(c =>
    {
        var cs = c.Split(",");
        return (Convert.ToInt32(cs[0]), Convert.ToInt32(cs[1]));
    }).ToList();

    for (int i = 0; i < coordinates.Count - 1; i++)
    {
        var coordinate1 = coordinates[i];
        var coordinate2 = coordinates[i + 1];

        scenario.UnionWith(GetLine(coordinate1, coordinate2));
    }
}

var res1 = GetMaxSand(scenario);
Console.WriteLine($"Part 1: {res1}");

var ground = scenario.Max(c => c.Item2) + 2;

for (int i = sandInitPos.Item1 - ground - 1; i < +sandInitPos.Item1 + ground + 1; i++)
{
    scenario.Add((i, ground));
}

var res2 = GetMaxSand(scenario);
Console.WriteLine($"Part 2: {res2}");

int GetMaxSand(HashSet<(int, int)> originalScenario)
{
    var scenario = new HashSet<(int, int)>(originalScenario);
    var blocks = scenario.Count;

    (int, int) sandPos;

    do
    {
        sandPos = sandInitPos;

        while (!IsGoingToTheAbyss(sandPos, scenario))
        {
            if (CanMoveDown(sandPos, scenario))
            {
                sandPos.Item2++;
            }
            else if (CanMoveDownLeft(sandPos, scenario))
            {
                sandPos.Item1--;
                sandPos.Item2++;
            }
            else if (CanMoveDownRight(sandPos, scenario))
            {
                sandPos.Item1++;
                sandPos.Item2++;
            }
            else
            {
                scenario.Add(sandPos);
                break;
            }
        }

    } while (!IsGoingToTheAbyss(sandPos, scenario) && sandPos != sandInitPos);

    return scenario.Count - blocks;
}

bool CanMoveDown((int, int) sandPos, HashSet<(int, int)> scenario) => !scenario.Contains((sandPos.Item1, sandPos.Item2 + 1));

bool CanMoveDownLeft((int, int) sandPos, HashSet<(int, int)> scenario) => !scenario.Contains((sandPos.Item1 - 1, sandPos.Item2 + 1));

bool CanMoveDownRight((int, int) sandPos, HashSet<(int, int)> scenario) => !scenario.Contains((sandPos.Item1 + 1, sandPos.Item2 + 1));

bool IsGoingToTheAbyss((int, int) sandPos, HashSet<(int, int)> scenario) => !scenario.Any(c => c.Item1 == sandPos.Item1 && c.Item2 > sandPos.Item2);

IEnumerable<(int, int)> GetLine((int, int) coordinate1, (int, int) coordinate2)
{
    if (coordinate1.Item1 != coordinate2.Item1)
    {
        for (int i = Math.Min(coordinate1.Item1, coordinate2.Item1); i <= Math.Max(coordinate1.Item1, coordinate2.Item1); i++)
        {
            yield return (i, coordinate1.Item2);
        }
    }
    else if (coordinate1.Item2 != coordinate2.Item2)
    {
        for (int i = Math.Min(coordinate1.Item2, coordinate2.Item2); i <= Math.Max(coordinate1.Item2, coordinate2.Item2); i++)
        {
            yield return (coordinate1.Item1, i);
        }
    }
}