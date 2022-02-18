namespace AdventOfCode2021.Challenges.Challenge20;

internal class Challenge20 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var mapping = inputText.First().ToCharArray().Select(x => x == '#').ToArray();

        var imageSet = inputText.Skip(2)
            .SelectMany((x, row) => x.ToCharArray().Select((c, col) => (x: row, y: col, c)))
            .Where(x => x.c == '#')
            .Select(x => new Point(x.x, x.y))
            .ToHashSet();

        var result = EnhanceImage(imageSet, mapping, 2);

        return result.Count;
    }

    public object RunTask2(string[] inputText)
    {
        var mapping = inputText.First().ToCharArray().Select(x => x == '#').ToArray();

        var imageSet = inputText.Skip(2)
            .SelectMany((x, row) => x.ToCharArray().Select((c, col) => (x: row, y: col, c)))
            .Where(x => x.c == '#')
            .Select(x => new Point(x.x, x.y))
            .ToHashSet();

        var result = EnhanceImage(imageSet, mapping, 50);

        return result.Count;
    }

    private static ICollection<Point> EnhanceImage(ICollection<Point> image, IReadOnlyList<bool> mapping, int iterations)
    {
        var result = image;

        for (var i = 0; i < iterations; i++)
        {
            var backgroundValue = i % 2 == 0 ? '0' : '1';
            result = DoOneIteration(result, mapping, backgroundValue);
        }

        return result;
    }

    private static ICollection<Point> DoOneIteration(ICollection<Point> image, IReadOnlyList<bool> mapping, char backgroundValue)
    {
        HashSet<Point> newImage = new();

        var minX = image.Min(x => x.X);
        var maxX = image.Max(x => x.X);
        var minY = image.Min(x => x.Y);
        var maxY = image.Max(x => x.Y);


        for (var x = minX - 1; x <= maxX + 1; x++)
        {
            for (var y = minY - 1; y <= maxY + 1; y++)
            {
                var point = new Point(x, y);
                var newIndex = NeighborhoodValue(point, image, backgroundValue, minX, maxX, minY, maxY);
                if (mapping[newIndex])
                {
                    newImage.Add(point);
                }
            }
        }

        return newImage;
    }

    private static int NeighborhoodValue(Point p, ICollection<Point> image, char backgroundValue, int minX, int maxX, int minY, int maxY)
    {
        var (x, y) = p;

        var checkCoordinates = new[]
        {
            new Point(x - 1, y - 1),
            new Point(x - 1, y),
            new Point(x - 1, y + 1),
            new Point(x, y - 1),
            new Point(x, y),
            new Point(x, y + 1),
            new Point(x + 1, y - 1),
            new Point(x + 1, y),
            new Point(x + 1, y + 1)
        };

        var pointValues = checkCoordinates
            .Select(point => 
                PointValue(point, image, backgroundValue, minX, maxX, minY, maxY)).ToList();
        var binaryString = string.Join("", pointValues);

        return Convert.ToInt32(binaryString, 2);
    }

    private static char PointValue(Point p, ICollection<Point> image, char backgroundValue,
        int minX, int maxX, int minY, int maxY)
    {
        if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
        {
            return backgroundValue;
        }

        return image.Contains(p) ? '1' : '0';
    }

    private static void PrintImage(ICollection<Point> image)
    {
        var minX = image.Min(x => x.X);
        var maxX = image.Max(x => x.X);
        var minY = image.Min(x => x.Y);
        var maxY = image.Max(x => x.Y);

        for (var x = minX; x <= maxX; x++)
        {
            for (var y = minY; y <= maxY; y++)
            {
                Console.Write(image.Contains(new Point(x, y)) ? '#' : '.');
            }
            Console.WriteLine();
        }
    }
}

internal record Point(int X, int Y);