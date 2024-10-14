using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FiveWordsFiveLetters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Stopwatch sw = Stopwatch.StartNew();

            string filePath = @"C:\Users\HFGF\Documents\GitHub\H2\ObjektOrienteretProgrammering\FiveWordsFiveLetters\Words.txt";
            string[] lines = File.ReadAllLines(filePath);

            // Filter words from file
            List<string> handledWords = lines.Where(line => IsValidWord(line)).ToList();

            ConcurrentBag<string> allCombinations = new ConcurrentBag<string>(); // Thread-safe collection

            // Find combinations of 5 words using parallel processing
            Parallel.ForEach(handledWords, (word, state) =>
            {
                List<string> selectedWords = new List<string> { word };
                HashSet<char> usedLetters = new HashSet<char>(word);

                FindFiveWords(handledWords, selectedWords, usedLetters, allCombinations, 0);
            });


            sw.Stop();
            // Output
            if (allCombinations.Count > 0)
            {
                Console.WriteLine("Found combinations of words:");
                foreach (var combo in allCombinations.Distinct())
                {
                    Console.WriteLine(combo);
                }
                Console.WriteLine($"Total combinations found: {allCombinations.Count}");
                Console.WriteLine(sw.ElapsedMilliseconds);
            }
            else
            {
                Console.WriteLine("No valid combination of words found.");
            }
        }

        private static bool IsValidWord(string text)
        {
            return text.Length == 5 && text.Distinct().Count() == text.Length;
        }

        private static void FindFiveWords(List<string> words, List<string> selectedWords, HashSet<char> usedLetters, ConcurrentBag<string> allCombinations, int startIndex)
        {
            // If 5 words selected, output them
            if (selectedWords.Count == 5)
            {
                allCombinations.Add(string.Join(", ", selectedWords)); // Add the combination to the bag
                return; // Return for more combinations
            }

            // Go through the list to find valid combinations
            for (int i = startIndex; i < words.Count; i++)
            {
                string word = words[i];

                // Check if word can be added
                if (CanAddWord(word, usedLetters))
                {
                    selectedWords.Add(word);

                    foreach (char c in word)
                    {
                        usedLetters.Add(c);
                    }

                    // Find next word
                    FindFiveWords(words, selectedWords, usedLetters, allCombinations, i + 1);

                    // Remove word and its chars
                    selectedWords.RemoveAt(selectedWords.Count - 1);
                    foreach (char c in word)
                    {
                        usedLetters.Remove(c);
                    }
                }
            }
        }

        private static bool CanAddWord(string word, HashSet<char> usedLetters)
        {
            return word.All(c => !usedLetters.Contains(c)); // Check if chars are already used
        }
    }
}
