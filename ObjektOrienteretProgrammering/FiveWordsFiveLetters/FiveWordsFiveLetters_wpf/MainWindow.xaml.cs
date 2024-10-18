using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using script;

namespace FiveWordsFiveLetters_wpf
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private CombinationFinder combinationFinder;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            combinationFinder = new CombinationFinder(); // Initialize the class from the library
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

                worker.RunWorkerAsync();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bgWorker = sender as BackgroundWorker;

            string filePath = @"C:\Users\HFGF\Documents\GitHub\H2\ObjektOrienteretProgrammering\FiveWordsFiveLetters\FiveWordsFiveLetters_console\Words.txt";
            int wordLength = 5; // Set your desired word length
            int numWords = 5; // Set the number of words in the combination
            var allCombinations = new ConcurrentBag<string>();

            combinationFinder.RunCombinationSearch(filePath, wordLength, numWords, allCombinations,
                (progress, time, combinations) => bgWorker.ReportProgress(progress, new { Count = combinations.Count, Time = time }),
                (time, count) => e.Result = new { Combinations = count, Time = time });
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Update the progress bar and percentage label
            ProgressBar.Value = e.ProgressPercentage;
            PercentLabel.Content = $"Percent: {ProgressBar.Value}%";

            dynamic state = e.UserState;
            CombinationsLabel.Content = $"Combinations: {state.Count}";
            TimeSpan elapsed = state.Time;
            TimeLabel.Content = $"Time: {elapsed.Minutes}m {elapsed.Seconds}s {elapsed.Milliseconds}ms";
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
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
    }
}
