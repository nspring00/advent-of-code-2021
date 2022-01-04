namespace AdventOfCode2021.Challenges.Challenge4;

internal class Challenge4 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var (numbers, bingoFields) = ParseInput(inputText);
        return Task1(numbers, bingoFields);
    }

    public object RunTask2(string[] inputText)
    {
        var (numbers, bingoFields) = ParseInput(inputText);
        return Task2(numbers, bingoFields);
    }

    private static (IList<int>, IList<BingoField>) ParseInput(string[] inputText)
    {
        var numbers = inputText.First().Split(',').Select(int.Parse).ToList();

        var bingoInput = inputText.Skip(2).ToList();

        var bingoFields = new List<BingoField>();

        while (true)
        {
            var nextInput = bingoInput.TakeWhile(x => !string.IsNullOrWhiteSpace(x)).ToList();
            if (nextInput.Count == 0)
            {
                break;
            }

            bingoFields.Add(new BingoField(nextInput));
            bingoInput = bingoInput.Skip(nextInput.Count + 1).ToList();
        }

        return (numbers, bingoFields);
    }

    private static int Task1(IEnumerable<int> numbers, IList<BingoField> bingoFields)
    {
        foreach (var inputNumber in numbers)
        {
            foreach (var bingoField in bingoFields)
            {
                if (bingoField.AddInput(inputNumber))
                {
                    return bingoField.Evaluate();
                }
            }
        }

        return -1;
    }

    private static int Task2(IEnumerable<int> numbers, IList<BingoField> bingoFields)
    {
        foreach (var inputNumber in numbers)
        {
            List<BingoField> nextItFields = new(bingoFields);

            foreach (var bingoField in bingoFields)
            {
                if (!bingoField.AddInput(inputNumber)) continue;

                if (bingoFields.Count == 1)
                {
                    return bingoField.Evaluate();
                }

                nextItFields.Remove(bingoField);
            }

            bingoFields = nextItFields;
        }

        return -1;
    }
}