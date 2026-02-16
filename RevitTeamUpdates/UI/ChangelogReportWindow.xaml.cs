using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace TeamUpdates.UI
{
    /// <summary>
    /// Dialog for viewing changelog reports
    /// </summary>
    public partial class ChangelogReportWindow : Window
    {
        private readonly string _reportText;

        public ChangelogReportWindow(string reportText)
        {
            InitializeComponent();

            _reportText = reportText;
            ReportTextBlock.Text = reportText;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Clipboard.SetText(_reportText);
                MessageBox.Show("Report copied to clipboard.", 
                              "Success", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error copying to clipboard: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog
                {
                    Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
                    FileName = $"changelog_report_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };

                if (saveDialog.ShowDialog() == true)
                {
                    File.WriteAllText(saveDialog.FileName, _reportText);
                    MessageBox.Show("Report exported successfully.", 
                                  "Success", 
                                  MessageBoxButton.OK, 
                                  MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting report: {ex.Message}", 
                              "Error", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Error);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
