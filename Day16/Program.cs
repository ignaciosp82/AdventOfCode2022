using System.Text.RegularExpressions;

var input = File.ReadAllLines("input.txt");

var valves = new Dictionary<string, Valve>();

foreach (var line in input)
{
    var m = Regex.Match(line, @"Valve ([A-Z]+) has flow rate=(\d+); tunnels? leads? to valves? ([A-Z]+)(?:, ([A-Z]+))*");

    var name = m.Groups[1].Value;
    var flowRate = Convert.ToInt32(m.Groups[2].Value);
    var valveNamesToGo = new List<string>();
    valveNamesToGo.Add(m.Groups[3].Value);

    if (m.Groups.Count > 4)
    {
        valveNamesToGo.AddRange(m.Groups[4].Captures.Select(c => c.Value));
    }

    var valve = GetOrCreateValve(valves, name);

    foreach (var valveNameToGo in valveNamesToGo)
    {
        var valveToGo = GetOrCreateValve(valves, valveNameToGo);
        valve.Valves.Add(valveToGo);
    }

    valve.FlowRate = flowRate;
}

var res1 = MaxPressure(valves);

Console.WriteLine($"Part 1: {res1}");

var res2 = MaxPressureWithMyFriend(valves);

Console.WriteLine($"Part 2: {res2}");

int MaxPressure(Dictionary<string, Valve> valves)
{
    Queue<State> pending = new Queue<State>();
    var visited = new Dictionary<string, int>();

    pending.Enqueue(new State { CurrentValve = valves["AA"], CurrentMinute = 30 });

    int max = 0;

    while (true)
    {
        if (pending.Count == 0) break;

        var state = pending.Dequeue();

        var key = $"{state.Key}{state.CurrentMinute}";

        if (!visited.ContainsKey(key) || visited[key] < state.Value) visited[key] = state.Value;
        else continue;

        max = Math.Max(max, state.Value);

        if (state.Path.Count == valves.Count(v => v.Value.FlowRate != 0)) continue;

        var newPath = new List<(Valve, int)>(state.Path);
        var newMinute = state.CurrentMinute;

        if (newMinute == 0) continue;

        if (!state.Path.Select(p => p.Item1).Contains(state.CurrentValve) && state.CurrentValve.FlowRate != 0)
        {
            newPath.Add((state.CurrentValve, --newMinute));
        }

        if (newMinute == 0) continue;

        foreach (var valveTo in state.CurrentValve.Valves)
        {
            var newState = new State { CurrentValve = valveTo, CurrentMinute = newMinute - 1, Path = newPath };
            pending.Enqueue(newState);
            newState = new State { CurrentValve = valveTo, CurrentMinute = state.CurrentMinute - 1, Path = state.Path };
            pending.Enqueue(newState);
        }
    }
    return max;
}

int MaxPressureWithMyFriend(Dictionary<string, Valve> valves)
{
    Queue<StateWithMyFriend> pending = new Queue<StateWithMyFriend>();
    var visited = new Dictionary<string, int>();

    pending.Enqueue(new StateWithMyFriend { CurrentValveMe = valves["AA"], CurrentValveElephant = valves["AA"], CurrentMinute = 26 });

    int max = 0;

    while (true)
    {
        if (pending.Count == 0) break;

        var state = pending.Dequeue();

        var key = $"{state.Key}{state.CurrentMinute}";

        if (!visited.ContainsKey(key) || visited[key] < state.Value) visited[key] = state.Value;
        else continue;

        max = Math.Max(max, state.Value);

        if (state.Path.Count == valves.Count(v => v.Value.FlowRate != 0)) continue;

        var newPath = new List<(Valve, int)>(state.Path);

        if (state.CurrentMinute == 0) continue;

        var newMinuteMe = state.CurrentMinute;
        var newMinuteElephant = state.CurrentMinute;

        if (!newPath.Select(p => p.Item1).Contains(state.CurrentValveMe) && state.CurrentValveMe.FlowRate != 0)
        {
            newPath.Add((state.CurrentValveMe, --newMinuteMe));
        }

        if (!newPath.Select(p => p.Item1).Contains(state.CurrentValveElephant) && state.CurrentValveElephant.FlowRate != 0)
        {
            newPath.Add((state.CurrentValveElephant, --newMinuteElephant));
        }

        if (newMinuteMe == 0 && newMinuteElephant == 0) continue;

        if (newMinuteMe == newMinuteElephant)
        {
            if (newMinuteMe == state.CurrentMinute)
            {
                foreach (var valveToMe in state.CurrentValveMe.Valves)
                {
                    foreach (var valveToElephant in state.CurrentValveElephant.Valves)
                    {
                        var newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = valveToElephant, CurrentMinute = state.CurrentMinute - 1, Path = state.Path };
                        pending.Enqueue(newState);
                    }
                }
            }
            else
            {
                foreach (var valveToMe in state.CurrentValveMe.Valves)
                {
                    foreach (var valveToElephant in state.CurrentValveElephant.Valves)
                    {
                        var newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = valveToElephant, CurrentMinute = newMinuteMe - 1, Path = newPath };
                        pending.Enqueue(newState);
                        newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = valveToElephant, CurrentMinute = state.CurrentMinute - 1, Path = state.Path };
                        pending.Enqueue(newState);
                        newState = new StateWithMyFriend { CurrentValveMe = state.CurrentValveMe, CurrentValveElephant = valveToElephant, CurrentMinute = state.CurrentMinute - 1, Path = newPath.Except(new[] { (state.CurrentValveElephant, newMinuteElephant) }).ToList() };
                        pending.Enqueue(newState);
                        newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = state.CurrentValveElephant, CurrentMinute = state.CurrentMinute - 1, Path = newPath.Except(new[] { (state.CurrentValveMe, newMinuteMe) }).ToList() };
                        pending.Enqueue(newState);
                    }
                }
            }
        }
        else if (newMinuteMe < newMinuteElephant)
        {
            foreach (var valveToElephant in state.CurrentValveElephant.Valves)
            {
                var newState = new StateWithMyFriend { CurrentValveMe = state.CurrentValveMe, CurrentValveElephant = valveToElephant, CurrentMinute = state.CurrentMinute - 1, Path = newPath };
                pending.Enqueue(newState);
            }
            foreach (var valveToMe in state.CurrentValveMe.Valves)
            {
                foreach (var valveToElephant in state.CurrentValveElephant.Valves)
                {
                    var newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = valveToElephant, CurrentMinute = state.CurrentMinute - 1, Path = state.Path };
                    pending.Enqueue(newState);
                }
            }
        }
        else
        {
            foreach (var valveToMe in state.CurrentValveMe.Valves)
            {
                var newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = state.CurrentValveElephant, CurrentMinute = state.CurrentMinute - 1, Path = newPath };
                pending.Enqueue(newState);
            }
            foreach (var valveToMe in state.CurrentValveMe.Valves)
            {
                foreach (var valveToElephant in state.CurrentValveElephant.Valves)
                {
                    var newState = new StateWithMyFriend { CurrentValveMe = valveToMe, CurrentValveElephant = valveToElephant, CurrentMinute = state.CurrentMinute - 1, Path = state.Path };
                    pending.Enqueue(newState);
                }
            }
        }
    }

    return max;
}

Valve GetOrCreateValve(Dictionary<string, Valve> valves, string name) =>
    valves.ContainsKey(name) ? valves[name] : (valves[name] = new Valve { Name = name });

class State
{
    public Valve CurrentValve { get; set; }
    public List<(Valve, int)> Path { get; set; } = new List<(Valve, int)>();
    public int CurrentMinute { get; set; }
    public int Value => Path.Select(p => p.Item1.FlowRate * p.Item2).DefaultIfEmpty(0).Sum();
    public bool ImAnElephant { get; set; } = false;
    public string Key => $"{CurrentValve.Name}";
    public override string ToString() => $"({CurrentValve.Name}, {CurrentMinute}) -> [{string.Join(", ", Path.Select(p => $"({p.Item1.Name}, {p.Item2})"))}] = {Value}";
}

class StateWithMyFriend
{
    public Valve CurrentValveMe { get; set; }
    public Valve CurrentValveElephant { get; set; }
    public List<(Valve, int)> Path { get; set; } = new List<(Valve, int)>();
    public int CurrentMinute { get; set; }
    public int Value => Path.Select(p => p.Item1.FlowRate * p.Item2).DefaultIfEmpty(0).Sum();
    public string Key => $"{CurrentValveMe.Name}_{CurrentValveElephant.Name}";
}

class Valve
{
    public string Name { get; set; }
    public int FlowRate { get; set; }
    public List<Valve> Valves { get; set; } = new List<Valve>();

    public override string ToString() => $"{Name}: {FlowRate} -> {string.Join(", ", Valves.Select(v => v.Name))}";
}