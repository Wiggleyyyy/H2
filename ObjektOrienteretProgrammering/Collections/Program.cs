using Collections;
using Collections.Collections;
using System.Security.Cryptography.X509Certificates;

var dynamicSize = RNG.Range(50000, 250000);
var input = new string[dynamicSize];
for (var i = 0; i < dynamicSize; i++)
{
    input[i] = RNG.Range(0, dynamicSize + 1).ToString();
}

var tests = new CollectionTester<int>(input, x => int.Parse(x), x => x);

//Add collections
tests.Add(new ConcreteCollection<int>());
/*tests.Add(new ListCollection<int>());
tests.Add(new LinkedListCollection<int>());
tests.Add(new ArrayUnknownSizeCollection<int>());*/

//Test all collections.
Console.WriteLine($"Testing collection size is {dynamicSize}.");
tests.RunAllTest();
Console.WriteLine("Press anykey...");
Console.ReadKey();

int StringToInt(string s)
{
    return int.Parse(s);
}