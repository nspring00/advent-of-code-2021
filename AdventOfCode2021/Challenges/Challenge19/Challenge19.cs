namespace AdventOfCode2021.Challenges.Challenge19;

public class Challenge19 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var scanners = ParseInput(inputText).ToList();

        var fixedScanners = scanners.Take(1).ToList();
        var relativeScanners = scanners.Skip(1).ToList();

        while (relativeScanners.Count > 0)
        {
            foreach (var fixedScanner in fixedScanners)
            {
                foreach (var relativeScanner in relativeScanners)
                {
                    foreach (var adjustedScanner in GetScannerPermutations(relativeScanner))
                    {
                        // TODO this does not (yet) work

                        var distances = fixedScanner.Beacons
                            .SelectMany(b1 => adjustedScanner.Beacons
                                .Select(b2 => Distance(b1, b2)))
                            .ToList();

                        var dict = distances
                            .GroupBy(x => x)
                            .ToDictionary(x => x.Key, x => x.Count());

                        var isOverlapping = distances
                            .GroupBy(x => x)
                            .Any(x => x.Count() >= 66);

                        Console.WriteLine(distances.GroupBy(x => x).Max(x => x.Count()));

                        if (distances.GroupBy(x => x).Max(x => x.Count()) > 2)
                        {
                            // TODO check
                        }

                        var test = distances.Average();

                        if (isOverlapping)
                        {
                            Console.WriteLine("working");
                            relativeScanners.Remove(relativeScanner);
                            // TODO transform scanner here
                            var transformedScanner = adjustedScanner;
                            fixedScanners.Add(transformedScanner);
                        }
                    }
                }
            }

            // TODO remove after test
            break;
        }

        return -1;
    }

    public object RunTask2(string[] inputText)
    {
        return -1;
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
}

internal record Scanner(IList<Position> Beacons);

internal record Position(int X, int Y, int Z);