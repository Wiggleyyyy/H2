using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using script;

namespace FiveWordsFiveLetters_wpf
{
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private CombinationFinder combinationFinder;
        private ConcurrentBag<string> allCombinations; // Store all combinations
        private TimeSpan elapsedTime; // Store elapsed time

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
            combinationFinder = new CombinationFinder(); // Initialize the class from the library
            allCombinations = new ConcurrentBag<string>(); // Initialize to store combinations
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
                DownloadButton.IsEnabled = false; // Disable download button during execution

                // Reset UI elements
                ProgressBar.Value = 0;
                PercentLabel.Content = "Percent: 0%";
                ResultLabel.Content = "Result: ";
                CombinationsLabel.Content = "Combinations: 0";
                TimeLabel.Content = "Time: 0s";

                allCombinations = new ConcurrentBag<string>(); // Clear previous combinations
                worker.RunWorkerAsync();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bgWorker = sender as BackgroundWorker;

            string filePath = @"C:\Users\HFGF\Documents\GitHub\H2\ObjektOrienteretProgrammering\FiveWordsFiveLetters\FiveWordsFiveLetters_console\Words.txt";
            int wordLength = 5; // Set your desired word length
            int numWords = 5; // Set the number of words in the combination

            combinationFinder.RunCombinationSearch(filePath, wordLength, numWords, allCombinations,
                (progress, time) => bgWorker.ReportProgress(progress, new { Count = allCombinations.Count, Time = time }),
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

                // Store the final elapsed time for use in download
                elapsedTime = result.Time;

                // Calculate the final elapsed time in minutes, seconds, and milliseconds
                TimeLabel.Content = $"Time: {elapsedTime.Minutes}m {elapsedTime.Seconds}s {elapsedTime.Milliseconds}ms";

                // Enable the download button after the task is complete
                DownloadButton.IsEnabled = true;
            }

            // Re-enable the Start button and reset its content
            StartButton.IsEnabled = true;
            StartButton.Content = "Start";
        }

        // Download button click handler
        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            // Create and configure the SaveFileDialog
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog
            {
                FileName = "Combinations", // Default file name
                DefaultExt = ".txt", // Default file extension
                Filter = "Text documents (.txt)|*.txt" // Filter files by extension
            };

            // Show save file dialog
            bool? result = saveFileDialog.ShowDialog();

            // If the user clicked "Save"
            if (result == true)
            {
                string savePath = saveFileDialog.FileName;

                try
                {
                    using (StreamWriter writer = new StreamWriter(savePath))
                    {
                        // Write each combination to the file
                        foreach (var combination in allCombinations)
                        {
                            writer.WriteLine(combination);
                        }

                        // Write the summary information at the bottom of the file
                        writer.WriteLine();
                        writer.WriteLine($"Total Combinations: {allCombinations.Count}");
                        writer.WriteLine($"Elapsed Time: {elapsedTime.Minutes}m {elapsedTime.Seconds}s {elapsedTime.Milliseconds}ms");
                    }

                    MessageBox.Show("Combinations successfully saved to file!", "Download Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error saving file: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
