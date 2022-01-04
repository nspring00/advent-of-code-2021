using System.Diagnostics;
using System.Reflection;
using AdventOfCode2021.Challenges;

namespace AdventOfCode2021;

public class AocRunner<T>
{
    private readonly string _inputDirectory;

    public AocRunner(string inputDirectory)
    {
        _inputDirectory = inputDirectory;
    }

    public void RunAllChallenges(int lastDay = 24)
    {
        for (var i = 1; i <= lastDay; i++)
        {
            RunChallenge(i);
        }
    }

    public void RunChallenge(int day)
    {
        var typeParam = typeof(T);
        var assemblyName = typeParam.Assembly.GetName().Name;
        if (assemblyName == null)
        {
            Console.WriteLine($"Failed to read assemble of type {typeParam}.");
        }

        var challengeClassName = $"{assemblyName}.Challenges.Challenge{day}.Challenge{day}";
        var challengeType = Assembly.GetExecutingAssembly().GetType(challengeClassName);
        var x = Assembly.GetExecutingAssembly().GetTypes();
        if (challengeType is null)
        {
            Console.WriteLine($"Failed to load class {challengeClassName}.");
            return;
        }

        var challenge = (IAocChallenge)Activator.CreateInstance(challengeType)!;


        var inputTextFile = Path.Combine(_inputDirectory, $"input{day}.txt");
        if (!File.Exists(inputTextFile))
        {
            Console.WriteLine($"Can not find input file {inputTextFile}.");
            return;
        }

        var inputText1 = File.ReadAllLines(inputTextFile);
        var inputText2 = (string[])inputText1.Clone();

        var stopwatch1 = new Stopwatch();
        var stopwatch2 = new Stopwatch();

        stopwatch1.Start();
        var result1 = challenge.RunTask1(inputText1);
        stopwatch1.Stop();

        stopwatch2.Start();
        var result2 = challenge.RunTask2(inputText1);
        stopwatch2.Stop();

        Console.WriteLine($"---------------------- Day {day} ----------------------");
        Console.WriteLine($"Part 1: took {stopwatch1.ElapsedMilliseconds / 1000.0:0.###} s\t\tResult 1: {result1}");
        Console.WriteLine($"Part 2: took {stopwatch2.ElapsedMilliseconds / 1000.0:0.###} s\t\tResult 2: {result2}");
    }
}