namespace AdventOfCode2021.Challenges.Challenge3;

internal class Challenge3 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        return Task1(inputText);
    }

    public object RunTask2(string[] inputText)
    {
        return Task2(inputText);
    }

    private static int Task1(IReadOnlyList<string> input)
    {
        var length = input[0].Length;
        var gamma = 0;

        for (var i = 0; i < length; i++)
        {
            var result = MostCommonValue(input, i);
            gamma += (int)Math.Pow(2, length - i - 1) * result;
        }

        var epsilon = (int)Math.Pow(2, length) - gamma - 1;

        return gamma * epsilon;
    }

    private static int Task2(IReadOnlyCollection<string> input)
    {
        var oxygenRating = OxygenRating(input, 0);
        var co2Rating = Co2Rating(input, 0);

        return oxygenRating * co2Rating;
    }

    private static int OxygenRating(IReadOnlyCollection<string> input, int index)
    {
        if (input.Count == 1)
        {
            return Convert.ToInt32(input.First(), 2);
        }

        var mcv = MostCommonValue(input, index);
        var newList = input
            .Where(x => int.Parse(x[index].ToString()) == mcv)
            .ToList();

        return OxygenRating(newList, index + 1);
    }

    private static int Co2Rating(IReadOnlyCollection<string> input, int index)
    {
        if (input.Count == 1)
        {
            return Convert.ToInt32(input.First(), 2);
        }

        var mcv = LeastCommonValue(input, index);
        var newList = input
            .Where(x => int.Parse(x[index].ToString()) == Convert.ToChar(mcv))
            .ToList();

        return Co2Rating(newList, index + 1);
    }

    private static int MostCommonValue(IEnumerable<string> input, int index)
    {
        var values = input.Select(x => x[index]).ToList();
        var dict = values
            .GroupBy(x => x)
            .ToDictionary(x => x.Key, x => x.Count());
        var result = dict
            .Where(x => x.Value == dict.Values.Max())
            .Select(x => int.Parse(x.Key.ToString())).ToList();

        return result.Count > 1 ? 1 : result.First();
    }

    private static int LeastCommonValue(IEnumerable<string> input, int index)
    {
        return 1 - MostCommonValue(input, index);
    }
}