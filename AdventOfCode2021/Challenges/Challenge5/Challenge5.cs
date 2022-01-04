namespace AdventOfCode2021.Challenges.Challenge5;

internal class Challenge5 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        return Task1(inputText);
    }

    public object RunTask2(string[] inputText)
    {
        return Task2(inputText);
    }

    private static int Task1(IEnumerable<string> input)
    {
        var points = input.Select(x => MapLine(x)).SelectMany(x => x).ToList();
        return CountMultiOccurringPoints(points);
    }

    private static int Task2(IEnumerable<string> input)
    {
        var points = input.Select(x => MapLine(x, true)).SelectMany(x => x).ToList();
        return CountMultiOccurringPoints(points);
    }

    private static int CountMultiOccurringPoints(IEnumerable<Point> points)
    {
        return points
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count())
            .Count(x => x.Value > 1);
    }

    private static IList<Point> MapLine(string inputRow, bool allowDiagonal = false)
    {
        var ends = inputRow
            .Split(" -> ")
            .Select(x => x
                .Split(",")
                .Select(int.Parse)
                .ToList())
            .Select(x => new Point(x.ElementAt(0), x.ElementAt(1)))
            .ToList();

        var first = ends.First();
        var last = ends.Last();

        List<Point> returnList = new();

        if (first.X == last.X)
        {
            if (first.Y > last.Y)
            {
                (first, last) = (last, first);
            }

            for (var y = first.Y; y <= last.Y; y++)
            {
                returnList.Add(new Point(first.X, y));
            }
        }
        else if (first.Y == last.Y)
        {
            if (first.X > last.X)
            {
                (first, last) = (last, first);
            }

            for (var x = first.X; x <= last.X; x++)
            {
                returnList.Add(new Point(x, first.Y));
            }
        }
        else if (allowDiagonal)
        {
            // Diagonal 45 Degrees
            if (first.X > last.X)
            {
                (first, last) = (last, first);
            }

            var deltaY = first.Y < last.Y ? 1 : -1;
            for (var x = first.X; x <= last.X; x++)
            {
                returnList.Add(new Point(x, first.Y + (x - first.X) * deltaY));
            }
        }

        return returnList;
    }

    private record Point(int X, int Y);
}