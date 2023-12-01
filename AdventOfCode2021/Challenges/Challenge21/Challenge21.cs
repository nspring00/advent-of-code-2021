using System.Collections;

namespace AdventOfCode2021.Challenges.Challenge21;

internal class Challenge21 : IAocChallenge
{
    private static readonly int[] PossibleThrowsPlain = {
        1,
        2,
        3
    };

    private static readonly List<int> PossibleThrows = PossibleThrowsPlain
        .SelectMany(x => PossibleThrowsPlain
            .SelectMany(y => PossibleThrowsPlain
                .Select(z => x + y + z)))
        .ToList();

    private static readonly IDictionary<int, int> PossibleThrowsDictionary = PossibleThrows
        .GroupBy(x => x)
        .ToDictionary(x => x.Key, x => x.Count());

    public object RunTask1(string[] inputText)
    {
        var dice = new Deterministic100SidedDice();

        var pos1 = int.Parse(inputText.First().Split(" ").ElementAt(4));
        var pos2 = int.Parse(inputText.Skip(1).First().Split(" ").ElementAt(4));

        var score1 = 0;
        var score2 = 0;

        var player1Turn = true;

        const int targetScore = 1000;
        while (score1 < targetScore && score2 < targetScore)
        {
            var nextRolls = dice.Take(3);
            var nextRollsSum = nextRolls.Sum();

            if (player1Turn)
            {
                pos1 = DeterminePosition(pos1, nextRollsSum);
                score1 += pos1;
            }
            else
            {
                pos2 = DeterminePosition(pos2, nextRollsSum);
                score2 += pos2;
            }

            player1Turn = !player1Turn;
        }

        var loserScore = score1 < targetScore ? score1 : score2;

        return loserScore * dice.TotalRolls;
    }

    public object RunTask2(string[] inputText)
    {
        var pos1 = int.Parse(inputText.First().Split(" ").ElementAt(4));
        var pos2 = int.Parse(inputText.Skip(1).First().Split(" ").ElementAt(4));

        var (wins1, wins2) = PlaySecondGame(pos1, pos2, 0, 0, true);
        return Math.Max(wins1, wins2);
    }

    private static (long wins1, long wins2) PlaySecondGame(int pos1, int pos2, int score1, int score2, bool player1Turn)
    {
        const int targetScore = 21;
        if (score1 >= targetScore)
        {
            return (1, 0);
        }

        if (score2 >= targetScore)
        {
            return (0, 1);
        }

        if (player1Turn)
        {
            var results = PossibleThrowsDictionary
                .Select(keyValuePair =>
                {
                    var (diceValue, count) = keyValuePair;
                    var newPos = DeterminePosition(pos1, diceValue);
                    var newScore = score1 + newPos;
                    var (wins1, wins2) = PlaySecondGame(newPos, pos2, newScore, score2, !player1Turn);
                    return (wins1: wins1 * count, wins2: wins2 * count);
                })
                .ToList();

            return (
                results.Select(x => x.wins1).Sum(),
                results.Select(x => x.wins2).Sum());
        }
        else
        {
            var results = PossibleThrowsDictionary
                .Select(keyValuePair =>
                {
                    var (diceValue, count) = keyValuePair;
                    var newPos = DeterminePosition(pos2, diceValue);
                    var newScore = score2 + newPos;
                    var (wins1, wins2) = PlaySecondGame(pos1, newPos, score1, newScore, !player1Turn);
                    return (wins1: wins1 * count, wins2: wins2 * count);
                })
                .ToList();

            return (
                results.Select(x => x.wins1).Sum(),
                results.Select(x => x.wins2).Sum());
        }
    }

    private static int DeterminePosition(int oldPosition, int diceValue) => (oldPosition + diceValue - 1) % 10 + 1;
}

internal class Deterministic100SidedDice : IEnumerable<int>
{
    public int TotalRolls { get; private set; }

    public int Next()
    {
        return TotalRolls++ % 100 + 1;
    }

    public IEnumerator<int> GetEnumerator()
    {
        while (true)
        {
            yield return Next();
        }
        // ReSharper disable once IteratorNeverReturns
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}