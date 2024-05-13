//V1

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Model;
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

        private ChoreCompletionViewModel _choreCompletionViewModel;

        public MainWindow()
        {
            InitializeComponent();
            //create an instance of the view model, set data context
            _viewModel = new MainWindowViewModel();
            DataContext = _viewModel;

            DBConnection dBConnection = new();
            // dBConnection.GetUsers();

            InitializeData(dBConnection, true);
            SetInitialVisibility();
            addRewardsDisplay();
            addGraph(dBConnection);
            ShowAdminFunctionality();

            _choreCompletionViewModel = new CrapApple.ChoreCompletionViewModel(_viewModel.PersonList);
            UserComboBox.DataContext = _choreCompletionViewModel;

            InitializeChoreCompletionView();
        }

        private void InitializeData(DBConnection conn, bool choresNeeded)
        {
            if (choresNeeded)
            {
                _viewModel.GenerateChores(5, conn);
            }
            _viewModel.PersonList.Clear();
            _viewModel.ChoreList.Clear();

            _viewModel.PersonList = conn.GetUsers();
            _viewModel.ChoreList = conn.GetChores(_viewModel.PersonList);

            GenerateDataGrids();
        }

        private void SetInitialVisibility()
        {
            //set the IsAdmin property in the view model to false and hide admin mode
            _viewModel.IsAdmin = false;
            HideAdminFunctionality();
        }

        private void GenerateDataGrids()
        {
            //set the data sources for various data grids and combo boxes
            usersDataGrid.ItemsSource = _viewModel.PersonList;
            choresDataGrid.ItemsSource = _viewModel.ChoreList;

            selectUserCB.DisplayMemberPath = "Forename";
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

            double points = 0;
            User selectedUser = _viewModel.SelectedUser;
            foreach (var i in _viewModel.ChoreList)
            {
                if (selectedUser != null)
                {
                    if (selectedUser == i.AssignedUser)
                    {
                        points += i.Weight / 2;
                    }
                }
            }
            points_display.Content = "Points Collected: " + points.ToString();
        }

        // Statistics Tab 
        private void addGraph(DBConnection conn)
        {
            //addSampleData();
            //scatter graph
            this.Scatter_Graph.Plot.XLabel("Chores");
            this.Scatter_Graph.Plot.YLabel("Chore weight");
            this.Scatter_Graph.Plot.Title("Chore Weight of Each Chore");
            double[] dataxScatter = { };
            double[] datayScatter = { };
            for (int i = 0; i < _viewModel.ChoreList.Count; i++)
            {
                Array.Resize(ref dataxScatter, dataxScatter.Length + 1);
                Array.Resize(ref datayScatter, datayScatter.Length + 1);
                double id = Convert.ToDouble(_viewModel.ChoreList[i].ID);
                dataxScatter[i] = id;
                datayScatter[i] = _viewModel.ChoreList[i].Weight;
            }
            var lines = this.Scatter_Graph.Plot.Add.Scatter(dataxScatter, datayScatter);
            lines.LegendText = "Chore Weight";
            this.Scatter_Graph.Plot.Grid.MajorLineColor = Color.FromHex("#0e3d54");
            this.Scatter_Graph.Plot.FigureBackground.Color = Color.FromHex("#07263b");
            this.Scatter_Graph.Plot.DataBackground.Color = Color.FromHex("#0b3049");
            this.Scatter_Graph.Plot.Axes.Color(Color.FromHex("#a0acb5"));


            //bar chart
            double[] values = { };
            List<String> names = new List<String>();
            Tick[] ticks = new Tick[_viewModel.PersonList.Count()];

            for (int i = 0; i < _viewModel.PersonList.Count; i++)
            {
                Array.Resize(ref values, values.Length + 1);
                User user = _viewModel.PersonList[i];
                values[i] = i + 1; // user.CompletedChores.Count;
                names.Add(user.Forename);
            }
            var barPlot = Bar_Chart.Plot.Add.Bars(values);

            // Add forenames as X-axis ticks
            var tickPositions = new double[_viewModel.PersonList.Count];
            for (int i = 0; i < _viewModel.PersonList.Count; i++)
            {
                tickPositions[i] = i;
            }

            var tickGenerator = new ScottPlot.TickGenerators.NumericManual(tickPositions, names.ToArray());
            Bar_Chart.Plot.Axes.Bottom.TickGenerator = tickGenerator;

            int barIndex = 0;
            foreach (var bar in barPlot.Bars)
            {
                bar.Label = names[barIndex];
                barIndex++;
            }
            this.Bar_Chart.Plot.XLabel("Users");
            this.Bar_Chart.Plot.YLabel("Chores Completed");
            this.Bar_Chart.Plot.Title("Chores Completed per Person");
            this.Bar_Chart.Plot.Grid.MajorLineColor = Color.FromHex("#0e3d54");
            this.Bar_Chart.Plot.FigureBackground.Color = Color.FromHex("#07263b");
            this.Bar_Chart.Plot.DataBackground.Color = Color.FromHex("#0b3049");
            this.Bar_Chart.Plot.Axes.Color(Color.FromHex("#a0acb5"));

            //pie chart
            double[] dataxPie = new double[0];
            foreach (User user in _viewModel.PersonList)
            {
                Array.Resize(ref dataxPie, dataxPie.Length + 1);
                dataxPie[dataxPie.Length - 1] = user.TotalChores - user.AssignedChores.Count;
            }
            this.Pie_Chart.Plot.Add.Pie(dataxPie);
            this.Pie_Chart.Plot.Title("Percentage of chores completed");
            this.Pie_Chart.Plot.Grid.MajorLineColor = Color.FromHex("#0e3d54");
            this.Pie_Chart.Plot.FigureBackground.Color = Color.FromHex("#07263b");
            this.Pie_Chart.Plot.DataBackground.Color = Color.FromHex("#0b3049");
            this.Pie_Chart.Plot.Axes.Color(Color.FromHex("#a0acb5"));

            this.Scatter_Graph.Refresh();
            this.Bar_Chart.Refresh();
            this.Pie_Chart.Refresh();

        }

        private void Tutorial_click(object sender, RoutedEventArgs e)
        {
            Statistics_tutorial tutorial = new Statistics_tutorial();
            tutorial.Show();
            this.Hide();
        }

        // adding sample data
        private void addSampleData()
        {
            _viewModel.PersonList.Clear();
            _viewModel.ChoreList.Clear();

            _viewModel.PersonList.Add(new Admin("1", "Alice", "Smith", "alice.smith@example.com", "password123", new List<Chore>(), new List<Chore>(), 3));
            _viewModel.PersonList.Add(new RegularUser("2", "Bob", "Johnson", "bob.johnson@example.com", "password456", new List<Chore>(), new List<Chore>(), 3));
            _viewModel.PersonList.Add(new RegularUser("3", "Eve", "Brown", "eve.brown@example.com", "password789", new List<Chore>(), new List<Chore>(), 3));
            _viewModel.ChoreList.Add(new Chore("1", "Wash Dishes", "Wash all the dishes in the sink.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("2", "Take out the trash", "Take the trash out to the curb for collection.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("3", "Vacuum the floor", "Vacuum all carpeted areas in the house.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("4", "Do the laundry", "Wash, dry, fold, and put away all the laundry.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("5", "Clean the bathroom", "Scrub the toilet, sink, shower, and mop the floor in the bathroom.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("6", "Water the plants", "Give water to all indoor and outdoor plants.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("7", "Mow the lawn", "Mow the grass in the front and back yards.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("8", "Weed the garden", "Pull out all weeds from the garden beds.", 0, null, new DateOnly(2024, 5, 12), true, false));
            _viewModel.ChoreList.Add(new Chore("9", "Dust furniture", "Dust all furniture surfaces in the living room and bedrooms.", 0, null, new DateOnly(2024, 5, 12), true, false));
        }

        // Chore management Tab
        private void WeeklyChoresDataGrid_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            //validate user input in the WeeklyChoresDataGrid based on estimated time
            e.Handled = !InputValidationUtil.ValidateEstimatedTime(e.Text, out _);
        }

        /// <summary>
        /// save saves to weeklyChores table then clears grid display, clear button clears grid display
        /// </summary>
        /// 
        private void InitializeChoreCompletionView()
        {
            var choreCompletionView = new ChoreCompletionView();
            var choreCompletionViewModel = (ChoreCompletionViewModel)choreCompletionView.DataContext;
            _choreCompletionViewModel = new ChoreCompletionViewModel(_viewModel.PersonList);

            var choreCompletionGrid = (Grid)ChoreCompletion.Content;
            choreCompletionGrid.Children.Add(choreCompletionView);
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