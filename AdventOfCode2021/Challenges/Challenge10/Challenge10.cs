namespace AdventOfCode2021.Challenges.Challenge10;

internal class Challenge10 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        return Task1(inputText);
    }

    public object RunTask2(string[] inputText)
    {
        return Task2(inputText);
    }

    private static long Task1(IEnumerable<string> inputText)
    {
        var corruptedChars = inputText
            .Select(CheckCorruptedLine)
            .Where(x => x.HasValue)
            .Select(x => x!.Value)
            .ToList();
        return corruptedChars
            .Select(EvaluateChar1)
            .Sum();
    }

    private static long Task2(IEnumerable<string> inputText)
    {
        var incomplete = inputText.Where(x => CheckCorruptedLine(x) is null).ToList();

        var missing = incomplete.Select(GetMissingChars).ToList();

        var values = missing.Select(EvaluateMissingChars).OrderBy(x => x).ToList();
        return values.ElementAt(missing.Count / 2);
    }

    private static char? CheckCorruptedLine(string line)
    {
        var stack = new Stack<char>();

        var opening = new[] { '(', '[', '<', '{' };

        foreach (var c in line.ToCharArray())
        {
            if (opening.Any(x => x.Equals(c)))
            {
                stack.Push(c);
                continue;
            }

            var lastValue = stack.Pop();
            if (c == GetCorresponding(lastValue)) continue;

            return c;
        }

        return null;
    }

    private static IEnumerable<char> GetMissingChars(string line)
    {
        var stack = new Stack<char>();

        var opening = new[] { '(', '[', '<', '{' };

        foreach (var c in line.ToCharArray())
        {
            if (opening.Any(x => x.Equals(c)))
            {
                stack.Push(c);
                continue;
            }

            stack.Pop();
        }

        return stack.Select(GetCorresponding).ToList();
    }

    private static char GetCorresponding(char c) =>
        c switch
        {
            '(' => ')',
            '[' => ']',
            '{' => '}',
            '<' => '>',
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };

    private static long EvaluateChar1(char c) =>
        c switch
        {
            ')' => 3,
            ']' => 57,
            '}' => 1197,
            '>' => 25137,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };

    private static long EvaluateMissingChars(IEnumerable<char> missingChars)
    {
        var sum = 0L;
        foreach (var c in missingChars)
        {
            sum *= 5;
            sum += EvaluateChar2(c);
        }

        return sum;
    }

    private static long EvaluateChar2(char c)
    {
        return c switch
        {
            ')' => 1,
            ']' => 2,
            '}' => 3,
            '>' => 4,
            _ => throw new ArgumentOutOfRangeException(nameof(c), c, null)
        };
    }
}