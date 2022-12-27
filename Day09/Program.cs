using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var movements = input.Select(line =>
{
    var m = Regex.Match(line, @"([UDRL]) (\d+)");
    var direction = m.Groups[1].Value.ToString();
    var n = Convert.ToInt32(m.Groups[2].Value);

    return (direction, n);
});

var res1 = CalculateNumberOfPositions(2);

Console.WriteLine($"Part 1: {res1}");

var res2 = CalculateNumberOfPositions(10);

Console.WriteLine($"Part 2: {res2}");

int CalculateNumberOfPositions(int nKnots)
{
    var knots = new (int, int)[nKnots];

    var positions = new HashSet<string>();
    positions.Add($"{knots[0].Item1} {knots[0].Item2}");

    foreach (var movement in movements)
    {
        for (int i = 0; i < movement.n; i++)
        {
            switch (movement.direction)
            {
                case "U":
                    knots[0].Item2++;
                    break;
                case "D":
                    knots[0].Item2--;
                    break;
                case "L":
                    knots[0].Item1--;
                    break;
                case "R":
                    knots[0].Item1++;
                    break;
            }

            for (int y = 0; y < knots.Length - 1; y++)
            {
                if (Math.Abs(knots[y].Item1 - knots[y + 1].Item1) <= 1 && Math.Abs(knots[y].Item2 - knots[y + 1].Item2) <= 1) continue;

                knots[y + 1].Item1 += knots[y].Item1 - knots[y + 1].Item1 >= 0 ? Math.Min(1, knots[y].Item1 - knots[y + 1].Item1) : Math.Max(-1, knots[y].Item1 - knots[y + 1].Item1);
                knots[y + 1].Item2 += knots[y].Item2 - knots[y + 1].Item2 >= 0 ? Math.Min(1, knots[y].Item2 - knots[y + 1].Item2) : Math.Max(-1, knots[y].Item2 - knots[y + 1].Item2);

                if (y == knots.Length - 2)
                {
                    positions.Add($"{knots[y + 1].Item1} {knots[y + 1].Item2}");
                }
            }
        }
    }
    return positions.Count;
}