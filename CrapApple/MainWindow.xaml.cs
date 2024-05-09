using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScottPlot;

namespace CrapApple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<User> personList { get; set; }
        private List<Chore> choreList { get; set; }
        DateOnly todays_date { get; set; }
        public bool loggedIn { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            
            choreList = new List<Chore>();
            personList = new List<User>();
            todays_date = new DateOnly();

            // sample person list
            personList.Add(new RegularUser("001", "Daniel", "Tomlinson", "dtomlinson10@outlook.com", "password"));
            personList.Add(new RegularUser("002", "Harvey", "Walker", "harveywalker500@gmail.com", "password2"));
            personList.Add(new Admin("101", "John", "Smith", "harveywalker500@gmail.com", "password"));

            bool loggedIn = true;
            if (loggedIn)
            {
                ShowAdminFunctionality(this);
            }
            else
            {
                HideAdminFunctionality(this);
            }
            GenerateChores(5);
            generateDataGrids();

        }

        private void generateDataGrids()
        {
            // Assign Tab
            usersDataGrid.ItemsSource = personList;
            choresDataGrid.ItemsSource = choreList;

            selectUserCB.DisplayMemberPath = "forename";
            selectUserCB.ItemsSource = personList;
            selectChoreCB.DisplayMemberPath = "name";
            selectChoreCB.ItemsSource = choreList;

            // Motivation Tab
            names_display.ItemsSource = personList;
            names_display.DisplayMemberPath = "forename";
            leaderboard_display.ItemsSource = personList;
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
            rewards_display_grid.Visibility = Visibility.Hidden;

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
            rewards_display_grid.Visibility = Visibility.Visible;

            //setting the visibility of the stats tab
            statLogInError.Visibility = Visibility.Hidden;
        }

        private void AutoAssignChores(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException(Name + " is not implemented yet.");
        }

        private void GenerateChores(int choresToGenerate)
        {
            ChoreGenerationScript choreGenerationScript = new ChoreGenerationScript();
            for (int i = 0; i < choresToGenerate; i++)
            {
                String choreIDIterator = i.ToString();
                Chore chore = choreGenerationScript.GenerateChore(choreIDIterator, personList[1]);
                choreList.Add(chore);
            }
        }

        //login retrieval 
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            // TODO: Update code to link with database
            string email = loginEmail.Text;
            string password = loginPassword.Text;

            User user = personList.FirstOrDefault(u => u.email == email);
            if (user != null)
            {
                if (user.password == password)
                {
                    ShowAdminFunctionality(this);
                }
                else
                {
                    HideAdminFunctionality(this);
                    MessageBox.Show("Incorrect password. Please try again.");
                }
            }
            else
            {
                HideAdminFunctionality(this);
                MessageBox.Show("User with this email does not exist.");
            }
        }

        private void assignButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Assigned " + selectUserCB.Text + " Chore: " + selectChoreCB.Text);
        }
    }
}
