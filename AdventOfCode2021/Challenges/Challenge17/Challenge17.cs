using System.Text.RegularExpressions;

namespace AdventOfCode2021.Challenges.Challenge17;

public class Challenge17 : IAocChallenge
{
    private static readonly Regex InputRegex = new(@"^target area: x=(\d+)..(\d+), y=(-\d+)..(-\d+)$");

    public object RunTask1(string[] inputText)
    {
        var targetArea = ParseInput(inputText);

        var maxVy = -1 - targetArea.YFrom;
        return maxVy * (maxVy + 1) / 2;
    }

    public object RunTask2(string[] inputText)
    {
        var targetArea = ParseInput(inputText);

        var minVy = targetArea.YFrom;
        var maxVy = -1 - targetArea.YFrom;
        const int minVx = 1;
        var maxVx = targetArea.XTo;

        var velocities = Enumerable
            .Range(minVx, maxVx - minVx + 1)
            .SelectMany(x => Enumerable
                .Range(minVy, maxVy - minVy + 1)
                .Select(y => (x, y)))
            .ToList();

        return velocities
            .Count(x => SimulateLaunch(x.x, x.y, targetArea));
    }

    private static TargetArea ParseInput(IEnumerable<string> inputText)
    {
        var match = InputRegex.Match(inputText.First());
        return new TargetArea(
            int.Parse(match.Groups[1].Value),
            int.Parse(match.Groups[2].Value),
            int.Parse(match.Groups[3].Value),
            int.Parse(match.Groups[4].Value)
        );
    }

    private static bool SimulateLaunch(int xVel, int yVel, TargetArea targetArea)
    {
        var xPos = 0;
        var yPos = 0;

        while (xPos <= targetArea.XTo && yPos >= targetArea.YFrom)
        {
            if (xPos >= targetArea.XFrom && yPos <= targetArea.YTo)
            {
                return true;
            }

            xPos += xVel;
            yPos += yVel;
            if (xVel > 0)
            {
                xVel--;
            }
            yVel--;
        }

        return false;
    }
}

internal record TargetArea(int XFrom, int XTo, int YFrom, int YTo);