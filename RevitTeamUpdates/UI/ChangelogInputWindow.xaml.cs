using System.Windows;

namespace TeamUpdates.UI
{
    /// <summary>
    /// Dialog for entering changelog text
    /// </summary>
    public partial class ChangelogInputWindow : Window
    {
        public string ChangelogText { get; private set; }

        public ChangelogInputWindow(string modelName = null)
        {
            InitializeComponent();

            if (!string.IsNullOrEmpty(modelName))
            {
                Title = $"Sync with Changelog - {modelName}";
            }

            ChangelogTextBox.Focus();
        }

        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            var text = ChangelogTextBox.Text?.Trim();
            
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Please enter a changelog description.", 
                              "Required", 
                              MessageBoxButton.OK, 
                              MessageBoxImage.Warning);
                return;
            }

            ChangelogText = text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
