namespace AdventOfCode2021.Challenges.Challenge18;

public class Challenge18 : IAocChallenge
{
    public object RunTask1(string[] inputText)
    {
        var input = ParseInput(inputText);

        var value = input.Skip(1).Aggregate(input.First(), AddNodes);

        return value.Magnitude;
    }

    public object RunTask2(string[] inputText)
    {
        var input = ParseInput(inputText);

        var combinedInputs = input
            .SelectMany(first => input
                .Where(second => first != second)
                .Select(second => (first, second)))
                .ToList();

        var values = combinedInputs
            .Select(value => AddNodes(value.first, value.second))
            .ToList();

        var magnitudes = values
            .Select(x => x.Magnitude)
            .ToList();

        return magnitudes.Max();
    }

    private static PairNode AddNodes(Node left, Node right)
    {
        var node = new PairNode((Node)left.Clone(), (Node)right.Clone());
        node.Reduce();
        return node;
    }

    private static IList<Node> ParseInput(IEnumerable<string> inputText)
    {
        return inputText.Select(ParseInputLine).ToList();
    }

    private static Node ParseInputLine(string inputLine)
    {
        if (int.TryParse(inputLine, out var val))
        {
            return new ValueNode(val);
        }

        var index = 0;
        var running = true;
        var count = 0;
        while (running)
        {
            index++;
            var value = inputLine[index];
            switch (value)
            {
                case ',' when count == 0:
                    running = false;
                    break;
                case '[':
                    count++;
                    break;
                case ']':
                    count--;
                    break;
            }
        }

        var leftString = inputLine[1..index];
        var rightString = inputLine[(index + 1)..^1];

        return new PairNode(ParseInputLine(leftString), ParseInputLine(rightString));
    }
}

internal abstract class Node : ICloneable
{
    public abstract long Magnitude { get; }
    public abstract (bool ret, int addLeft, int addRight) ExplodeIfNecessary(int depth);
    public abstract void AddToLeftmost(int value);
    public abstract void AddToRightmost(int value);
    public abstract object Clone();
}

internal class PairNode : Node
{
    public PairNode(Node left, Node right)
    {
        Left = left;
        Right = right;
    }

    public void Reduce()
    {
        while (true)
        {
            //Console.WriteLine(this);
            var (explode, _, _) = ExplodeIfNecessary(0);
            if (explode)
            {
                continue;
            }

            if (SplitIfNecessary())
            {
                continue;
            }

            break;
        }
    }

    public bool SplitIfNecessary()
    {
        if (Left is ValueNode leftValueNode)
        {
            var value = leftValueNode.Value;
            if (value >= 10)
            {
                //Console.WriteLine($"Splitting value {value}");
                Left = new PairNode(
                    new ValueNode(value / 2),
                    new ValueNode(value / 2 + value % 2));

                return true;
            }
        }
        else
        {
            if (((PairNode)Left).SplitIfNecessary()) return true;
        }

        if (Right is not ValueNode rightValueNode) return ((PairNode)Right).SplitIfNecessary();

        var rightValue = rightValueNode.Value;
        if (rightValue < 10) return false;

        //Console.WriteLine($"Splitting value {rightValue}");
        Right = new PairNode(
            new ValueNode(rightValue / 2),
            new ValueNode(rightValue / 2 + rightValue % 2));

        return true;
    }

    public override long Magnitude => 3 * Left.Magnitude + 2 * Right.Magnitude;

    public override (bool ret, int addLeft, int addRight) ExplodeIfNecessary(int depth)
    {
        if (depth == 3)
        {
            if (Left is PairNode leftPair)
            {
                Left = new ValueNode(0);
                //Console.WriteLine($"Exploding {leftPair}");
                var leftNestedValue = ((ValueNode)leftPair.Left).Value;
                var rightNestedValue = ((ValueNode)leftPair.Right).Value;
                Right.AddToLeftmost(rightNestedValue);
                return (true, leftNestedValue, -1);
            }

            if (Right is PairNode rightPair)
            {
                Right = new ValueNode(0);
                //Console.WriteLine($"Exploding {rightPair}");
                var leftNestedValue = ((ValueNode)rightPair.Left).Value;
                var rightNestedValue = ((ValueNode)rightPair.Right).Value;
                Left.AddToRightmost(leftNestedValue);
                return (true, -1, rightNestedValue);
            }
        }

        var (ret, addLeft, addRight) = Left.ExplodeIfNecessary(depth + 1);
        if (ret)
        {
            if (addRight != -1)
            {
                Right.AddToLeftmost(addRight);
            }

            return (true, addLeft, -1);
        }

        (ret, addLeft, addRight) = Right.ExplodeIfNecessary(depth + 1);
        if (!ret) return (false, -1, -1);
        
        if (addLeft != -1)
        {
            Left.AddToRightmost(addLeft);
        }

        return (true, -1, addRight);

    }

    public override void AddToLeftmost(int value)
    {
        Left.AddToLeftmost(value);
    }

    public override void AddToRightmost(int value)
    {
        Right.AddToRightmost(value);
    }

    public override object Clone()
    {
        return new PairNode(
            (Node)Left.Clone(),
            (Node)Right.Clone()
        );
    }

    public Node Left { get; set; }
    public Node Right { get; set; }

    public override string ToString()
    {
        return $"[{Left},{Right}]";
    }
}

internal class ValueNode : Node
{
    public ValueNode(int value)
    {
        Value = value;
    }

    public int Value { get; private set; }

    public override long Magnitude => Value;

    public override (bool ret, int addLeft, int addRight) ExplodeIfNecessary(int depth)
    {
        return (false, -1, -1);
    }

    public override void AddToLeftmost(int value)
    {
        Value += value;
    }

    public override void AddToRightmost(int value)
    {
        Value += value;
    }

    public override object Clone()
    {
        return new ValueNode(Value);
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}