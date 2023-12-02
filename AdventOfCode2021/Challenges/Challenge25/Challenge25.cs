namespace AdventOfCode2021.Challenges.Challenge25;

internal class Challenge25 : IAocChallenge
{
    public object RunTask1(string[] input)
    {
        var field = ParseInput(input);
        var anyMoved = true;
        var step = 0;

        while (anyMoved)
        {
            anyMoved = false;

            // Copy first line
            var firstCol = new bool[field.GetLength(0)];
            for (var y = 0; y < field.GetLength(0); y++)
            {
                firstCol[y] = field[y, 0] == Tile.Empty;
            }

            // Move all right and wrap around
            for (var y = 0; y < field.GetLength(0); y++)
            {
                for (var x = 0; x < field.GetLength(1); x++)
                {
                    if (field[y, x] != Tile.Right) continue;
                    if (x == field.GetLength(1) - 1)
                    {
                        if (!firstCol[y]) continue;
                    }
                    else
                    {
                        if (field[y, x + 1] != Tile.Empty) continue;
                    }

                    field[y, x] = Tile.Empty;
                    field[y, (x + 1) % field.GetLength(1)] = Tile.Right;
                    x++;
                    anyMoved = true;
                }
            }

            var firstRow = new bool[field.GetLength(1)];
            for (var x = 0; x < field.GetLength(1); x++)
            {
                firstRow[x] = field[0, x] == Tile.Empty;
            }

            // Move all down
            for (var x = 0; x < field.GetLength(1); x++)
            {
                for (var y = 0; y < field.GetLength(0); y++)
                {
                    if (field[y, x] != Tile.Down) continue;
                    if (y == field.GetLength(0) - 1)
                    {
                        if (!firstRow[x]) continue;
                    }
                    else
                    {
                        if (field[y + 1, x] != Tile.Empty) continue;
                    }

                    field[y, x] = Tile.Empty;
                    field[(y + 1) % field.GetLength(0), x] = Tile.Down;
                    y++;
                    anyMoved = true;
                }
            }

            step++;
        }

        return step;
    }

    public object RunTask2(string[] input)
    {
        return 0;
    }

    private enum Tile
    {
        Empty,
        Right,
        Down,
    }

    private static Tile[,] ParseInput(string[] input)
    {
        var field = new Tile[input.Length, input[0].Length];
        for (var y = 0; y < input.Length; y++)
        {
            for (var x = 0; x < input[y].Length; x++)
            {
                field[y, x] = (input[y][x]) switch
                {
                    '.' => Tile.Empty,
                    '>' => Tile.Right,
                    'v' => Tile.Down,
                    _ => throw new Exception($"Unknown tile {input[y][x]} at {x},{y}."),
                };
            }
        }

        return field;
    }

    private static void PrintField(Tile[,] field)
    {
        for (var y = 0; y < field.GetLength(0); y++)
        {
            for (var x = 0; x < field.GetLength(1); x++)
            {
                Console.Write(field[y, x] switch
                {
                    Tile.Empty => '.',
                    Tile.Right => '>',
                    Tile.Down => 'v',
                    _ => throw new Exception($"Unknown tile {field[y, x]} at {x},{y}."),
                });
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }
}