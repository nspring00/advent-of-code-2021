namespace AdventOfCode2021.Challenges.Challenge15;

internal class Challenge15 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var field = inputText.Select((row) =>
            row.ToCharArray().Select((val) => int.Parse(val.ToString())).ToArray()).ToArray();

        return FindShortestPath(field);
    }

    public object RunTask2(string[] inputText)
    {
        var fieldTemp = inputText.Select((row) =>
            row.ToCharArray().Select((val) => int.Parse(val.ToString())).ToArray()).ToArray();

        var field = new int[fieldTemp.Length * 5][];
        for (var i = 0; i < 5 * fieldTemp.Length; i++)
        {
            field[i] = new int[fieldTemp[0].Length * 5];
        }

        for (var y = 0; y < fieldTemp.Length; y++)
        {
            for (var x = 0; x < fieldTemp[0].Length; x++)
            {
                var vertex = fieldTemp[y][x];

                for (var yOffset = 0; yOffset < 5; yOffset++)
                {
                    for (var xOffset = 0; xOffset < 5; xOffset++)
                    {
                        var newValue = vertex + xOffset + yOffset;
                        var newX = xOffset * fieldTemp[0].Length + x;
                        var newY = yOffset * fieldTemp.Length + y;
                        if (newValue > 9)
                        {
                            newValue -= 9;
                        }

                        field[newY][newX] = newValue;
                    }
                }
            }
        }

        return FindShortestPath(field);
    }

    private static int FindShortestPath(IReadOnlyList<int[]> field)
    {
        var queue = new PriorityQueue<(int X, int Y), int>();
        queue.Enqueue((0, 0), 0);

        var bestValues = new Dictionary<(int X, int Y), int>(field.Count * field[0].Length)
        {
            {
                (0, 0), 0
            }
        };

        while (queue.TryDequeue(out var point, out var value))
        {
            var neighbors = GetNeighbors(field, point);
            foreach (var neighbor in neighbors)
            {
                var newNeighborValue = value + field[neighbor.Y][neighbor.X];
                if (bestValues.ContainsKey(neighbor) && newNeighborValue >= bestValues[neighbor])
                {
                    continue;
                }

                bestValues[neighbor] = newNeighborValue;
                queue.Enqueue(neighbor, newNeighborValue);
            }
        }

        return bestValues[(field.Count - 1, field[0].Length - 1)];
    }

    private static IList<(int X, int Y)> GetNeighbors(IReadOnlyList<int[]> field, (int X, int Y) v)
    {
        var (x, y) = v;
        var neighbors = new List<(int, int)>();

        if (x - 1 >= 0)
        {
            neighbors.Add((y, x - 1));
        }

        if (y - 1 >= 0)
        {
            neighbors.Add((y - 1, x));
        }

        if (x + 1 < field.Count)
        {
            neighbors.Add((y, x + 1));
        }

        if (y + 1 < field[0].Length)
        {
            neighbors.Add((y + 1, x));
        }

        return neighbors;
    }
}