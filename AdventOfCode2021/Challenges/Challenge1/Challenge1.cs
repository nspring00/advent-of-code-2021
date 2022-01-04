namespace AdventOfCode2021.Challenges.Challenge1;

public class Challenge1 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var values = inputText.Select(int.Parse).ToList();
        return Task1(values);
    }

    public object RunTask2(string[] inputText)
    {
        var values = inputText.Select(int.Parse).ToList();
        return Task2(values);
    }

    private static int Task1(IEnumerable<int> values)
    {
        var lastValue = int.MaxValue;
        var counter = 0;

        foreach (var value in values)
        {
            if (value > lastValue)
            {
                counter++;
            }

            lastValue = value;
        }

        return counter;
    }

    private static int Task2(IReadOnlyList<int> values)
    {
        var lastSum = int.MaxValue;
        var counter = 0;

        for (var i = 0; i < values.Count - 2; i++)
        {
            var sum = values[i] + values[i + 1] + values[i + 2];
            if (sum > lastSum)
            {
                counter++;
            }

            lastSum = sum;
        }

        return counter;
    }
}