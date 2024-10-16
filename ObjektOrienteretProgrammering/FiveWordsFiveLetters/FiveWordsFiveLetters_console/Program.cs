using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveWordsFiveLetters
{
    internal class Program
    {
        private const int AlphabetSize = 26;
        private static object progressLock = new object();  // Lock for thread-safe progress bar update
        private const int ProgressUpdateInterval = 500;  // Increase interval to minimize locking

        static void Main(string[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            // Load and pre-filter words efficiently
            var words = new List<string>();
            using (var file = new StreamReader(@"C:\Users\HFGF\Documents\GitHub\H2\ObjektOrienteretProgrammering\FiveWordsFiveLetters\Words.txt"))
            {
                string line;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length == 5 && line.Distinct().Count() == 5)
                    {
                        words.Add(line);
                    }
                }
            }

            // Prepare all bitmask representations of the words
            var wordBitmasks = GetWordBitmasks(words);
            var allCombinations = new ConcurrentBag<string>(); // Thread-safe collection
            int totalWords = words.Count;
            int completedIterations = 0;

            // Use Parallel.For to run multiple searches in parallel
            Parallel.For(0, totalWords, i =>
            {
                FindCombinations(words, wordBitmasks, new List<string> { words[i] }, wordBitmasks[i], i + 1, allCombinations);

                // Update progress less frequently to reduce lock contention
                if (++completedIterations % ProgressUpdateInterval == 0)
                {
                    ReportProgress(completedIterations, totalWords);
                }
            });

            // Final progress update
            ReportProgress(totalWords, totalWords);

            stopwatch.Stop();

            // Output the results
            if (allCombinations.Count > 0)
            {
                Console.WriteLine("\nFound combinations of words:");
                foreach (var combination in allCombinations)
                {
                    Console.WriteLine(combination);
                }
                Console.WriteLine($"Total combinations found: {allCombinations.Count}");
            }
            else
            {
                Console.WriteLine("\nNo valid combination of words found.");
            }

            Console.WriteLine($"Time taken: {stopwatch.ElapsedMilliseconds} ms");
        }

        private static void FindCombinations(List<string> words, List<int> wordBitmasks, List<string> selectedWords, int usedBitmask, int startIndex, ConcurrentBag<string> allCombinations)
        {
            // If 5 words are selected, add to combinations and return
            if (selectedWords.Count == 5)
            {
                allCombinations.Add(string.Join(", ", selectedWords));
                return;
            }

            // Optimization: Avoid unnecessary recursion if the number of remaining words is insufficient
            if (selectedWords.Count + (words.Count - startIndex) < 5)
            {
                return; // Early exit, not enough words left to form a combination
            }

            // Try adding more words recursively
            for (int i = startIndex; i < words.Count; i++)
            {
                int wordBitmask = wordBitmasks[i];

                // Early exit if the word can't be added without overlapping letters
                if ((usedBitmask & wordBitmask) != 0) continue;

                selectedWords.Add(words[i]);

                // Update the bitmask with the new word's bitmask
                int newUsedBitmask = usedBitmask | wordBitmask;

                // Recur to find more combinations
                FindCombinations(words, wordBitmasks, selectedWords, newUsedBitmask, i + 1, allCombinations);

                // Backtrack: remove the last word to try other combinations
                selectedWords.RemoveAt(selectedWords.Count - 1);
            }
        }

        private static List<int> GetWordBitmasks(List<string> words)
        {
            var wordBitmasks = new List<int>(words.Count);

            // Generate a bitmask for each word
            foreach (var word in words)
            {
                int bitmask = 0;
                foreach (var c in word)
                {
                    bitmask |= (1 << (c - 'a'));
                }
                wordBitmasks.Add(bitmask);
            }

            return wordBitmasks;
        }

        private static void ReportProgress(int completedIterations, int totalWords)
        {
            lock (progressLock)
            {
                int progressPercentage = (completedIterations * 100) / totalWords;
                DrawTextProgressBar(progressPercentage);
            }
        }

        private static void DrawTextProgressBar(int progress)
        {
            // Create a text-based progress bar
            const int barSize = 50;
            int filledBars = (progress * barSize) / 100;
            string progressBar = new string('#', filledBars) + new string('-', barSize - filledBars);

            // Display progress
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write($"[{progressBar}] {progress}% Complete");
        }
    }
}
