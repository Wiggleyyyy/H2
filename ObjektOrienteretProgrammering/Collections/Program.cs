using BenchmarkDotNet.Running;
using Collections;
using Collections.Benchmark;
using Collections.Collections;
using System.Security.Cryptography.X509Certificates;
/// <summary>
/// ConsoleProgram testing different collections
/// Size of collections is between 50000 to 250000 (Same size for all)
/// CollectionTest class has to inharent from CollectionTestClass
/// Every test have to call, FillCollection, SortCollection and PrintCollection.
/// </summary>
var dynamicSize = RNG.Range(50000, 250000);
var input = new string[dynamicSize];
for (var i = 0; i < dynamicSize; i++)
{
    input[i] = RNG.Range(0, dynamicSize + 1).ToString();
}

var tests = new CollectionTester<int>(input, x => int.Parse(x), x => x);

//Add collections
tests.Add(new BaseLineCollection<int>());
//tests.Add(new ListCollection<int>());
//tests.Add(new LinkedListCollection<int>());
//tests.Add(new ArrayUnknownSizeCollection<int>());

//Test all collections.
Console.WriteLine($"Testing collection size is {dynamicSize}.");
tests.RunAllTest();
//BenchmarkRunner.Run<CollectionBenchmark>();
Console.WriteLine("Press anykey...");
Console.ReadKey();

int StringToInt(string s)
{
    return int.Parse(s);
}