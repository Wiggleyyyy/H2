using BenchmarkDotNet.Running;
using Collections;
using Collections.Benchmark;
using Collections.Collections;
using Collections.DataModels;
using System;

public class Program
{
    public static void Main()
    {
        // Step 1: Initialize Random Number Generator
        Random random = new Random();

        // Step 2: Generate dynamic size for the test collections
        int dynamicSize = random.Next(50000, 250000);
        var input = new string[dynamicSize];

        // Generate random numbers and text as input
        for (var i = 0; i < dynamicSize; i++)
        {
            input[i] = random.Next(0, dynamicSize + 1).ToString();
        }

        // Step 3: Initialize CollectionTester with MyObject type
        var tests = new CollectionTester<MyObject>(
            input,
            x => new MyObject(int.Parse(x), $"Text_{x}"), // Function to convert string to MyObject
            x => x // No specific sorting logic applied here
        );

        // Step 4: Add different collections to the tester
        //tests.Add(new BaseLineCollection<MyObject>());
        //tests.Add(new ListCollection<MyObject>());
        //tests.Add(new LinkedListCollection<MyObject>());
        //tests.Add(new ArrayUnknownSizeCollection<MyObject>());
        tests.Add(new ConcurrentListCollection<MyObject>());

        // Step 5: Print the size of the collection being tested
        Console.WriteLine($"Testing collection size is {dynamicSize}.");

        // Step 6: Run all the collection tests
        tests.RunAllTest();

        // Step 7: Run the BenchmarkDotNet benchmark for the CollectionBenchmark class
        //BenchmarkRunner.Run<CollectionBenchmark>();

        // Wait for user input to exit
        Console.WriteLine("Press any key...");
        Console.ReadKey();
    }
}
