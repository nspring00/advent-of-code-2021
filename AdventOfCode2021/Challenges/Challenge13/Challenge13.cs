namespace AdventOfCode2021.Challenges.Challenge13;

internal class Challenge13 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var (field, instructions) = ParseInput(inputText);
        return Task1(field, instructions);
    }

    public object RunTask2(string[] inputText)
    {
        var (field, instructions) = ParseInput(inputText);
        return Task2(field, instructions);
    }

    private static (List<Point> field, List<FoldInstruction> instructions) ParseInput(string[] inputText)
    {
        var field = inputText
            .TakeWhile(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x
                .Split(',')
                .Select(int.Parse)
                .ToArray())
            .Select(x => new Point(x[0], x[1]))
            .ToList();

        var folds = inputText
            .SkipWhile(x => !string.IsNullOrWhiteSpace(x))
            .Skip(1)
            .Select(x => new FoldInstruction(x[11], int.Parse(x[13..])))
            .ToList();

        return (field, folds);
    }

    private static int Task1(ICollection<Point> field, IEnumerable<FoldInstruction> foldInstructions)
    {
        field = ExecuteFold(field, foldInstructions.First());
        return field.Count;
    }

    private static string Task2(ICollection<Point> field, IEnumerable<FoldInstruction> foldInstructions)
    {
        var resultField = foldInstructions.Aggregate(field, ExecuteFold);

        PrintField(resultField);

        return "EFJKZLBL";
    }

    private static void PrintField(ICollection<Point> field)
    {
        var maxX = field.Max(x => x.X);
        var maxY = field.Max(x => x.Y);

        for (var y = 0; y <= maxY; y++)
        {
            for (var x = 0; x <= maxX; x++)
            {
                //Console.Write(field.Contains(new Point(x, y)) ? '#' : ' ');
            }
            //Console.WriteLine();
        }
        //Console.WriteLine();
    }


    private static IList<Point> ExecuteFold(IEnumerable<Point> field, FoldInstruction foldInstruction)
    {
        var (axis, foldValue) = foldInstruction;
        if (axis == 'x')
        {
            return field
                .Select(p => p.X <= foldValue ? p : p with { X = 2 * foldValue - p.X })
                .Where(p => p.X != foldValue)
                .Distinct()
                .ToList();
        }

        return field
            .Select(p => p.Y <= foldValue ? p : p with { Y = 2 * foldValue - p.Y })
            .Where(p => p.Y != foldValue)
            .Distinct()
            .ToList();
    }
}

internal record Point(int X, int Y);

internal record FoldInstruction(char Axis, int Value);