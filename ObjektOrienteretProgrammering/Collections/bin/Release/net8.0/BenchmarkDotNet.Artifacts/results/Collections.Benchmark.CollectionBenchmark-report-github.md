```

BenchmarkDotNet v0.14.0, Windows 10 (10.0.19045.4780/22H2/2022Update)
Intel Core i7-8700 CPU 3.20GHz (Coffee Lake), 1 CPU, 12 logical and 6 physical cores
.NET SDK 8.0.403
  [Host]     : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.10 (8.0.1024.46610), X64 RyuJIT AVX2


```
| Method        | Mean       | Error   | StdDev  |
|-------------- |-----------:|--------:|--------:|
| TestArrayFill |   307.6 μs | 1.65 μs | 1.54 μs |
| TestArraySort | 1,707.3 μs | 6.73 μs | 5.97 μs |
