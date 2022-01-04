namespace AdventOfCode2021.Challenges.Challenge7;

internal class Challenge7 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var positions = ParseInput(inputText);
        return DetermineLeastCost(positions, PositionCost1);
    }

    public object RunTask2(string[] inputText)
    {
        var positions = ParseInput(inputText);
        return DetermineLeastCost(positions, PositionCost2);
    }

    private static IList<int> ParseInput(IEnumerable<string> inputText)
    {
        return inputText.First().Split(",").Select(int.Parse).ToList();
    }

    private static decimal DetermineLeastCost(IList<int> positions,
        Func<IEnumerable<int>, int, decimal> positionCostFunc)
    {
        var minPos = positions.Min();
        var maxPos = positions.Max();

        var cheapestCost = decimal.MaxValue;
        for (var i = minPos; i <= maxPos; i++)
        {
            //Console.WriteLine($"{i}/{maxPos}");
            var cost = positionCostFunc.Invoke(positions, i);
            if (cost >= cheapestCost) continue;

            cheapestCost = cost;
        }

        return cheapestCost;
    }

    private static decimal PositionCost1(IEnumerable<int> states, int position)
    {
        return states.Select(x => (decimal)Math.Abs(x - position)).Sum();
    }

    private static decimal PositionCost2(IEnumerable<int> states, int position)
    {
        return states.Select(x => (decimal)Math.Abs(x - position)).Select(n => n * (n + 1) / 2).Sum();
    }
}