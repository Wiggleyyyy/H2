using System;
using System.Collections.Generic;
using System.IO;

namespace FiveWordsFiveLetters
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string filePath = @"C:\Users\HFGF\Documents\GitHub\H2\ObjektOrienteretProgrammering\FiveWordsFiveLetters\Words.txt";
            string[] lines = File.ReadAllLines(filePath);

            List<string> handledWords = new List<string>();

            // Filter words from file
            foreach (string line in lines)
            {
                if (IsValidWord(line))
                {
                    handledWords.Add(line);
                }
            }

            List<string> selectedWords = new List<string>();
            HashSet<char> usedLetters = new HashSet<char>();
            List<string> allCombinations = new List<string>(); // To store all combinations

            // Find combinations of 5 words
            FindFiveWords(handledWords, selectedWords, usedLetters, 0, allCombinations);

            // Output
            if (allCombinations.Count > 0)
            {
                Console.WriteLine("Found combinations of words:");
                foreach (var combo in allCombinations)
                {
                    Console.WriteLine(combo);
                }
                Console.WriteLine($"Total combinations found: {allCombinations.Count}");
            }
            else
            {
                Console.WriteLine("No valid combination of words found.");
            }
        }

        private static bool IsValidWord(string text)
        {
            // Check if word is 5 chars
            if (text.Length != 5)
            {
                return false;
            }

            // Check for duplicate chars
            var charSet = new HashSet<char>();
            foreach (char c in text)
            {
                if (!charSet.Add(c))
                {
                    return false; // Duplicate
                }
            }

            return true; // Valid
        }

        private static void FindFiveWords(List<string> words, List<string> selectedWords, HashSet<char> usedLetters, int startIndex, List<string> allCombinations)
        {
            // If 5 words selected, output them
            if (selectedWords.Count == 5)
            {
                allCombinations.Add(string.Join(", ", selectedWords)); // Add the combination to the list
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
                    FindFiveWords(words, selectedWords, usedLetters, i + 1, allCombinations);

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
            // Check if chars are already used
            foreach (char c in word)
            {
                if (usedLetters.Contains(c))
                {
                    return false; // Invalid
                }
            }
            return true; // Valid
        }
    }
}