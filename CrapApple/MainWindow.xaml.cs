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

            Admin administrator = new Admin("001", "Harvey", "Walker", "harveywalker500@gmail.com", "password");
            //RegularUser user = new RegularUser("Tom", "002", "password");
            bool loggedIn = true;
            if (loggedIn)
            {
                ShowAdminFunctionality(this);
            }
            else
            {
                HideAdminFunctionality(this);
            }

            
        }

        private void HideAdminFunctionality(MainWindow mainWindow)
        {
            autoAssign.Visibility = Visibility.Hidden;
            ManualAssign.Visibility = Visibility.Hidden;
            notLoggedInError.Visibility = Visibility.Visible;
        }

        private void ShowAdminFunctionality(MainWindow mainWindow)
        {
            autoAssign.Visibility = Visibility.Visible;
            ManualAssign.Visibility = Visibility.Visible;
            notLoggedInError.Visibility = Visibility.Hidden;
        }

        private void AutoAssignChores(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException(Name + " is not implemented yet.");
        }
    }
}
