using System.Windows;

namespace TeamUpdates.UI
{
    /// <summary>
    /// Dialog for selecting date range for reports
    /// </summary>
    public partial class DateRangeWindow : Window
    {
        public int Days { get; private set; } = 7;

        public DateRangeWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            // Map selection to days
            Days = PresetComboBox.SelectedIndex switch
            {
                0 => 1,   // Last Day
                1 => 7,   // Last Week
                2 => 14,  // Last 2 Weeks
                3 => 30,  // Last Month
                _ => 7
            };

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
