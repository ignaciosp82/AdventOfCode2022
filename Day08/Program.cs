var input = File.ReadAllLines("input.txt");

var matrix = input.Select(x => x.ToArray()).ToArray();

var res1 = matrix.SelectMany((row, y) => row.Select((col, x) => CheckVisibility(matrix, x, y))).Count(v => v);

Console.WriteLine($"Part 1: {res1}");

var res2 = matrix.SelectMany((row, y) => row.Select((col, x) => GetScenicScore(matrix, x, y))).Max();

Console.WriteLine($"Part 2: {res2}");

int GetScenicScore(char[][] matrix, int x, int y)
{
    int left = 0, right = 0, up = 0, down = 0;

    var arr = matrix[y].Take(x).ToArray();
    for (int i = arr.Length - 1; i >= 0; i--)
    {
        left++;
        if (arr[i] >= matrix[y][x]) break;
    }

    arr = matrix[y].Skip(x + 1).ToArray();
    for (int i = 0; i < arr.Length; i++)
    {
        right++;
        if (arr[i] >= matrix[y][x]) break;
    }

    arr = matrix.Select(z => z[x]).ToArray().Take(y).ToArray();
    for (int i = arr.Length - 1; i >= 0; i--)
    {
        up++;
        if (arr[i] >= matrix[y][x]) break;
    }

    arr = matrix.Select(z => z[x]).ToArray().Skip(y + 1).ToArray();
    for (int i = 0; i < arr.Length; i++)
    {
        down++;
        if (arr[i] >= matrix[y][x]) break;
    }

    return left * right * up * down;
}

bool CheckVisibility(char[][] matrix, int x, int y) =>
    matrix[y].Take(x).All(i => i < matrix[y][x]) ||
    matrix[y].Skip(x + 1).All(i => i < matrix[y][x]) ||
    matrix.Select(z => z[x]).ToArray().Take(y).All(i => i < matrix[y][x]) ||
    matrix.Select(z => z[x]).ToArray().Skip(y + 1).All(i => i < matrix[y][x]);
