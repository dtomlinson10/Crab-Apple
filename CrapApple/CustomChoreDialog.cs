//V1

//logic & functionality for dialog window when adding custom chore
//CustomChoreDialog.xaml.cs
using System.Windows;

namespace CrapApple
{
    public partial class CustomChoreDialog : Window
    {
        public int EstimatedTime { get; private set; } = -1;
        public CustomChoreDialog()
        {
            InitializeComponent();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            //validate the estimated time input
            if (InputValidationUtil.ValidateEstimatedTime(estimatedTimeTextBox.Text, out int estimatedTime))
            {
                //set the EstimatedTime property and close the dialog if good result
                EstimatedTime = estimatedTime;
                DialogResult = true;
            }
            else
            {
                //error message if the input is not valid
                MessageBox.Show("Please enter a valid estimated time (non-negative integer).");
            }
        }
    }
}
