using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CrapApple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            bool loggedIn = false; // Placeholder for debugging
            Debug.WriteLine("loggedIn = " + loggedIn);
            // Displays warning if user is not logged in
            if (loggedIn)
            {
                autoAssign.Visibility = Visibility.Visible;
                ManualAssign.Visibility = Visibility.Visible;
                notLoggedInError.Visibility = Visibility.Hidden;
            }
            else
            {
                autoAssign.Visibility = Visibility.Hidden;
                ManualAssign.Visibility = Visibility.Hidden;
                notLoggedInError.Visibility = Visibility.Visible;
            }
        }

        private void AutoAssignChores(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException(Name + " is not implemented yet.");
        }
    }
}
