using System.Text.RegularExpressions;

var inputs = File.ReadAllLines("input.txt");

MyDir root = new MyDir("ROOT", null);
root.Dirs.Add(new MyDir("/", root));

MyDir currentDir = root;

foreach (var input in inputs)
{
    var m = Regex.Match(input, @"cd \.\.");
    if (m.Success)
    {
        currentDir = currentDir.Parent;
        continue;
    }

    m = Regex.Match(input, @"\$ cd (.+)");
    if (m.Success)
    {
        var dirName = m.Groups[1].ToString();
        currentDir = currentDir.Dirs.Single(d => d.Name == dirName);
        continue;
    }

    m = Regex.Match(input, @"(\d+) (.+)");
    if (m.Success)
    {
        var fileSize = Convert.ToInt32(m.Groups[1].ToString());
        var fileName = m.Groups[2].ToString();
        currentDir.Files.Add(new MyFile(fileName, fileSize));
        continue;
    }

    m = Regex.Match(input, @"dir (.+)");
    if (m.Success)
    {
        var dirName = m.Groups[1].ToString();
        currentDir.Dirs.Add(new MyDir(dirName, currentDir));
        continue;
    }
}

var sumSizes = root.GetAllSubDirs().Where(d => d.Size < 100000).Sum(d => d.Size);

Console.WriteLine($"Part 1: {sumSizes}");

var sizeNeeded = 30000000 - (70000000 - root.Size);

var sizeToDelete = root.GetAllSubDirs().Where(d => d.Size >= sizeNeeded).OrderBy(d => d.Size).First().Size;

Console.WriteLine($"Part 2: {sizeToDelete}");

class MyFile
{
    public string Name { get; set; }
    public int Size { get; set; }

    public MyFile(string name, int size)
    {
        Name = name;
        Size = size;
    }
}

class MyDir
{
    public string Name { get; set; }
    public MyDir Parent { get; set; }
    public List<MyFile> Files { get; set; } = new List<MyFile>();
    public List<MyDir> Dirs { get; set; } = new List<MyDir>();
    public int Size => Dirs.Sum(d => d.Size) + Files.Sum(d => d.Size);

    public MyDir(string name, MyDir parent = null)
    {
        Name = name;
        Parent = parent;
    }

    public IEnumerable<MyDir> GetAllSubDirs() => Dirs.Concat(Dirs.SelectMany(d => d.GetAllSubDirs()));
}