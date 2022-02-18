using System.Runtime.CompilerServices;

namespace AdventOfCode2021.Challenges.Challenge19;

public class Challenge19 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var scanners = ParseInput(inputText).ToList();

        var fixedScanners = scanners.Take(1)
            .Select(x => new FixedScanner(new Position(0, 0, 0), x)).ToList();
        var relativeScanners = scanners.Skip(1).ToList();

        while (relativeScanners.Count > 0)
        {
            AdjustOneScanner(fixedScanners, relativeScanners);
        }

        var distinctBeacons = fixedScanners
            .SelectMany(x => x.Scanner.Beacons)
            .Distinct()
            .ToList();

        return distinctBeacons.Count;
    }

    public object RunTask2(string[] inputText)
    {
        var scanners = ParseInput(inputText).ToList();

        var fixedScanners = scanners.Take(1)
            .Select(x => new FixedScanner(new Position(0, 0, 0), x)).ToList();
        var relativeScanners = scanners.Skip(1).ToList();

        while (relativeScanners.Count > 0)
        {
            AdjustOneScanner(fixedScanners, relativeScanners);
        }

        var scannerPositions = fixedScanners.Select(x => x.Position).ToList();

        var manhattanDistances = scannerPositions
            .SelectMany(x => scannerPositions.Select(y => ManhattanDistance(x, y)))
            .ToList();

        return manhattanDistances.Max();
    }

    private static void AdjustOneScanner(ICollection<FixedScanner> fixedScanners, ICollection<Scanner> relativeScanners)
    {
        foreach (var fixedScanner in fixedScanners)
        {
            foreach (var relativeScanner in relativeScanners)
            {
                foreach (var adjustedScanner in GetScannerPermutations(relativeScanner))
                {
                    var distances = fixedScanner.Scanner.Beacons
                        .SelectMany(b1 => adjustedScanner.Beacons
                            .Select(b2 => new DistanceRecord(b1, b2, Distance(b1, b2))))
                        .ToList();

                    var maxOverlappingGrouping = distances
                        .GroupBy(x => x.Distance)
                        .MaxBy(x => x.Count())!;

                    var overlappingCount = maxOverlappingGrouping.Count();

                    if (overlappingCount < 12) continue;

                    relativeScanners.Remove(relativeScanner);

                    var (position1, position2, _) = maxOverlappingGrouping.First();
                    var positionOffset = position1 - position2;

                    var newFixedScanner = new FixedScanner(positionOffset,
                        new Scanner(adjustedScanner.Beacons.Select(x => x + positionOffset).ToList()));

                    fixedScanners.Add(newFixedScanner);

                    return;
                }
            }
        }

        throw new Exception("No overlapping scanners found");
    }

    private static IEnumerable<Scanner> GetScannerPermutations(Scanner scanner)
    {
        foreach (var repositionedScanner in GetScannerPositionPermutations(scanner))
        {
            yield return repositionedScanner;
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(p.X, p.Y, -p.Z)).ToList()
            };
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(p.X, -p.Y, p.Z)).ToList()
            };
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(p.X, -p.Y, -p.Z)).ToList()
            };
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(-p.X, p.Y, p.Z)).ToList()
            };
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(-p.X, p.Y, -p.Z)).ToList()
            };
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(-p.X, -p.Y, p.Z)).ToList()
            };
            yield return repositionedScanner with
            {
                Beacons = repositionedScanner.Beacons.Select(p => new Position(-p.X, -p.Y, -p.Z)).ToList()
            };
        }
    }

    private static IEnumerable<Scanner> GetScannerPositionPermutations(Scanner scanner)
    {
        yield return scanner;
        yield return new Scanner(scanner.Beacons.Select(p => new Position(p.X, p.Z, p.Y)).ToList());
        yield return new Scanner(scanner.Beacons.Select(p => new Position(p.Y, p.X, p.Z)).ToList());
        yield return new Scanner(scanner.Beacons.Select(p => new Position(p.Y, p.Z, p.X)).ToList());
        yield return new Scanner(scanner.Beacons.Select(p => new Position(p.Z, p.X, p.Y)).ToList());
        yield return new Scanner(scanner.Beacons.Select(p => new Position(p.Z, p.Y, p.X)).ToList());
    }

    private static double Distance(Position pos1, Position pos2)
    {
        var (x1, y1, z1) = pos1;
        var (x2, y2, z2) = pos2;
        return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2) + Math.Pow(z1 - z2, 2));
    }

    private static IEnumerable<Scanner> ParseInput(IEnumerable<string> inputText)
    {
        List<Position> currentPositions = new();
        foreach (var line in inputText)
        {
            if (line.StartsWith("---")) continue;

            if (string.IsNullOrWhiteSpace(line))
            {
                yield return new Scanner(currentPositions);
                currentPositions = new List<Position>();
                continue;
            }

            var position = line
                .Split(',')
                .Select(int.Parse)
                .ToArray();

            currentPositions.Add(new Position(
                position[0],
                position[1],
                position[2]
            ));
        }

        yield return new Scanner(currentPositions);
    }

    private static int ManhattanDistance(Position pos1, Position pos2)
    {
        var (x1, y1, z1) = pos1;
        var (x2, y2, z2) = pos2;
        return Math.Abs(x1 - x2) + Math.Abs(y1 - y2) + Math.Abs(z1 - z2);
    }
}

internal record DistanceRecord(Position Position1, Position Position2, double Distance);

internal record FixedScanner(Position Position, Scanner Scanner);

internal record Scanner(IList<Position> Beacons);

internal record Position(int X, int Y, int Z)
{
    public static Position operator -(Position pos1, Position pos2)
    {
        var (x1, y1, z1) = pos1;
        var (x2, y2, z2) = pos2;
        return new Position(x1 - x2, y1 - y2, z1 - z2);
    }

    public static Position operator +(Position pos1, Position pos2)
    {
        var (x1, y1, z1) = pos1;
        var (x2, y2, z2) = pos2;
        return new Position(x1 + x2, y1 + y2, z1 + z2);
    }
}