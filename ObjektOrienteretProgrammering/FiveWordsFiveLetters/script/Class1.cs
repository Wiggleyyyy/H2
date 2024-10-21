using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace script
{
    public class CombinationFinder
    {
        // Generates bitmask representations of words
        public List<int> GetWordBitmasks(List<string> words)
        {
            var charBitmasks = new int[26];
            for (char c = 'a'; c <= 'z'; c++)
            {
                charBitmasks[c - 'a'] = 1 << (c - 'a');
            }

            var wordBitmasks = new List<int>(words.Count);
            foreach (var word in words)
            {
                int bitmask = 0;
                foreach (var c in word)
                {
                    bitmask |= charBitmasks[c - 'a'];
                }
                wordBitmasks.Add(bitmask);
            }
            return wordBitmasks;
        }

        // Recursive method to find combinations of words
        public void FindCombinations(List<string> words, List<int> wordBitmasks, string[] selectedWords, int depth, int usedBitmask, int startIndex, ConcurrentBag<string> allCombinations, int numWords)
        {
            if (depth == numWords)
            {
                allCombinations.Add(string.Join(", ", selectedWords.Take(numWords)));
                return;
            }

            for (int i = startIndex; i < words.Count; i++)
            {
                int wordBitmask = wordBitmasks[i];
                if ((usedBitmask & wordBitmask) != 0) continue; // Skip if there's a conflicting bitmask

                selectedWords[depth] = words[i];
                int newUsedBitmask = usedBitmask | wordBitmask;

                // Recursively find the next combination
                FindCombinations(words, wordBitmasks, selectedWords, depth + 1, newUsedBitmask, i + 1, allCombinations, numWords);
            }
        }

        // Load words from the file
        public List<string> LoadWords(string filePath, int wordLength)
        {
            var words = new List<string>();
            try
            {
                using (var file = new StreamReader(filePath))
                {
                    string line;
                    while ((line = file.ReadLine()) != null)
                    {
                        if (line.Length == wordLength && line.Distinct().Count() == wordLength)
                        {
                            words.Add(line);
                        }
                    }
                }
            }
            catch (IOException ex)
            {
                Debug.WriteLine($"Error reading file {filePath}: {ex.Message}");
                throw; // Re-throw the exception to be handled by the calling method
            }
            return words;
        }

        // Main method to run combination search
        public void RunCombinationSearch(string filePath, int wordLength, int numWords, ConcurrentBag<string> allCombinations, Action<int, TimeSpan> reportProgress, Action<TimeSpan, int> onComplete)
        {
            // Load words and generate bitmasks
            var words = LoadWords(filePath, wordLength);
            var wordBitmasks = GetWordBitmasks(words);

            int totalWords = words.Count;
            int completedIterations = 0;

            Stopwatch stopwatch = Stopwatch.StartNew();

            // Use Parallel.ForEach to process words concurrently
            Parallel.ForEach(Partitioner.Create(0, totalWords), range =>
            {
                try
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        // Create an array for selectedWords
                        var selectedWords = new string[numWords];
                        selectedWords[0] = words[i]; // Initialize the first word

                        // Find combinations for the current word
                        FindCombinations(words, wordBitmasks, selectedWords, 1, wordBitmasks[i], i + 1, allCombinations, numWords);

                        // Atomically increment and calculate progress
                        int currentIteration = System.Threading.Interlocked.Increment(ref completedIterations);
                        if (currentIteration % 100 == 0)
                        {
                            reportProgress((currentIteration * 100) / totalWords, stopwatch.Elapsed);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error processing word range {range.Item1}-{range.Item2}: {ex.Message}");
                }
            });

            stopwatch.Stop();
            onComplete(stopwatch.Elapsed, allCombinations.Count); // Report completion
        }
    }
}
