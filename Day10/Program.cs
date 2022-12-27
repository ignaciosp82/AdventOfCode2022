using System.Text;
using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

int cycle = 0;
int x = 1;
int[] cycles = new int[] { 20, 60, 100, 140, 180, 220 };
int i = 0;
int sum = 0;
StringBuilder sb = new StringBuilder();
foreach (var line in input)
{
    var m = Regex.Match(line, "noop");
    if (m.Success)
    {
        cycle++;

        var pixelPosition = ((cycle - 1) % 40);
        if (pixelPosition == x || pixelPosition == x - 1 || pixelPosition == x + 1) sb.Append("#");
        else sb.Append(".");

        if (i < cycles.Length && cycle > cycles[i]) sum += x * cycles[i++];
    }

    m = Regex.Match(line, @"addx (-?\d+)");
    if (m.Success)
    {
        cycle++;

        var pixelPosition = (cycle - 1) % 40;
        if (pixelPosition == x || pixelPosition == x - 1 || pixelPosition == x + 1) sb.Append("#");
        else sb.Append(".");

        cycle++;

        pixelPosition = (cycle - 1) % 40;
        if (pixelPosition == x || pixelPosition == x - 1 || pixelPosition == x + 1) sb.Append("#");
        else sb.Append(".");

        if (i < cycles.Length && cycle >= cycles[i]) sum += x * cycles[i++];

        x += Convert.ToInt32(m.Groups[1].Value);
    }
}

Console.WriteLine($"Part 1: {sum}");

Console.WriteLine("Part 2:");
var res2 = sb.ToString();
Console.WriteLine(res2.Substring(0, 40));
Console.WriteLine(res2.Substring(40, 40));
Console.WriteLine(res2.Substring(80, 40));
Console.WriteLine(res2.Substring(120, 40));
Console.WriteLine(res2.Substring(160, 40));
Console.WriteLine(res2.Substring(200, 40));

