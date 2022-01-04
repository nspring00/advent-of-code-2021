namespace AdventOfCode2021.Challenges.Challenge6;

internal class Challenge6 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var states = ParseInput(inputText);
        return SimulateNew(states, 80);
    }

    public object RunTask2(string[] inputText)
    {
        var states = ParseInput(inputText);
        return SimulateNew(states, 256);
    }

    private static IList<int> ParseInput(IEnumerable<string> inputText)
    {
        return inputText.First().Split(",").Select(int.Parse).ToList();
    }

    // too slow for high years value
    private static int Simulate(IList<int> states, int years)
    {
        for (var y = 0; y < years; y++)
        {
            List<int> newStates = new();
            foreach (var state in states)
            {
                if (state == 0)
                {
                    newStates.Add(6);
                    newStates.Add(8);
                }
                else
                {
                    newStates.Add(state - 1);
                }
            }

            states = newStates;
        }

        return states.Count;
    }

    private static long SimulateNew(IList<int> statesTemp, int years)
    {
        var count = new long[9];
        for (var i = 0; i < 9; i++)
        {
            count[i] = statesTemp.Count(x => x == i);
        }

        for (var y = 0; y < years; y++)
        {
            var newCount = new long[9];

            for (var age = 8; age >= 0; age--)
            {
                if (age == 0)
                {
                    newCount[8] = count[0];
                    newCount[6] += count[0];

                    continue;
                }

                newCount[age - 1] = count[age];
            }

            count = newCount;
        }

        return count.Sum();
    }
}