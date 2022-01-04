namespace AdventOfCode2021.Challenges.Challenge9;

internal class Challenge9 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var input = ParseInput(inputText);
        return Task1(input);
    }

    public object RunTask2(string[] inputText)
    {
        var input = ParseInput(inputText);
        return Task2(input);
    }

    private static int[][] ParseInput(IEnumerable<string> inputText) =>
        inputText.Select(x => x.ToCharArray().Select(y => int.Parse(y.ToString())).ToArray()).ToArray();

    private static int Task1(IReadOnlyList<int[]> input)
    {
        var maxI = input.Count;
        var maxJ = input[0].Length;

        var sum = 0;

        for (var i = 0; i < maxI; i++)
        {
            for (var j = 0; j < maxJ; j++)
            {
                var value = input[i][j];
                if (i > 0 && input[i - 1][j] <= value)
                {
                    continue;
                }

                if (i + 1 < maxI && input[i + 1][j] <= value)
                {
                    continue;
                }

                if (j > 0 && input[i][j - 1] <= value)
                {
                    continue;
                }

                if (j + 1 < maxJ && input[i][j + 1] <= value)
                {
                    continue;
                }

                sum += value + 1;
            }
        }

        return sum;
    }

    private static int Task2(IReadOnlyList<int[]> input)
    {
        var maxI = input.Count;
        var maxJ = input[0].Length;

        input = input.Select(row => row.Select(x => x == 9 ? 0 : -1).ToArray()).ToArray();
        var counter = 1;

        for (var i = 0; i < maxI; i++)
        {
            for (var j = 0; j < maxJ; j++)
            {
                var value = input[i][j];

                if (value == 0) continue;

                if (value == -1)
                {
                    value = counter++;
                    input[i][j] = value;
                }

                if (i > 0)
                {
                    var otherValue = input[i - 1][j];
                    if (value != otherValue && otherValue > 0)
                    {
                        input = input.Select(row => row.Select(x => x == otherValue ? value : x).ToArray()).ToArray();
                    }
                }

                if (i + 1 < maxI)
                {
                    if (input[i + 1][j] == -1)
                    {
                        input[i + 1][j] = value;
                    }
                }

                if (j > 0)
                {
                    var otherValue = input[i][j - 1];
                    if (value != otherValue && otherValue > 0)
                    {
                        input = input.Select(row => row.Select(x => x == otherValue ? value : x).ToArray()).ToArray();
                    }
                }

                if (j + 1 < maxJ)
                {
                    var otherValue = input[i][j + 1];
                    if (otherValue == 0) continue;
                    if (otherValue == -1)
                    {
                        input[i][j + 1] = value;
                    }
                    else if (value != otherValue)
                    {
                        input = input.Select(row => row.Select(x => x == otherValue ? value : x).ToArray()).ToArray();
                    }
                }
            }
        }

        var counts = input.SelectMany(x => x).Where(x => x > 0).GroupBy(x => x).Select(x => x.Count())
            .OrderByDescending(x => x).Take(3);

        return counts.Aggregate(1, (current, count) => current * count);
    }
}