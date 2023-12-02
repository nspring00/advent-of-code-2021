using System.Diagnostics;

namespace AdventOfCode2021.Challenges.Challenge23;

internal class Challenge23 : IAocChallenge
{
    private const int FieldSize = 27;
    private const int CorridorSize = 11;

    private static readonly int[] ForbiddenFields =
    {
        2,
        4,
        6,
        8
    };

    private enum Tile
    {
        Empty = 4,
        A = 0,
        B = 1,
        C = 2,
        D = 3
    }

    public object RunTask1(string[] inputText)
    {
        var start = ParseInput(inputText);
        var isGoalCheck = (Tile[] stateToCheck) =>
        {
            if (stateToCheck[11] != Tile.A) return false;
            if (stateToCheck[12] != Tile.B) return false;
            if (stateToCheck[13] != Tile.C) return false;
            if (stateToCheck[14] != Tile.D) return false;
            if (stateToCheck[15] != Tile.A) return false;
            if (stateToCheck[16] != Tile.B) return false;
            if (stateToCheck[17] != Tile.C) return false;
            return stateToCheck[18] == Tile.D;
        };
        const int nrPerType = 2;

        var testResult = RoomMoves(new[]
        {
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.Empty,
            Tile.C,
            Tile.Empty,
            Tile.Empty,
            Tile.D,
            Tile.A,
            Tile.B,
            Tile.Empty,
            Tile.D,
        }, 2);
        Debug.Assert(new[]
        {
            -1,
            0,
            1,
            -1
        }.AsSpan().SequenceEqual(testResult));

        var visited = new HashSet<ulong>();
        var queue = new PriorityQueue<(Tile[], List<(Tile[], int)>), int>();
        queue.Enqueue((start, new List<(Tile[], int)>()), 0);

        while (queue.TryDequeue(out var item, out var cost))
        {
            var (state, prevStates) = item;
            // Console.WriteLine("---------------------------------------------");
            // PrintState(state, nrPerType);

            if (isGoalCheck(state))
            {
                Console.WriteLine("Found solution!");
                foreach (var (s, c) in prevStates)
                {
                    Console.WriteLine(c);
                    PrintState(s, nrPerType);
                }
                Console.WriteLine(cost);
                PrintState(state, nrPerType);

                return cost;
            }

            var stateSerialized = SerializeState(state);
            if (visited.Contains(stateSerialized))
            {
                continue;
            }

            visited.Add(stateSerialized);

            // Available rooms
            var rooms = RoomMoves(state, nrPerType);

            var prevStatesCopy = new List<(Tile[], int)>(prevStates)
            {
                (state, cost)
            };

            // First check for each Amphipod on the corridor (0<i<CorridorSize) if they can move into some room
            var canMoveToTarget = false;
            for (var i = 0; i < CorridorSize; i++)
            {
                var t = state[i];
                if (t == Tile.Empty) continue;

                if (rooms[(int)t] == -1) continue;

                // Check if Amphipod can move into room
                var targetCol = 2 + 2 * (int)t;
                var canMoveToRoom = true;
                for (var j = Math.Min(i, targetCol); j <= Math.Max(i, targetCol); j++)
                {
                    if (j == i || state[j] == Tile.Empty) continue;

                    canMoveToRoom = false;
                    break;
                }

                if (!canMoveToRoom) continue;

                // Move Amphipod into room
                var newState = (Tile[])state.Clone();
                newState[i] = Tile.Empty;
                newState[CorridorSize + 4 * rooms[(int)t] + (int)t] = t;

                var extraSteps = Math.Abs(targetCol - i) + rooms[(int)t] + 1;
                var extraCost = extraSteps * CostOfType(t);
                //PrintState(newState, nrPerType, 4);
                queue.Enqueue((newState, prevStatesCopy), cost + extraCost);
                canMoveToTarget = true;
                break;
            }

            // If any Amphipod can move into room, skip the rest
            if (canMoveToTarget) continue;

            for (var i = CorridorSize; i < CorridorSize + nrPerType * 4; i++)
            {
                // Check if Amphipod can move into corridor
                var t = state[i];
                if (t == Tile.Empty) continue;

                // Check that no other Amphipod is above
                var canMoveToCorridor = true;
                for (var j = i - 4; j >= CorridorSize; j -= 4)
                {
                    if (state[j] == Tile.Empty) continue;

                    canMoveToCorridor = false;
                    break;
                }

                if (!canMoveToCorridor) continue;

                var currentCol = 2 + 2 * ((i - CorridorSize) % 4);

                // Move left
                for (var j = currentCol - 1; j >= 0; j--)
                {
                    if (state[j] != Tile.Empty) break;
                    if (ForbiddenFields.Contains(j)) continue;

                    var newState = (Tile[])state.Clone();
                    newState[i] = Tile.Empty;
                    newState[j] = t;

                    var extraRows = (i - CorridorSize) / 4 + 1;
                    var extraSteps = Math.Abs(currentCol - j) + extraRows;
                    var extraCost = extraSteps * CostOfType(t);
                    // PrintState(newState, nrPerType, 4);
                    queue.Enqueue((newState, prevStatesCopy), cost + extraCost);
                }

                // Move right
                for (var j = currentCol + 1; j < CorridorSize; j++)
                {
                    if (state[j] != Tile.Empty) break;
                    if (ForbiddenFields.Contains(j)) continue;

                    var newState = (Tile[])state.Clone();
                    newState[i] = Tile.Empty;
                    newState[j] = t;

                    var extraRows = (i - CorridorSize) / 4 + 1;
                    var extraSteps = Math.Abs(currentCol - j) + extraRows;
                    var extraCost = extraSteps * CostOfType(t);
                    // PrintState(newState, nrPerType, 4);
                    queue.Enqueue((newState, prevStatesCopy), cost + extraCost);
                }
            }
        }

        return -1;
    }

    private static void PrintState(Tile[] state, int nrPerType, int indent = 0)
    {
        var prefix = new string(' ', indent);
        Console.Write(prefix);
        Console.WriteLine("#############");
        Console.Write(prefix);
        Console.Write("#");
        for (var i = 0; i < CorridorSize; i++)
        {
            Console.Write(state[i] switch
            {
                Tile.Empty => '.',
                Tile.A => 'A',
                Tile.B => 'B',
                Tile.C => 'C',
                Tile.D => 'D',
                _ => throw new UnreachableException()
            });
        }
        Console.WriteLine("#");

        for (var j = 0; j < nrPerType; j++)
        {
            Console.Write(prefix);
            Console.Write(j == 0 ? "###" : "  #");
            for (var i = 0; i < 4; i++)
            {
                Console.Write(state[CorridorSize + 4 * j + i] switch
                {
                    Tile.Empty => '.',
                    Tile.A => 'A',
                    Tile.B => 'B',
                    Tile.C => 'C',
                    Tile.D => 'D',
                    _ => throw new UnreachableException()
                });
                Console.Write("#");
            }

            if (j == 0)
            {
                Console.Write("##");
            }
            Console.WriteLine();
        }
        Console.Write(prefix);
        Console.WriteLine("  #########");
    }

    private static int CostOfType(Tile t)
    {
        return t switch
        {
            Tile.A => 1,
            Tile.B => 10,
            Tile.C => 100,
            Tile.D => 1000,
            _ => throw new UnreachableException()
        };
    }

    private static int[] RoomMoves(IReadOnlyList<Tile> state, int nrPerType)
    {
        int[] rooms =
        {
            -1,
            -1,
            -1,
            -1
        };

        for (var i = 0; i < 4; i++)
        {
            var targetTile = (Tile)i;
            var isOkay = true;
            for (var j = 0; j < nrPerType; j++)
            {
                var tile = state[CorridorSize + j * 4 + i];
                if (tile != Tile.Empty && tile != targetTile)
                {
                    isOkay = false;
                    break;
                }

                if (tile == Tile.Empty)
                {
                    rooms[i] = j;
                }
            }

            if (!isOkay)
            {
                rooms[i] = -1;
            }
        }

        return rooms;
    }

    public object RunTask2(string[] inputText)
    {
        return 0;
    }

    private static ulong SerializeState(Tile[] state)
    {
        ulong result = 0;
        for (var i = 0; i < FieldSize; i++)
        {
            result += (ulong)state[i] * (ulong)Math.Pow(5, FieldSize - i - 1);
        }

        return result;
    }

    private static Tile[] ParseInput(string[] inputText)
    {
        var state = new Tile[FieldSize];
        int i;
        for (i = 0; i < FieldSize; i++)
        {
            state[i] = Tile.Empty;
        }

        i = CorridorSize;
        foreach (var line in inputText[2..])
        {
            foreach (var c in line.Where(c => c is 'A' or 'B' or 'C' or 'D'))
            {
                state[i] = c switch
                {
                    'A' => Tile.A,
                    'B' => Tile.B,
                    'C' => Tile.C,
                    'D' => Tile.D,
                    _ => throw new UnreachableException()
                };
                i++;
            }
        }

        return state;
    }
}