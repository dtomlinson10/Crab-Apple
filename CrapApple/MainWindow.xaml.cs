//V1

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ScottPlot;
using ScottPlot.WPF;

namespace CrapApple
{
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private List<String> rewardsList { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            //create an instance of the view model, set data context
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            DBConnection dBConnection = new();
            // dBConnection.GetUsers();

            InitializeData(dBConnection);
            SetInitialVisibility();
            addRewardsDisplay();
            addGraph();
            ShowAdminFunctionality();
        }

        private void InitializeData(DBConnection conn)
        {
            _viewModel.GenerateChores(5, conn);
            //clear the PersonList in the view model and add people
            _viewModel.PersonList.Clear();
            _viewModel.PersonList = conn.GetUsers();
            foreach (User user in _viewModel.PersonList)
            {
                conn.AddUser(user);
            }

            //generates sample chores and populate the data grids
            GenerateDataGrids(conn);
        }

        private void SetInitialVisibility()
        {
            //set the IsAdmin property in the view model to false and hide admin mode
            _viewModel.IsAdmin = false;
            HideAdminFunctionality();
        }

        private void GenerateDataGrids(DBConnection db)
        {
            //set the data sources for various data grids and combo boxes
            usersDataGrid.ItemsSource = db.GetUsers();
            choresDataGrid.ItemsSource = _viewModel.ChoreList;

            selectUserCB.DisplayMemberPath = "FullName";
            selectUserCB.ItemsSource = _viewModel.PersonList;
            selectChoreCB.DisplayMemberPath = "Name";
            selectChoreCB.ItemsSource = _viewModel.ChoreList;

            names_display.ItemsSource = _viewModel.PersonList;
            names_display.DisplayMemberPath = "Forename";
            leaderboard_display.ItemsSource = _viewModel.PersonList;
        }

        private void HideAdminFunctionality()
        {
            //hide or disable UI elements related to admin
            autoAssign.Visibility = Visibility.Collapsed;
            ManualAssign.Visibility = Visibility.Collapsed;
            assignChoresLoggedInError.Visibility = Visibility.Visible;

            Leaderboard_Layout_Grid.Visibility = Visibility.Collapsed;
            names_display.Visibility = Visibility.Collapsed;
            UserInfoGrid.Visibility = Visibility.Collapsed;
            LogInError.Visibility = Visibility.Visible;
            rewards_display_grid.Visibility = Visibility.Collapsed;

            statLogInError.Visibility = Visibility.Visible;

            AssignChores.Visibility = Visibility.Collapsed;
            Motivation.Visibility = Visibility.Collapsed;
            Statistics.Visibility = Visibility.Collapsed;

            ChoreManagement.Visibility = Visibility.Visible;
            ChoreManagement.IsEnabled = false;
            CommonChoresTabItem.Visibility = Visibility.Collapsed;
            CustomChoresTabItem.Visibility = Visibility.Collapsed;
            WeeklyChoresTabItem.Visibility = Visibility.Visible;
            weeklyChoresDataGrid.IsReadOnly = true;

            saveButton.Visibility = Visibility.Collapsed;
            clearButton.Visibility = Visibility.Collapsed;
        }

        private void ShowAdminFunctionality()
        {
            //shows UI elements in admin
            autoAssign.Visibility = Visibility.Visible;
            ManualAssign.Visibility = Visibility.Visible;
            assignChoresLoggedInError.Visibility = Visibility.Collapsed;

            Leaderboard_Layout_Grid.Visibility = Visibility.Visible;
            names_display.Visibility = Visibility.Visible;
            UserInfoGrid.Visibility = Visibility.Visible;
            LogInError.Visibility = Visibility.Collapsed;
            rewards_display_grid.Visibility = Visibility.Visible;

            statLogInError.Visibility = Visibility.Collapsed;

            AssignChores.Visibility = Visibility.Visible;
            Motivation.Visibility = Visibility.Visible;
            Statistics.Visibility = Visibility.Visible;

            ChoreManagement.Visibility = Visibility.Visible;
            ChoreManagement.IsEnabled = true;
            CommonChoresTabItem.Visibility = Visibility.Visible;
            CustomChoresTabItem.Visibility = Visibility.Visible;
            WeeklyChoresTabItem.Visibility = Visibility.Visible;
            weeklyChoresDataGrid.IsReadOnly = false;

            saveButton.Visibility = Visibility.Visible;
            clearButton.Visibility = Visibility.Visible;
        }

        private void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //get the email and password from the login controls
            string email = loginEmail.Text;
            string password = loginPassword.Password;

            User user = _viewModel.PersonList.FirstOrDefault(u => u.Email == email);
            if (user != null)
            {
                if (user.Password == password)
                {
                    _viewModel.IsAdmin = true;
                    ShowAdminFunctionality();
                }
                else
                {
                    MessageBox.Show("Incorrect password. Please try again.");
                    HideAdminFunctionality();
                }
            }
            else
            {
                MessageBox.Show("User with this email does not exist.");
                HideAdminFunctionality();
            }
        }

        // Assign Chores Tab
        private void AutoAssignChores_Click(object sender, RoutedEventArgs e)
        {
            List<Chore> selectedChores = new List<Chore>();
            foreach (var item in choresDataGrid.SelectedItems)
            {
                selectedChores.Add((Chore)item);
            }

            List<User> selectedUsers = new List<User>();
            foreach (var item in usersDataGrid.SelectedItems)
            {
                selectedUsers.Add((User)item);
            }

            _viewModel.AssignmentSystem.autoAssignChores(selectedChores, selectedUsers);
        }

        private void assignButton_Click(object sender, RoutedEventArgs e)
        {
            User selectedUser = (User)selectUserCB.SelectedItem;
            Chore selectedChore = (Chore)selectChoreCB.SelectedItem;

            selectedUser.AssignedChores.Add(selectedChore);
            Debug.WriteLine("Assigned " + selectUserCB.Text + " Chore: " + selectChoreCB.Text);

            DBConnection connection = new DBConnection();
            connection.RunSQL("UPDATE Users SET AssignedChores = " + selectedChore.ID + " WHERE ID = " + selectedUser.Id);
        }

        // Motivation Tab 
        private void names_display_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            //when a user is selected in the names_display list
            if (names_display.SelectedItem != null)
            {
                //get the selected user from the list
                if (names_display.SelectedItem is Admin)
                {
                    Admin selectedUser = (Admin)names_display.SelectedItem;
                    UpdateUserInfo(selectedUser);

                }
                else
                {
                    RegularUser selectedUser = (RegularUser)names_display.SelectedItem;
                    UpdateUserInfo(selectedUser);
                }
                //update the user information displays

            }
        }

        private void UpdateUserInfo(User user)
        {
            //update the user info displays based on user selected
            if (user != null)
            {
                firstname_display.Text = user.Forename;
                lastname_display.Text = user.Surname;
                idDisplay.Text = user.Id;
                email_display.Text = user.Email;
                choresassigned_display.Text = user.AssignedChores.Count.ToString();
                chorestotal_display.Text = user.TotalChores.ToString();
                choresCompleted_display.Text = user.CompletedChores.Count.ToString();
            }
            else
            {
                //clear the user info displays if no user is selected
                firstname_display.Text = string.Empty;
                lastname_display.Text = string.Empty;
                idDisplay.Text = string.Empty;
                email_display.Text = string.Empty;
                choresassigned_display.Text = string.Empty;
                chorestotal_display.Text = string.Empty;
                choresCompleted_display.Text = string.Empty;
            }
        }

        private void addRewardsDisplay()
        {
            rewardsList = new List<String>();
            rewardsList.Add("1 chore off!");
            rewardsList.Add("10 points off voucher!");
            rewardsList.Add("free pizza!");
            rewardsList.Add("30 points off voucher!");
            rewardsList.Add("2 small chores off!");
            Rewards_display.ItemsSource = rewardsList;
        }

        private void Collect_Rewards_Button_Click(object sender, RoutedEventArgs e)
        {
            //add logic
        }

        // Statistics Tab 
        private void addGraph()
        {
            // TODO: Generate from database 
            double[] datax = { 1, 2, 3, 4, 5 };
            double[] datay = { 5, 20, 15, 20, 25 };
            this.WpfPlot1.Plot.Add.Scatter(datax, datay);
            this.WpfPlot2.Plot.Add.Bars(datax);
            this.WpfPlot3.Plot.Add.Pie(datay);
            this.WpfPlot1.Refresh();
        }

        // Chore management Tab
        private void WeeklyChoresDataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //validate user input in the WeeklyChoresDataGrid based on estimated time
            e.Handled = !InputValidationUtil.ValidateEstimatedTime(e.Text, out _);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SaveWeeklyChores();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearWeeklyChores();
        }

    }
}