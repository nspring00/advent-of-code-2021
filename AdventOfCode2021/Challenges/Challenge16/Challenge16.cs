namespace AdventOfCode2021.Challenges.Challenge16;

public class Challenge16 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var input = ParseInput(inputText);

        var (packet, _) = ParsePacket(input);

        return packet.VersionSum();
    }

    public object RunTask2(string[] inputText)
    {
        var input = ParseInput(inputText);

        var (packet, _) = ParsePacket(input);

        return packet.Evaluate();
    }

    private static (Packet, long bitLength) ParsePacket(Span<char> data)
    {
        var version = ExtractNumber(data[..3]);
        var typeId = ExtractNumber(data[3..6]);

        if (typeId == 4)
        {
            return ParseLiteral(version, data);
        }

        return ParseOperator(version, typeId, data);
    }

    private static (Literal, long bitLength) ParseLiteral(int version, Span<char> data)
    {
        const int typeId = 4;

        var value = 0L;
        var running = true;
        var offset = 6;
        while (running)
        {
            if (data[offset] == '0')
            {
                running = false;
            }

            for (var i = 1; i <= 4; i++)
            {
                value *= 2;
                if (data[offset + i] == '1')
                {
                    value++;
                }
            }

            offset += 5;
        }

        return (new Literal(version, typeId, value), offset);
    }

    private static (Operator, long bitLength) ParseOperator(int version, int typeId, Span<char> data)
    {
        var lengthTypeId = data[6] == '0' ? 0 : 1;
        var op = new Operator(version, typeId);
        long offset;

        if (lengthTypeId == 0)
        {
            var subPacketLength = ExtractNumber(data[7..22]);

            offset = 0L;
            while (subPacketLength - offset > 0)
            {
                var (packet, length) = ParsePacket(data[(22 + (int)offset)..]);
                op.Packets.Add(packet);
                offset += length;
            }

            return (op, subPacketLength + 22);
        }

        var subPacketCount = ExtractNumber(data[7..18]);
        offset = 18;
        for (var i = 0; i < subPacketCount; i++)
        {
            var (packet, length) = ParsePacket(data[(int)offset..]);
            op.Packets.Add(packet);
            offset += length;
        }

        return (op, offset);
    }

    private static int ExtractNumber(Span<char> span)
    {
        return Convert.ToInt32(span.ToString(), 2);
    }

    private static Span<char> ParseInput(IEnumerable<string> inputText)
    {
        var dict = new Dictionary<char, string>
        {
            {
                '0', "0000"
            },
            {
                '1', "0001"
            },
            {
                '2', "0010"
            },
            {
                '3', "0011"
            },
            {
                '4', "0100"
            },
            {
                '5', "0101"
            },
            {
                '6', "0110"
            },
            {
                '7', "0111"
            },
            {
                '8', "1000"
            },
            {
                '9', "1001"
            },
            {
                'A', "1010"
            },
            {
                'B', "1011"
            },
            {
                'C', "1100"
            },
            {
                'D', "1101"
            },
            {
                'E', "1110"
            },
            {
                'F', "1111"
            }
        };

        var str = string.Join("", inputText.First().Select(x => dict[x]));
        return new Span<char>(str.ToCharArray());
    }
}

internal abstract record Packet(int Version, int TypeId)
{
    public abstract int VersionSum();

    public abstract long Evaluate();
}

internal record Literal(int Version, int TypeId, long Value) : Packet(Version, TypeId)
{
    public override int VersionSum()
    {
        return Version;
    }

    public override long Evaluate()
    {
        return Value;
    }
}

internal record Operator(int Version, int TypeId) : Packet(Version, TypeId)
{
    public IList<Packet> Packets { get; } = new List<Packet>();

    public override int VersionSum()
    {
        return Packets.Sum(x => x.VersionSum()) + Version;
    }

    public override long Evaluate()
    {
        var value = TypeId switch
        {
            0 => Packets.Sum(x => x.Evaluate()),
            1 => Packets.Aggregate(1L, (acc, packet) => acc * packet.Evaluate()),
            2 => Packets.Min(x => x.Evaluate()),
            3 => Packets.Max(x => x.Evaluate()),
            5 when Packets.Count == 2 => Packets[0].Evaluate() > Packets[1].Evaluate() ? 1 : 0,
            6 when Packets.Count == 2 => Packets[0].Evaluate() < Packets[1].Evaluate() ? 1 : 0,
            7 when Packets.Count == 2 => Packets[0].Evaluate() == Packets[1].Evaluate() ? 1 : 0,
            _ => throw new ArgumentOutOfRangeException()
        };

        return value;
    }
}