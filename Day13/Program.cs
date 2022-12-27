var input = File.ReadAllText("input.txt");

const string DIV1 = "[[2]]";
const string DIV2 = "[[6]]";

var pairs = input.Split("\n\n", StringSplitOptions.RemoveEmptyEntries);

List<Pair> pairss = new List<Pair> { Pair.Parse(DIV1, new Dictionary<string, Pair>()), Pair.Parse(DIV2, new Dictionary<string, Pair>()) };

int res1 = 0;
for (int i = 0; i < pairs.Length; i++)
{
    var par = pairs[i];

    var p = par.Split("\n", StringSplitOptions.RemoveEmptyEntries);

    Pair p1 = Pair.Parse(p[0].Trim(), new Dictionary<string, Pair>());
    Pair p2 = Pair.Parse(p[1].Trim(), new Dictionary<string, Pair>());

    pairss.Add(p1);
    pairss.Add(p2);

    var ordered = Pair.Ordered(p1, p2);

    if (ordered == -1) res1 += i + 1;
}

Console.WriteLine($"Part 1: {res1}");

pairss.Sort(new Comp());

var res2 = pairss.
    Select((p, i) => new { P = p.ToString(), I = i + 1 }).
    Where(p => p.P == DIV1 || p.P == DIV2).
    Aggregate(1, (a, b) => a * b.I);

Console.WriteLine($"Part 2: {res2}");

class Comp : IComparer<Pair>
{
    public int Compare(Pair? x, Pair? y) => Pair.Ordered(x, y);
}

class Pair
{
    public int? Int { get; set; }
    public List<Pair> Lis { get; set; }

    public override string ToString()
    {
        return $"{(Int.HasValue ? Int : "[" + string.Join(",", Lis) + "]")}";
    }

    public static int Ordered(Pair a, Pair b)
    {
        if (a.Int.HasValue && b.Int.HasValue) return a.Int.Value < b.Int.Value ? -1 : (a.Int.Value > b.Int.Value ? 1 : 0);

        if (a.Lis != null && b.Lis != null)
        {
            for (int i = 0; i < Math.Max(a.Lis.Count, b.Lis.Count); i++)
            {
                if (i >= a.Lis.Count) return -1;
                if (i >= b.Lis.Count) return 1;
                var o = Ordered(a.Lis[i], b.Lis[i]);
                if (o == 0) continue;
                else return o;
            }
            return 0;
        }

        if (a.Lis != null && b.Int != null) return Ordered(a, new Pair { Lis = new List<Pair> { b } });
        if (b.Lis != null && a.Int != null) return Ordered(new Pair { Lis = new List<Pair> { a } }, b);

        return 1;
    }

    public static Pair Parse(string str, Dictionary<string, Pair> dic)
    {
        Pair currentPar = new Pair();

        if (str.Count() == 2)
        {
            return new Pair { Lis = new List<Pair>() };
        }

        if (str.Count(s => s == '[') == 1)
        {
            return new Pair { Lis = str.Substring(1, str.Length - 2).Split(',').Select(s => s.StartsWith("P") ? dic[s] : new Pair { Int = Convert.ToInt32(s) }).ToList() };
        }

        Queue<Pair> queue = new Queue<Pair>();

        int lastIndex = 0;
        for (int i = 0; i < str.Length; i++)
        {
            var c = str[i];

            if (c == '[')
            {
                lastIndex = i;
            }
            else if (c == ']')
            {
                Pair p = Parse(str.Substring(lastIndex, i - lastIndex + 1), dic);
                var id = $"P{dic.Count}";
                dic[id] = p;
                str = $"{str.Substring(0, lastIndex)}{id}{str.Substring(i + 1)}";
                break;
            }
        }
        return Parse(str, dic);
    }
}