using System.Text.RegularExpressions;

namespace AdventOfCode2021.Challenges.Challenge22;

internal class Challenge22 : IAocChallenge
{
    private const string InputPattern = @"(on|off) x=(-?\d+)..(-?\d+),y=(-?\d+)..(-?\d+),z=(-?\d+)..(-?\d+)";

    public object RunTask1(string[] inputText)
    {
        var input = ParseInput(inputText);

        var field = GenerateField();

        foreach (var instruction in input)
        {
            var points = instruction.GetPoints1();
            if (instruction.TurnOn)
            {
                foreach (var point in points)
                {
                    field[point] = true;
                }
            }
            else
            {
                foreach (var point in points)
                {
                    field[point] = false;
                }
            }
        }

        return field.Count(x => x.Value);
    }

    public object RunTask2(string[] inputText)
    {
        var input = ParseInput(inputText);

        var field = new HashSet<Point>();

        var c = 1;
        foreach (var instruction in input)
        {
            var points = instruction.GetPoints2();
            if (instruction.TurnOn)
            {
                foreach (var point in points)
                {
                    field.Add(point);
                }
            }
            else
            {
                foreach (var point in points)
                {
                    field.Remove(point);
                }
            }
            Console.WriteLine(c++);
        }

        return field.Count;
    }

    private static IList<Instruction> ParseInput(IEnumerable<string> inputText)
    {
        var regex = new Regex(InputPattern, RegexOptions.Compiled);

        return inputText
            .Select(x => ParseInputLine(x, regex))
            .ToList();
    }

    private static Instruction ParseInputLine(string inputLine, Regex regex)
    {
        var match = regex.Match(inputLine);
        var turnOn = inputLine.StartsWith("on");

        return new Instruction(
            turnOn,
            int.Parse(match.Groups[2].Value),
            int.Parse(match.Groups[3].Value),
            int.Parse(match.Groups[4].Value),
            int.Parse(match.Groups[5].Value),
            int.Parse(match.Groups[6].Value),
            int.Parse(match.Groups[7].Value)
        );
    }

    private static IDictionary<Point, bool> GenerateField()
    {
        Dictionary<Point, bool> dict = new();

        for (var x = -50; x <= 50; x++)
        {
            for (var y = -50; y <= 50; y++)
            {
                for (var z = -50; z <= 50; z++)
                {
                    dict.Add(new Point(x, y, z), false);
                }
            }
        }

        return dict;
    }
}

internal record Point(int X, int Y, int Z);

internal record Instruction(bool TurnOn, int FromX, int ToX, int FromY, int ToY, int FromZ, int ToZ)
{
    public IList<Point> GetPoints1()
    {
        var points = new List<Point>();

        var fromX = Math.Max(FromX, -50);
        var toX = Math.Min(ToX, 50);
        var fromY = Math.Max(FromY, -50);
        var toY = Math.Min(ToY, 50);
        var fromZ = Math.Max(FromZ, -50);
        var toZ = Math.Min(ToZ, 50);

        for (var x = fromX; x <= toX; x++)
        {
            for (var y = fromY; y <= toY; y++)
            {
                for (var z = fromZ; z <= toZ; z++)
                {
                    points.Add(new Point(x, y, z));
                }
            }
        }

        return points;
    }
    
    public IList<Point> GetPoints2()
    {
        var points = new List<Point>();

        for (var x = FromX; x <= ToX; x++)
        {
            for (var y = FromY; y <= ToY; y++)
            {
                for (var z = FromZ; z <= ToZ; z++)
                {
                    points.Add(new Point(x, y, z));
                }
            }
        }

        return points;
    }
}
