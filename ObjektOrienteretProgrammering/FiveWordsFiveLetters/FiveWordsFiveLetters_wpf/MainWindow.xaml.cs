using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;

namespace FiveWordsFiveLetters_wpf
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private Stopwatch stopwatch;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!worker.IsBusy)
            {
                // Disable the Start button and set it to "Running"
                StartButton.IsEnabled = false;
                StartButton.Content = "Running...";

                // Reset UI elements
                ProgressBar.Value = 0;
                PercentLabel.Content = "Percent: 0%";
                ResultLabel.Content = "Result: ";
                CombinationsLabel.Content = "Combinations: 0";
                TimeLabel.Content = "Time: 0s";

                stopwatch = Stopwatch.StartNew(); // Start the stopwatch to measure elapsed time

                worker.RunWorkerAsync();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bgWorker = sender as BackgroundWorker;

            // Load and pre-filter words
            List<string> words = new List<string>();
            using (var file = new StreamReader(@"C:\Users\HFGF\Documents\GitHub\H2\ObjektOrienteretProgrammering\FiveWordsFiveLetters\FiveWordsFiveLetters_console/Words.txt"))
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

                // Update progress
                if (++completedIterations % 100 == 0)
                {
                    // Report progress back to the UI thread, including the current time
                    bgWorker.ReportProgress((completedIterations * 100) / totalWords, new { Count = allCombinations.Count, Time = stopwatch.Elapsed });
                }
            });

            e.Result = new { Combinations = allCombinations.Count, Time = stopwatch.Elapsed };
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the progress bar and percentage label
            ProgressBar.Value = e.ProgressPercentage;
            PercentLabel.Content = $"Percent: {e.ProgressPercentage}%";

            // Extract the combination count and time from the UserState object
            dynamic state = e.UserState;
            CombinationsLabel.Content = $"Combinations: {state.Count}";
            TimeSpan elapsed = state.Time;
            TimeLabel.Content = $"Time: {elapsed.Minutes}m {elapsed.Seconds}s {elapsed.Milliseconds}ms";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            stopwatch.Stop(); // Stop the stopwatch when work is done

            if (e.Error != null)
            {
                ResultLabel.Content = $"Error: {e.Error.Message}";
            }
            else if (e.Cancelled)
            {
                ResultLabel.Content = "Operation was canceled.";
            }
            else
            {
                var result = (dynamic)e.Result;
                CombinationsLabel.Content = $"Combinations: {result.Combinations}";

                // Calculate the final elapsed time in minutes, seconds, and milliseconds
                TimeSpan elapsed = result.Time;
                TimeLabel.Content = $"Time: {elapsed.Minutes}m {elapsed.Seconds}s {elapsed.Milliseconds}ms";
            }

            // Re-enable the Start button and reset its content
            StartButton.IsEnabled = true;
            StartButton.Content = "Start";
        }

        // Functions for the bitmask and combination logic
        private static List<int> GetWordBitmasks(List<string> words)
        {
            var wordBitmasks = new List<int>(words.Count);
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

        private static void FindCombinations(List<string> words, List<int> wordBitmasks, List<string> selectedWords, int usedBitmask, int startIndex, ConcurrentBag<string> allCombinations)
        {
            if (selectedWords.Count == 5)
            {
                allCombinations.Add(string.Join(", ", selectedWords));
                return;
            }

            for (int i = startIndex; i < words.Count; i++)
            {
                int wordBitmask = wordBitmasks[i];
                if ((usedBitmask & wordBitmask) != 0) continue;

                selectedWords.Add(words[i]);
                int newUsedBitmask = usedBitmask | wordBitmask;

                FindCombinations(words, wordBitmasks, selectedWords, newUsedBitmask, i + 1, allCombinations);

                selectedWords.RemoveAt(selectedWords.Count - 1); // Backtrack
            }
        }
    }
}
