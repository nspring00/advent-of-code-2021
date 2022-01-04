namespace AdventOfCode2021.Challenges.Challenge4;

internal class BingoField
{
    private readonly int[][] _field;
    private readonly List<int> _inputs = new();

    public BingoField(IEnumerable<string> fieldInput)
    {
        _field = fieldInput
            .Select(x => x.Split(' '))
            .Select(x => x
                .Where(y => !string.IsNullOrWhiteSpace(y))
                .Select(int.Parse)
                .ToArray())
            .ToArray();
    }

    public bool AddInput(int number)
    {
        _inputs.Add(number);
        return CheckFinished();
    }

    private bool CheckFinished()
    {
        var checkedField = _field.Select(row => row.Select(val => _inputs.Contains(val)).ToArray()).ToArray();
        var checkRows = checkedField.Any(row => row.All(x => x));
        if (checkRows)
        {
            return true;
        }

        var checkCols = checkedField
            .Select((_, i) => checkedField
                .Select(row => row[i])
                .All(x => x))
            .Any(checkColumns => checkColumns);

        if (checkCols)
        {
            return true;
        }

        return checkCols;
    }

    public int Evaluate()
    {
        var unmarkedSum = _field.SelectMany(x => x).Where(x => !_inputs.Contains(x)).Sum();

        return unmarkedSum * _inputs.Last();
    }
}