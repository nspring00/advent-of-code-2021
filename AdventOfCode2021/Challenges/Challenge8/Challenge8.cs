namespace AdventOfCode2021.Challenges.Challenge8;

internal class Challenge8 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        return Task1(inputText);
    }

    public object RunTask2(string[] inputText)
    {
        return Task2(inputText);
    }

    private static int Task1(IEnumerable<string> textInput)
    {
        var outputs = textInput.Select(x => x.Split("|").Last()).ToList();
        return outputs.Select(CountSpecificDigits).Sum();
    }

    private static int Task2(IEnumerable<string> textInput)
    {
        var sum = 0;
        foreach (var line in textInput)
        {
            var split = line.Split("|");
            var map = CreateSignalMap(split[0]);
            var result = CalculateResult(split[1], map);
            sum += result;
        }

        return sum;
    }

    private static int CalculateResult(string outputText, IDictionary<IList<Signal>, int> map)
    {
        var outputs = outputText
            .Split(" ")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseSignal)
            .ToList();

        var mapped = outputs.Select(x => map.Keys.Single(y => y.OrderBy(e => e).SequenceEqual(x.OrderBy(e => e))))
            .ToList();

        var values = mapped.Select(x => map[x]).ToList();

        var expoValues = values.Select((value, i) => value * (int)Math.Pow(10, 3 - i));
        return expoValues.Sum();
    }

    private static IDictionary<IList<Signal>, int> CreateSignalMap(string inputText)
    {
        var inputs = inputText
            .Split(" ")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseSignal)
            .ToList();

        var seg1 = inputs.Single(x => x.Count == 2);
        var seg7 = inputs.Single(x => x.Count == 3);
        var seg4 = inputs.Single(x => x.Count == 4);
        var seg235 = inputs.Where(x => x.Count == 5).ToList();
        var seg690 = inputs.Where(x => x.Count == 6).ToList();
        var seg8 = inputs.Single(x => x.Count == 7);

        if (seg235.Count != 3 || seg690.Count != 3) throw new Exception();

        var segA = seg7.Except(seg1).Single();
        var segBd = seg4.Except(seg1).ToList();
        if (segBd.Count != 2) throw new Exception();
        var seg5 = seg235.Single(x => x.Contains(segA) && x.Contains(segBd[0]) && x.Contains(segBd[1]));

        var segF = seg5.Intersect(seg1).Single();
        var segC = seg1.Except(new[]
        {
            segF
        }).Single();

        var seg23 = seg235.Except(new[]
        {
            seg5
        }).ToList();
        var seg2 = seg23.Single(x => !x.Contains(segF));
        var seg3 = seg23.Single(x => x.Contains(segF));

        var seg6 = seg690.Single(x => !x.Contains(segC));
        var segE = seg8.Except(seg3).Except(seg4).Single();
        var seg90 = seg690.Except(new[]
        {
            seg6
        }).ToList();
        var seg9 = seg90.Single(x => !x.Contains(segE));
        var seg0 = seg90.Single(x => x.Contains(segE));

        Dictionary<IList<Signal>, int> returnDictionary = new(10)
        {
            {
                seg0, 0
            },
            {
                seg1, 1
            },
            {
                seg2, 2
            },
            {
                seg3, 3
            },
            {
                seg4, 4
            },
            {
                seg5, 5
            },
            {
                seg6, 6
            },
            {
                seg7, 7
            },
            {
                seg8, 8
            },
            {
                seg9, 9
            }
        };

        return returnDictionary;
    }

    private static int CountSpecificDigits(string outputText)
    {
        var outputs = outputText
            .Split(" ")
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(ParseSignal)
            .ToList();

        return outputs.Count(x => x.Count is <= 4 or 7);
    }

    private static IList<Signal> ParseSignal(string signalText)
    {
        return signalText
            .ToCharArray()
            .Select(x => x switch
            {
                'a' => Signal.A,
                'b' => Signal.B,
                'c' => Signal.C,
                'd' => Signal.D,
                'e' => Signal.E,
                'f' => Signal.F,
                'g' => Signal.G,
                _ => throw new ArgumentOutOfRangeException(nameof(x), x, null)
            })
            .ToList();
    }

    private enum Signal
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G
    }
}