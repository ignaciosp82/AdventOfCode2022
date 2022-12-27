using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var blueprints = input.Select(line =>
{
    var m = Regex.Match(line, @"Blueprint \d+: Each ore robot costs (?<oreore>\d+) ore. Each clay robot costs (?<clayore>\d+) ore\. Each obsidian robot costs (?<obsore>\d+) ore and (?<obsclay>\d+) clay\. Each geode robot costs (?<geoore>\d+) ore and (?<geoobs>\d+) obsidian\.");

    return new Blueprint
    {
        OreRobotCost = new Cost { Ore = Convert.ToInt32(m.Groups["oreore"].Value) },
        ClayRobotCost = new Cost { Ore = Convert.ToInt32(m.Groups["clayore"].Value) },
        ObsidianRobotCost = new Cost { Ore = Convert.ToInt32(m.Groups["obsore"].Value), Clay = Convert.ToInt32(m.Groups["obsclay"].Value) },
        GeodeRobotCost = new Cost { Ore = Convert.ToInt32(m.Groups["geoore"].Value), Obsidian = Convert.ToInt32(m.Groups["geoobs"].Value) }
    };
}).ToList();

var res1 = blueprints.Select((bp, i) => new { Max = GetMaxGeodes(bp, 24), Index = i + 1 }).Sum(s => s.Max * s.Index);

Console.WriteLine($"Part 1: {res1}");

var res2 = GetMaxGeodes(blueprints[0], 32) * GetMaxGeodes(blueprints[1], 32) * GetMaxGeodes(blueprints[2], 32);

Console.WriteLine($"Part 1: {res2}");

int GetMaxGeodes(Blueprint bp, int minutes)
{
    var pending = new Queue<State>();
    var visited = new HashSet<string>();

    pending.Enqueue(new State { CurrentMin = minutes, OreRobots = 1 });

    var max = 0;

    while (pending.Any())
    {
        var state = pending.Dequeue();

        if (state.CurrentMin == 0)
        {
            max = Math.Max(max, state.Mat.geo);
            continue;
        }

        var key = state.Key(bp);

        if (visited.Contains(key)) continue;

        visited.Add(key);

        if (bp.CanBuildGeodeRobot(state.Mat))
        {
            var newState = new State
            {
                CurrentMin = state.CurrentMin - 1,
                OreRobots = state.OreRobots,
                ClayRobots = state.ClayRobots,
                ObsiRobots = state.ObsiRobots,
                GeoRobots = state.GeoRobots + 1,
                Mat = (
                    state.Mat.ore + state.OreRobots - bp.GeodeRobotCost.Ore,
                    state.Mat.clay + state.ClayRobots - bp.GeodeRobotCost.Clay,
                    state.Mat.obsi + state.ObsiRobots - bp.GeodeRobotCost.Obsidian,
                    state.Mat.geo + state.GeoRobots
                )
            };
            pending.Enqueue(newState);
        }

        if (bp.CanBuildObsidianRobot(state.Mat))
        {
            var newState = new State
            {
                CurrentMin = state.CurrentMin - 1,
                OreRobots = state.OreRobots,
                ClayRobots = state.ClayRobots,
                ObsiRobots = state.ObsiRobots + 1,
                GeoRobots = state.GeoRobots,
                Mat = (
                    state.Mat.ore + state.OreRobots - bp.ObsidianRobotCost.Ore,
                    state.Mat.clay + state.ClayRobots - bp.ObsidianRobotCost.Clay,
                    state.Mat.obsi + state.ObsiRobots - bp.ObsidianRobotCost.Obsidian,
                    state.Mat.geo + state.GeoRobots
                )
            };
            pending.Enqueue(newState);
        }

        if (bp.CanBuildClayRobot(state.Mat))
        {
            var newState = new State
            {
                CurrentMin = state.CurrentMin - 1,
                OreRobots = state.OreRobots,
                ClayRobots = state.ClayRobots + 1,
                ObsiRobots = state.ObsiRobots,
                GeoRobots = state.GeoRobots,
                Mat = (
                    state.Mat.ore + state.OreRobots - bp.ClayRobotCost.Ore,
                    state.Mat.clay + state.ClayRobots - bp.ClayRobotCost.Clay,
                    state.Mat.obsi + state.ObsiRobots - bp.ClayRobotCost.Obsidian,
                    state.Mat.geo + state.GeoRobots
                )
            };
            pending.Enqueue(newState);
        }

        if (bp.CanBuildOreRobot(state.Mat))
        {
            var newState = new State
            {
                CurrentMin = state.CurrentMin - 1,
                OreRobots = state.OreRobots + 1,
                ClayRobots = state.ClayRobots,
                ObsiRobots = state.ObsiRobots,
                GeoRobots = state.GeoRobots,
                Mat = (
                    state.Mat.ore + state.OreRobots - bp.OreRobotCost.Ore,
                    state.Mat.clay + state.ClayRobots - bp.OreRobotCost.Clay,
                    state.Mat.obsi + state.ObsiRobots - bp.OreRobotCost.Obsidian,
                    state.Mat.geo + state.GeoRobots
                )
            };
            pending.Enqueue(newState);
        }

        var passState = new State
        {
            CurrentMin = state.CurrentMin - 1,
            OreRobots = state.OreRobots,
            ClayRobots = state.ClayRobots,
            ObsiRobots = state.ObsiRobots,
            GeoRobots = state.GeoRobots,
            Mat = (
                    state.Mat.ore + state.OreRobots,
                    state.Mat.clay + state.ClayRobots,
                    state.Mat.obsi + state.ObsiRobots,
                    state.Mat.geo + state.GeoRobots
                )
        };
        pending.Enqueue(passState);
    }

    return max;
}

class State
{
    public int CurrentMin { get; set; }
    public int OreRobots { get; set; }
    public int ClayRobots { get; set; }
    public int ObsiRobots { get; set; }
    public int GeoRobots { get; set; }

    public (int ore, int clay, int obsi, int geo) Mat { get; set; }

    public string Key(Blueprint bp) => $"[{CurrentMin}]({OreRobots},{ClayRobots},{ObsiRobots},{GeoRobots}){(GetOreCostKey(bp), GetClayCostKey(bp), GetObsiCostKey(bp), Mat.geo)}";

    private int GetOreCostKey(Blueprint bp) => Math.Min(Mat.ore, bp.MaxOreCost);

    private int GetClayCostKey(Blueprint bp) => Math.Min(Mat.clay, bp.MaxClayCost);

    private int GetObsiCostKey(Blueprint bp) => Math.Min(Mat.obsi, bp.MaxObsiCost);
}

class Blueprint
{
    public Cost OreRobotCost { get; init; }
    public Cost ClayRobotCost { get; init; }
    public Cost ObsidianRobotCost { get; init; }
    public Cost GeodeRobotCost { get; init; }

    private int? maxOreCost;
    public int MaxOreCost => (maxOreCost ?? (maxOreCost = new[] { OreRobotCost.Ore, ClayRobotCost.Ore, ObsidianRobotCost.Ore, GeodeRobotCost.Ore }.Max())).Value;

    private int? maxClayCost;
    public int MaxClayCost => (maxClayCost ?? (maxClayCost = new[] { OreRobotCost.Clay, ClayRobotCost.Clay, ObsidianRobotCost.Clay, GeodeRobotCost.Clay }.Max())).Value;

    private int? maxObsiCost;
    public int MaxObsiCost => (maxObsiCost ?? (maxObsiCost = new[] { OreRobotCost.Obsidian, ClayRobotCost.Obsidian, ObsidianRobotCost.Obsidian, GeodeRobotCost.Obsidian }.Max())).Value;

    public bool CanBuildOreRobot((int ore, int clay, int obsi, int geo) mat) =>
        OreRobotCost.Ore <= mat.ore && OreRobotCost.Clay <= mat.clay && OreRobotCost.Obsidian <= mat.obsi;

    public bool CanBuildClayRobot((int ore, int clay, int obsi, int geo) mat) =>
        ClayRobotCost.Ore <= mat.ore && ClayRobotCost.Clay <= mat.clay && ClayRobotCost.Obsidian <= mat.obsi;

    public bool CanBuildObsidianRobot((int ore, int clay, int obsi, int geo) mat) =>
        ObsidianRobotCost.Ore <= mat.ore && ObsidianRobotCost.Clay <= mat.clay && ObsidianRobotCost.Obsidian <= mat.obsi;

    public bool CanBuildGeodeRobot((int ore, int clay, int obsi, int geo) mat) =>
        GeodeRobotCost.Ore <= mat.ore && GeodeRobotCost.Clay <= mat.clay && GeodeRobotCost.Obsidian <= mat.obsi;
}

class Cost
{
    public int Ore { get; set; }
    public int Clay { get; set; }
    public int Obsidian { get; set; }
}
