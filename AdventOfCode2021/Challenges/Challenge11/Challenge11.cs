namespace AdventOfCode2021.Challenges.Challenge11;

internal class Challenge11 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var input = inputText.Select(x => x.ToCharArray().Select(y => y - '0').ToArray()).ToArray();
        return Task1(input);
    }

    public object RunTask2(string[] inputText)
    {
        var input = inputText.Select(x => x.ToCharArray().Select(y => y - '0').ToArray()).ToArray();
        return Task2(input);
    }

    private static int Task1(IReadOnlyList<int[]> input)
    {
        // copy
        input = input.Select(x => x.Select(y => y).ToArray()).ToArray();

        //Console.WriteLine("Before any step:");
        //PrintField(input);

        var flashCount = 0;

        for (var step = 1; step <= 100; step++)
        {
            input = input.Select(y => y.Select(x => x + 1).ToArray()).ToArray();

            for (var i = 0; i < input.Count; i++)
            {
                for (var j = 0; j < input[0].Length; j++)
                {
                    if (input[i][j] < 10) continue;

                    // flash
                    flashCount += Flash(input, i, j);
                }
            }

            input = input.Select(x => x.Select(y => y < 0 ? 0 : y).ToArray()).ToArray();

            //Console.WriteLine($"After step {step}: {flashCount}");
            //PrintField(input);
        }

        return flashCount;
    }

    private static int Task2(IReadOnlyList<int[]> input)
    {
        // copy
        input = input.Select(x => x.Select(y => y).ToArray()).ToArray();

        //Console.WriteLine("Before any step:");
        //PrintField(input);

        var total = input.Count * input[0].Length;

        for (var step = 1;; step++)
        {
            var flashCount = 0;
            input = input.Select(y => y.Select(x => x + 1).ToArray()).ToArray();

            for (var i = 0; i < input.Count; i++)
            {
                for (var j = 0; j < input[0].Length; j++)
                {
                    if (input[i][j] < 10) continue;

                    // flash
                    flashCount += Flash(input, i, j);
                }
            }

            if (flashCount == total)
            {
                return step;
            }

            input = input.Select(x => x.Select(y => y < 0 ? 0 : y).ToArray()).ToArray();

            //Console.WriteLine($"After step {step}: {flashCount}");
            //PrintField(input);
        }
    }

    private static int Flash(IReadOnlyList<int[]> input, int i, int j)
    {
        var flashCount = 1;
        input[i][j] = int.MinValue;

        for (var k = i - 1; k <= i + 1; k++)
        {
            for (var l = j - 1; l <= j + 1; l++)
            {
                if (k == i && l == j) continue;
                if (k < 0 || k >= input.Count) continue;
                if (l < 0 || l >= input[0].Length) continue;

                var value = ++input[k][l];
                if (value < 10) continue;

                flashCount += Flash(input, k, l);
            }
        }

        return flashCount;
    }

    private static void PrintField(IEnumerable<int[]> field)
    {
        var output = field
            .Select(x => x
                .Select(y => (char)(y + '0'))
                .ToArray())
            .Select(x => new string(x))
            .ToList();
        Console.WriteLine(string.Join('\n', output) + '\n');
    }
}