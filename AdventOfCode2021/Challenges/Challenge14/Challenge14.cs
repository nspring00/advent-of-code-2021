using System.Text.RegularExpressions;

namespace AdventOfCode2021.Challenges.Challenge14;

public class Challenge14 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var (baseLine, mappings) = ParseInput(inputText);
        return ExecuteSimulationFast(baseLine, mappings, 10);
    }

    public object RunTask2(string[] inputText)
    {
        var (baseLine, mappings) = ParseInput(inputText);
        return ExecuteSimulationFast(baseLine, mappings, 40);
    }

    private static (string, IDictionary<(char, char), char>) ParseInput(string[] textInput)
    {
        var baseLine = textInput.First();
        var mappings = textInput
            .Skip(2)
            .Select(x => x.Split(" -> ").ToArray())
            .ToDictionary(x => (x[0][0], x[0][1]), x => x[1][0]);

        return (baseLine, mappings);
    }

    private static long ExecuteSimulationFast(string baseLine, IDictionary<(char, char), char> mappings, int iterations)
    {
        var last = baseLine.Last();
        var values = mappings.Keys
            .ToDictionary(x => x, x =>
                Regex.Matches(baseLine, new string(new[] { x.Item1, x.Item2 })).LongCount())
            .Where(x => x.Value != 0)
            .ToDictionary(x => x.Key, x => x.Value);

        for (var i = 0; i < iterations; i++)
        {
            var newValues = new Dictionary<(char, char), long>();
            foreach (var (firstC, lastC) in values.Keys)
            {
                var count = values[(firstC, lastC)];
                var middleC = mappings[(firstC, lastC)];

                var key = (firstC, middleC);
                if (!newValues.ContainsKey(key))
                {
                    newValues.Add(key, count);
                }
                else
                {
                    newValues[key] += count;
                }

                key = (middleC, lastC);
                if (!newValues.ContainsKey(key))
                {
                    newValues.Add(key, count);
                }
                else
                {
                    newValues[key] += count;
                }
            }

            values = newValues;
        }

        var chars = new Dictionary<char, long>();
        foreach (var (firstC, lastC) in values.Keys)
        {
            if (!chars.ContainsKey(firstC))
            {
                chars.Add(firstC, values[(firstC, lastC)]);
            }
            else
            {
                chars[firstC] += values[(firstC, lastC)];
            }
        }

        if (!chars.ContainsKey(last))
        {
            chars.Add(last, 1);
        }
        else
        {
            chars[last] += 1;
        }

        var mostCommonCount = chars.Max(x => x.Value);
        var leastCommonCount = chars.Min(x => x.Value);

        return mostCommonCount - leastCommonCount;
    }

    private static long ExecuteSimulation(char[] baseLine, IDictionary<(char, char), char> mappings, int iterations)
    {
        var line = baseLine;
        for (var i = 0; i < iterations; i++)
        {
            Console.WriteLine(i);
            line = SimulateRound(line, mappings);
        }

        var mostCommonCount = line.GroupBy(x => x).Max(x => x.LongCount());
        var leastCommonCount = line.GroupBy(x => x).Min(x => x.LongCount());

        return mostCommonCount - leastCommonCount;
    }

    private static char[] SimulateRound(IReadOnlyList<char> inputLine, IDictionary<(char, char), char> mappings)
    {
        var returnSpan = new Span<char>(new char[inputLine.Count * 2 - 1]);

        for (var i = 0; i < inputLine.Count; i++)
        {
            var c = inputLine[i];
            returnSpan[i * 2] = c;

            if (i == inputLine.Count - 1) continue;
            var nextC = inputLine[i + 1];
            returnSpan[i * 2 + 1] = mappings[(c, nextC)];
        }

        return returnSpan.ToArray();
    }
}