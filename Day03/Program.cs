var input = File.ReadAllLines("input.txt");

int GetPriority(char itemType)
{
    if (itemType >= 'a' && itemType <= 'z') return itemType - 'a' + 1;
    if (itemType >= 'A' && itemType <= 'Z') return itemType - 'A' + 27;

    return 0;
}

var priority1 = input.Sum(i =>
{
    var itemType = i.Substring(0, i.Length / 2).
        Join(i.Substring(i.Length / 2), o => o, i => i, (o, i) => o).
        Distinct().
        Single();

    return GetPriority(itemType);
});

Console.WriteLine($"Part 1: {priority1}");

var priority2 = input.
    Select((item, index) => new { Item = item, Group = index / 3 }).
    GroupBy(ig => ig.Group).Sum(items =>
    {
        var arrItems = items.
            Select(i => i.Item).
            ToArray();

        var itemType = arrItems[0].
            Join(arrItems[1], o => o, i => i, (o, i) => o).
            Join(arrItems[2], o => o, i => i, (o, i) => o).
            Distinct().
            Single();

        return GetPriority(itemType);
    });

Console.WriteLine($"Part 2: {priority2}");
