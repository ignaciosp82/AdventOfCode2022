using System.Text;

var input = File.ReadAllLines("input.txt");

var snafuMap = new Dictionary<char, int> { { '2', 2 }, { '1', 1 }, { '0', 0 }, { '-', -1 }, { '=', -2 } };

var longMap = new Dictionary<int, char> { { 2, '2' }, { 1, '1' }, { 0, '0' }, { -1, '-' }, { -2, '=' } };

var total = input.Sum(SnafuToLong);

var snafu = LongToSnafu(total);
Console.WriteLine($"Part 1: {snafu}");

long SnafuToLong(string snafu)
{
    long result = 0;
    for (int i = 0; i < snafu.Length; i++)
    {
        result += snafuMap[snafu[i]] * (long)Math.Pow(5, snafu.Length - 1 - i);
    }
    return result;
}

string LongToSnafu(long value)
{
    var sb = new StringBuilder();
    for (; value > 0; value = (value + 2) / 5) sb.Append(longMap[(int)((value + 2) % 5) - 2]);
    return new string(sb.ToString().AsEnumerable().Reverse().ToArray());
}