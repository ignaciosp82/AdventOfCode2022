using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var cubes = new HashSet<(int x, int y, int z)>(input.Select(line =>
{
    var m = Regex.Match(line, @"(?<x>\d+),(?<y>\d+),(?<z>\d+)");
    return (Convert.ToInt32(m.Groups["x"].Value), Convert.ToInt32(m.Groups["y"].Value), Convert.ToInt32(m.Groups["z"].Value));
}));

int CalculateSides(HashSet<(int x, int y, int z)> cubes)
{
    var sharedSides = 0;

    foreach (var cube in cubes)
    {
        if (cubes.Contains((cube.x + 1, cube.y, cube.z))) sharedSides++;
        if (cubes.Contains((cube.x - 1, cube.y, cube.z))) sharedSides++;
        if (cubes.Contains((cube.x, cube.y + 1, cube.z))) sharedSides++;
        if (cubes.Contains((cube.x, cube.y - 1, cube.z))) sharedSides++;
        if (cubes.Contains((cube.x, cube.y, cube.z + 1))) sharedSides++;
        if (cubes.Contains((cube.x, cube.y, cube.z - 1))) sharedSides++;
    }

    return cubes.Count * 6 - sharedSides;
}

var res1 = CalculateSides(cubes);

Console.WriteLine($"Part 1: {res1}");

var surfaceCubes = GetSurfaceCubes(cubes);

var res2 = CalculateSides(surfaceCubes);

Console.WriteLine($"Part 2: {res2}");


HashSet<(int x, int y, int z)> GetSurfaceCubes(HashSet<(int x, int y, int z)> cubes)
{
    (int x, int y, int z) min = (cubes.Min(c => c.x), cubes.Min(c => c.y), cubes.Min(c => c.z));
    (int x, int y, int z) max = (cubes.Max(c => c.x), cubes.Max(c => c.y), cubes.Max(c => c.z));

    var pending = new Queue<(int x, int y, int z)>();
    var visited = new HashSet<(int x, int y, int z)>();

    pending.Enqueue((0, 0, 0));

    while (true)
    {
        if (pending.Count == 0) break;

        var state = pending.Dequeue();

        if (cubes.Contains(state)) continue;

        if (visited.Contains(state)) continue;

        visited.Add(state);

        if (state.x < max.x) pending.Enqueue((state.x + 1, state.y, state.z));
        if (state.x > min.x) pending.Enqueue((state.x - 1, state.y, state.z));
        if (state.y < max.y) pending.Enqueue((state.x, state.y + 1, state.z));
        if (state.y > min.y) pending.Enqueue((state.x, state.y - 1, state.z));
        if (state.z < max.z) pending.Enqueue((state.x, state.y, state.z + 1));
        if (state.z > min.z) pending.Enqueue((state.x, state.y, state.z - 1));
    }

    var surfaceCubes = new HashSet<(int x, int y, int z)>();

    for (int x = min.x; x <= max.x; x++)
    {
        for (int y = min.y ; y <= max.y; y++)
        {
            for (int z = min.z; z <= max.z; z++)
            {
                if (!visited.Contains((x, y, z))) surfaceCubes.Add((x, y, z));
            }
        }
    }

    return surfaceCubes;
}
