namespace AdventOfCode2021.Challenges.Challenge2;

internal class Challenge2 : IAocChallenge
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

    private static IList<InputLine> ParseInput(IEnumerable<string> inputText) =>
        inputText.Select(MapInput).ToList();

    private static int Task1(IEnumerable<InputLine> input)
    {
        var horizontalPosition = 0;
        var depth = 0;

        foreach (var (direction, value) in input)
        {
            switch (direction)
            {
                case "forward":
                    horizontalPosition += value;
                    break;
                case "down":
                    depth += value;
                    break;
                case "up":
                    depth -= value;
                    break;
            }
        }

        return horizontalPosition * depth;
    }

    private static int Task2(IEnumerable<InputLine> input)
    {
        var aim = 0;
        var horizontalPosition = 0;
        var depth = 0;

        foreach (var (direction, value) in input)
        {
            switch (direction)
            {
                case "forward":
                    horizontalPosition += value;
                    depth += aim * value;
                    break;
                case "down":
                    aim += value;
                    break;
                case "up":
                    aim -= value;
                    break;
            }
        }

        return horizontalPosition * depth;
    }

    private static InputLine MapInput(string line)
    {
        var split = line.Split(' ');
        return new InputLine(split[0], int.Parse(split[1]));
    }

    private readonly record struct InputLine(string Direction, int Value);
}