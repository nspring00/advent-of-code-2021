namespace AdventOfCode2021.Challenges.Challenge12;

internal class Challenge12 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var caves = ParseInput(inputText);
        return Task1(caves);
    }

    public object RunTask2(string[] inputText)
    {
        var caves = ParseInput(inputText);
        return Task2(caves);
    }

    private static int Task1(IEnumerable<Cave> caves)
    {
        var start = caves.Single(x => x.Name.Equals("start"));
        var result = Dfs1(start, new List<Cave> { start });
        //Console.WriteLine(string.Join('\n', result));

        return result.Count;
    }

    private static int Task2(IEnumerable<Cave> caves)
    {
        var start = caves.Single(x => x.Name.Equals("start"));
        var result = Dfs2(start, new List<Cave> { start });
        //Console.WriteLine(string.Join('\n', result));

        return result.Count;
    }

    private static IList<string> Dfs1(Cave cave, ICollection<Cave> visited)
    {
        if (cave.Name.Equals("end"))
        {
            return new List<string>
            {
                "end"
            };
        }

        var results = new List<string>();
        foreach (var adjacentCave in cave.AdjacentCaves)
        {
            if (!adjacentCave.IsBigCave && visited.Contains(adjacentCave))
            {
                continue;
            }

            var visitedCopy = new List<Cave>(visited) { adjacentCave };

            var nextCaves = Dfs1(adjacentCave, visitedCopy);
            results.AddRange(nextCaves
                .Select(x => $"{cave.Name},{x}"));
        }

        return results;
    }

    private static IList<string> Dfs2(Cave cave, ICollection<Cave> visited, Cave? doubleVisitedCave = null)
    {
        if (cave.Name.Equals("end"))
        {
            return new List<string>
            {
                "end"
            };
        }

        var results = new List<string>();
        foreach (var adjacentCave in cave.AdjacentCaves)
        {
            if (adjacentCave.Name.Equals("start")) continue;

            var localDoubleVisitedCave = doubleVisitedCave;
            if (!adjacentCave.IsBigCave && visited.Contains(adjacentCave))
            {
                if (doubleVisitedCave != null) continue;

                localDoubleVisitedCave = adjacentCave;
            }

            var visitedCopy = new List<Cave>(visited) { adjacentCave };

            var nextCaves = Dfs2(adjacentCave, visitedCopy, localDoubleVisitedCave);
            if (nextCaves.Count == 0) continue;

            results.AddRange(nextCaves
                .Select(x => $"{cave.Name},{x}"));
        }

        return results;
    }

    private static IList<Cave> ParseInput(IEnumerable<string> inputText)
    {
        var inputParsed = inputText
            .Select(x => x
                .Split('-')
                .ToList())
            .ToList();


        var paths = inputParsed
            .Select(y => (Cave1: y.ElementAt(0), Cave2: y.ElementAt(1)))
            .ToList();

        var caves = inputParsed
            .SelectMany(x => x)
            .Distinct()
            .Select(x => new Cave(x))
            .ToList();

        foreach (var cave in caves)
        {
            var adjacent1 = paths
                .Where(x => x.Cave1.Equals(cave.Name))
                .Select(x => caves.Single(y => y.Name.Equals(x.Cave2)));
            var adjacent2 = paths
                .Where(x => x.Cave2.Equals(cave.Name))
                .Select(x => caves.Single(y => y.Name.Equals(x.Cave1)));
            cave.AdjacentCaves.AddRange(adjacent1.Union(adjacent2));
        }

        return caves;
    }
}

internal record Cave(string Name)
{
    public bool IsBigCave => Name[0] >= 'A' && Name[0] <= 'Z';
    public List<Cave> AdjacentCaves = new();
}