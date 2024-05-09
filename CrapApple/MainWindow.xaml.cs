using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CrapApple
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<RegularUser> personList { get; set; }
        private List<Chore> choreList { get; set; }
        DateOnly todays_date { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            
            RefreshTable();
            addUserInfo("daniel");
            choreList = new List<Chore>();
            personList = new List<RegularUser>();
            todays_date = new DateOnly();

            personList.Clear();
            // adding new people to the personList 
            personList.Add(new RegularUser("001", "Daniel", "Tomlinson", "dtomlinson10@outlook.com", "password"));
            personList.Add(new RegularUser("002", "Harvey", "Walker", "harveywalker500@gmail.com", "password2"));

            Admin administrator = new Admin("101", "Harvey", "Walker", "harveywalker500@gmail.com", "password");
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

            // Create an instance of the view model
            var viewModel = new MainWindowViewModel();

            // Set the CurrentUser property based on the logged-in user
            // Replace the following line with your logic to retrieve the logged-in user
            viewModel.CurrentUser = new RegularUser("001", "John", "Doe", "john.doe@example.com", "password");

            // Set the data context to the view model instance
            DataContext = viewModel;

            // Subscribe to the PreviewTextInput event of the weeklyChoresDataGrid
            weeklyChoresDataGrid.PreviewTextInput += WeeklyChoresDataGrid_PreviewTextInput;
        }

        private void generateDataGrids()
        {
            usersDataGrid.ItemsSource = personList;
            choresDataGrid.ItemsSource = choreList;

            selectUserCB.DisplayMemberPath = "forename " + "surname"; ;
            selectUserCB.ItemsSource = personList;
            selectChoreCB.DisplayMemberPath = "name";
            selectChoreCB.ItemsSource = personList;
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
            for (int i = 0; i < choresToGenerate; i++)
            {
                ChoreGenerationScript choreGenerationScript = new ChoreGenerationScript();
                Chore chore = choreGenerationScript.GenerateChore();
                choreList.Add(chore);
            }
        }

        //login retrieval 
        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            string email = loginEmail.Text;
            string password = loginPassword.Text;

            if (email != "Email" && password != "Password")
            {
                if (email == "daniel" && password == "password")
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
        DateOnly date_completed = new DateOnly();
        private int points = 0;
        private User daniel { get; set; }

        //function to add details to user info boxes
        private void addUserInfo(string user)
        {
            if (user == null)
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

        //function to load rewards grid info into the motivation tab on project load 
        // and when someone changes the person they want to view
        private void RewardsInfo_display()
        {
            choreList.Add(new Chore("001", "Wash the Dishes", "Wash the Dishes", 1.5, daniel, date_completed, false, false));
            this.Rewards_display.ItemsSource = choreList;

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
            if (names_display.SelectedItem != null)
            {
                addUserInfo(names_display.SelectedItem.ToString());
            }
        }

        private void Collect_Rewards_Button_Click(object sender, RoutedEventArgs e)
        {
            if (choreList.Count == 0)
            {
                points += 0;
            }
            // else if()
            // {

            // }
        }

        //matts code
        // Event handler for the PreviewTextInput event of the weeklyChoresDataGrid
        private void WeeklyChoresDataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Validate the input text using the ValidateEstimatedTime method from InputValidationUtil
            // If input is not valid, set e.Handled to true to prevent the input from being processed
            e.Handled = !InputValidationUtil.ValidateEstimatedTime(e.Text, out _);
        }
    }
}
