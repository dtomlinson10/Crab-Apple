using System;
using System.Collections.Generic;
using System.Data.Common;
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
            addUserInfo("daniel");

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
            assignChoresLoggedInError.Visibility = Visibility.Visible;

            // setting the property visibility to hidden or visible depending on the required display for motivation tab
            Leaderboard_Layout_Grid.Visibility = Visibility.Hidden;
            names_display.Visibility = Visibility.Hidden;
            UserInfoGrid.Visibility = Visibility.Hidden;    
            LogInError.Visibility = Visibility.Visible;

            //setting visibility of stats tab
            statLogInError.Visibility = Visibility.Visible;

        }

        private void ShowAdminFunctionality(MainWindow mainWindow)
        {
            autoAssign.Visibility = Visibility.Visible;
            ManualAssign.Visibility = Visibility.Visible;
            assignChoresLoggedInError.Visibility = Visibility.Hidden;

            // setting the property visibility to hidden or visible depending on the required display for motivation tab
            Leaderboard_Layout_Grid.Visibility = Visibility.Visible;
            names_display.Visibility = Visibility.Visible;  
            UserInfoGrid.Visibility = Visibility.Visible;
            LogInError.Visibility = Visibility.Hidden;

            //setting the visibility of the stats tab
            statLogInError.Visibility = Visibility.Hidden;
        }

        private void AutoAssignChores(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException(Name + " is not implemented yet.");
        }

        //login retrieval 
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = loginEmail.Text;
            string password = loginPassword.Text;

            if(email != "Email" && password != "Password")
            {
                if(email == "daniel" && password == "password")
                {
                    ShowAdminFunctionality(this);
                }
                else
                {
                    HideAdminFunctionality(this);
                }
            }
            else
            {
                HideAdminFunctionality(this);
            }
        }

        //dans code
        private const string V = "";
        private List<RegularUser> personList = new List<RegularUser>();
        private List<RegularUser> userInfo = new List<RegularUser>();
        private List<string> box = new List<string>();
        private List<int> choresCompleted = new List<int>();

        //function to add details to user info boxes
        private void addUserInfo(string user)
        {
            if(user == null)
            {
                firstname_display.Text = V;
                lastname_display.Text = V;
                idDisplay.Text = V;
                email_display.Text = V;
                choresassigned_display.Text = V;
                chorestotal_display.Text = V;
                choresCompleted_display.Text = V;
            }
            else
            {
                firstname_display.Text = user;
                lastname_display.Text = user;
                idDisplay.Text = user;
                email_display.Text = user;
                choresassigned_display.Text = user;
                chorestotal_display.Text = user;
                choresCompleted_display.Text = user;
            }
            
        }

        private void RefreshTable()
        {
            // displaying the firstname of the people in the list 
            // in the combo box for selection
            this.names_display.ItemsSource = personList;
            this.names_display.DisplayMemberPath = "forename";

            // displaying all the people in the leaderboard 
            // with all the deatils from the database
            this.leaderboard_display.ItemsSource = personList;
        }

        private void names_display_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(names_display.SelectedItem != null)
            {
                addUserInfo(names_display.SelectedItem.ToString());
            }
        }
    }
}
