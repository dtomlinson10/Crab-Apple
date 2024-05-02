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
            RefreshTable();

            personList.Clear();
            // adding new people to the personList 
            personList.Add(new RegularUser("001", "Daniel", "Tomlinson", "dtomlinson10@outlook.com", "password"));
            personList.Add(new RegularUser("002", "Harvey", "Walker", "harveywalker500@gmail.com", "password2"));
            //creating an admin
            Admin administrator = new Admin("001", "Harvey", "Walker", "harveywalker500@gmail.com", "password");
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

            // setting the property visibility to hidden or visible depending on the required display
            leaderboard_display.Visibility = Visibility.Hidden;
            leaderboard_heading.Visibility = Visibility.Hidden;
            streak_counter_heading.Visibility = Visibility.Hidden;
            notloggedInError.Visibility = Visibility.Visible;
        }

        private void ShowAdminFunctionality(MainWindow mainWindow)
        {
            autoAssign.Visibility = Visibility.Visible;
            ManualAssign.Visibility = Visibility.Visible;
            notLoggedInError.Visibility = Visibility.Hidden;

            // setting the property visibility to hidden or visible depending on the required display
            leaderboard_display.Visibility = Visibility.Visible;
            leaderboard_heading.Visibility = Visibility.Visible;
            streak_counter_heading.Visibility = Visibility.Visible;
            notloggedInError.Visibility = Visibility.Hidden;
        }

        private void AutoAssignChores(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException(Name + " is not implemented yet.");
        }

        // dans code
        private List<RegularUser> personList = new List<RegularUser>();
        private List<RegularUser> userInfo = new List<RegularUser>();
        private List<string> box = new List<string>();
        private List<int> choresCompleted = new List<int>();

        private void RefreshTable()
        {
            personList.Clear();
            // adding a new person to the personList 
            personList.Add(new RegularUser("1","Daniel","Tomlinson","dtomlinson10@outlook.com","password"));

            // displaying the firstname of the people in the list 
            // in the combo box for selection
            this.names_display.ItemsSource = personList;
            this.names_display.DisplayMemberPath = "firstname";

            // displaying all the people in the leaderboard 
            // with all the deatils from the database
            this.leaderboard_display.ItemsSource = personList;
        }

        private void names_display_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
