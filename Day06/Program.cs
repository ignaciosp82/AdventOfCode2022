var input = File.ReadAllText("input.txt");

var res1 = GetMarkerIndex(input, 4);

Console.WriteLine($"Part 1: {res1}");

var res2 = GetMarkerIndex(input, 14);

Console.WriteLine($"Part 2: {res2}");

int GetMarkerIndex(string signal, int markerLength)
{
    Queue<char> queue = new Queue<char>();

    for (int i = 0; i < signal.Length; i++)
    {
        var c = signal[i];
        if (queue.Count >= markerLength) queue.Dequeue();
        queue.Enqueue(c);
        if (queue.Distinct().Count() >= markerLength) return i + 1;
    }
    return -1;
}