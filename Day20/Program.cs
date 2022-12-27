var input = File.ReadAllLines("input.txt");

var list = input.Select(line => new Number(Convert.ToInt32(line))).ToList();

var listPart1 = new List<Number>(list);
Decrypt(new List<Number>(listPart1), listPart1);
var res1 = GetResult(listPart1);
Console.WriteLine($"Part 1: {res1}");

var listPart2 = new List<Number>(list);
listPart2.ForEach(n => n.Value *= 811589153);
var baseListPart2 = new List<Number>(listPart2);
for (int i = 0; i < 10; i++) Decrypt(baseListPart2, listPart2);
var res2 = GetResult(listPart2);
Console.WriteLine($"Part 2: {res2}");

void Decrypt(List<Number> baseList, List<Number> list)
{
    for (int i = 0; i < list.Count; i++)
    {
        var number = baseList[i];
        Move(list, number);
    }
}

void Move(List<Number> list, Number number)
{
    var index = list.IndexOf(number);
    var movements = (int)(number.Value % (list.Count - 1));

    if (movements > 0)
    {
        list.RemoveAt(index);
        list.Insert((index + movements) % list.Count, number);
    }

    if (movements < 0)
    {
        list.RemoveAt(index);
        movements = movements + list.Count;
        var newIndex = (index + movements) % list.Count;
        if (newIndex == 0) newIndex = list.Count;
        list.Insert(newIndex, number);
    }
}

long GetResult(List<Number> list)
{
    var indexOfZero = list.FindIndex(n => n.Value == 0);

    return
        list[(indexOfZero + 1000) % list.Count].Value +
        list[(indexOfZero + 2000) % list.Count].Value +
        list[(indexOfZero + 3000) % list.Count].Value;
}

class Number
{
    public long Value { get; set; }
    public Number(long value) => Value = value;
}

