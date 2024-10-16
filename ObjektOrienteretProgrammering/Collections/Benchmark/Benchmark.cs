using BenchmarkDotNet.Attributes;
using Collections;
using System;

namespace Collections.Benchmark;
public class CollectionBenchmark
{
    private int[] _input;

    [GlobalSetup]
    public void Setup()
    {
        var dynamicSize = RNG.Range(50000, 250000);
        _input = new int[dynamicSize];
        for (var i = 0; i < dynamicSize; i++)
        {
            _input[i] = RNG.Range(0, dynamicSize + 1);
        }
    }

    [Benchmark]
    public void TestArrayFill()
    {
        var array = new int[_input.Length];
        for (int i = 0; i < _input.Length; i++)
        {
            array[i] = _input[i];
        }
    }

    [Benchmark]
    public void TestArraySort()
    {
        Array.Sort(_input);
    }
}
