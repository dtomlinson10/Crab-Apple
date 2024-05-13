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

            // get database connection
            DBConnection dBConnection = new();

            // set up intial layout for each tab
            InitializeData(dBConnection, false);
            SetInitialVisibility();
            addRewardsDisplay();
            addGraph(dBConnection);
            ShowAdminFunctionality();

            _choreCompletionViewModel = new CrapApple.ChoreCompletionViewModel(_viewModel.PersonList);
            UserComboBox.DataContext = _choreCompletionViewModel;

            InitializeChoreCompletionView();
        }

        // get data from database and populate data grids
        private void InitializeData(DBConnection conn, bool choresNeeded)
        {
            // generate chores in database if there isnt any
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

            // check if details match user/admin in database 
            // and show functionality accordingly
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

        /// <summary>
        /// add selected chore(s) to the selected user automatically
        /// </summary>
        
        private void AutoAssignChores_Click(object sender, RoutedEventArgs e)
        {
            // add selected chores to a list
            List<Chore> selectedChores = new List<Chore>();
            foreach (var item in choresDataGrid.SelectedItems)
            {
                selectedChores.Add((Chore)item);
            }

            // add selected user to a list
            List<User> selectedUsers = new List<User>();
            foreach (var item in usersDataGrid.SelectedItems)
            {
                selectedUsers.Add((User)item);
            }

            // assign chore(s) to selected user
            _viewModel.AssignmentSystem.autoAssignChores(selectedChores, selectedUsers);
        }

        /// <summary>
        /// Manually assign selected chores to the selected user 
        /// </summary>
 
        private void assignButton_Click(object sender, RoutedEventArgs e)
        {
            // get selected user and chore(s)
            User selectedUser = (User)selectUserCB.SelectedItem;
            Chore selectedChore = (Chore)selectChoreCB.SelectedItem;

            selectedUser.AssignedChores.Add(selectedChore);
            Debug.WriteLine("Assigned " + selectUserCB.Text + " Chore: " + selectChoreCB.Text);

            // query database to add selected chore to user
            DBConnection connection = new DBConnection();
            connection.RunSQL("UPDATE Users SET AssignedChores = " + selectedChore.ID + " WHERE ID = " + selectedUser.Id);
        }

        // Motivation Tab 

        /// <summary>
        /// call function to display the selected user info
        /// </summary>
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

        /// <summary>
        /// update user info boxes when a user is selected 
        /// </summary>
        private void UpdateUserInfo(User user)
        {
            //update the user info displays based on user selected
            if (user != null)
            {
                // display info if there is a user selected
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

        /// <summary>
        /// create a list of rewards for display in motivation tab
        /// </summary>
        private void addRewardsDisplay()
        {
            rewardsList = new List<String>();
            rewardsList.Add("1 chore off!");
            rewardsList.Add("10 points off voucher!");
            rewardsList.Add("free pizza!");
            rewardsList.Add("30 points off voucher!");
            rewardsList.Add("2 small chores off!");

            // display the rewards in the rewards box
            Rewards_display.ItemsSource = rewardsList;
        }

        /// <summary>
        /// collect rewards button functionality to collect available points if the user has completed some chores
        /// </summary>
        private void Collect_Rewards_Button_Click(object sender, RoutedEventArgs e)
        {
            // get selected user and check if they have completed the chore 
            // if so add points to the counter and display in the textbox
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

        /// <summary>
        /// adding graphs to statistics tab
        /// query database for data representation
        /// style graphs
        /// </summary>
        private void addGraph(DBConnection conn)
        {
            //addSampleData(); used for when testing graph display
            //bar chart for chore weight
            double[] values_choreWeight = { };
            List<String> names_ChoreWeight = new List<String>();
            Tick[] ticks1 = new Tick[_viewModel.ChoreList.Count()];

            for (int i = 0; i < _viewModel.ChoreList.Count; i++)
            {
                Array.Resize(ref values_choreWeight, values_choreWeight.Length + 1);
                Chore chores = _viewModel.ChoreList[i];
                values_choreWeight[i] = i + 1; // user.CompletedChores.Count;
                names_ChoreWeight.Add(chores.Name);
            }
            Bar_choreWeight_Graph.Plot.Add.Bars(values_choreWeight);

            // Add names of chores as X-axis ticks
            var tickPositions1 = new double[_viewModel.ChoreList.Count];
            for (int i = 0; i < _viewModel.ChoreList.Count; i++)
            {
                tickPositions1[i] = i;
            }
            var tickGenerator1 = new ScottPlot.TickGenerators.NumericManual(tickPositions1, names_ChoreWeight.ToArray());
            Bar_choreWeight_Graph.Plot.Axes.Bottom.TickGenerator = tickGenerator1;

            // style the bar chart for the chore weights
            this.Bar_choreWeight_Graph.Plot.XLabel("Chores");
            this.Bar_choreWeight_Graph.Plot.YLabel("Chore weight");
            this.Bar_choreWeight_Graph.Plot.Title("Chore Weighting");
            this.Bar_choreWeight_Graph.Plot.Grid.MajorLineColor = Color.FromHex("#0e3d54");
            this.Bar_choreWeight_Graph.Plot.FigureBackground.Color = Color.FromHex("#07263b");
            this.Bar_choreWeight_Graph.Plot.DataBackground.Color = Color.FromHex("#0b3049");
            this.Bar_choreWeight_Graph.Plot.Axes.Color(Color.FromHex("#a0acb5"));


            //bar chart for amount of chores completed per person
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
            Bar_Chart.Plot.Add.Bars(values);

            // Add forenames as X-axis ticks
            var tickPositions = new double[_viewModel.PersonList.Count];
            for (int i = 0; i < _viewModel.PersonList.Count; i++)
            {
                tickPositions[i] = i;
            }
            var tickGenerator = new ScottPlot.TickGenerators.NumericManual(tickPositions, names.ToArray());
            Bar_Chart.Plot.Axes.Bottom.TickGenerator = tickGenerator;

            // add styling for bar chart for chores completed per person
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
            // create graph and style the pie chart
            this.Pie_Chart.Plot.Add.Pie(dataxPie);
            this.Pie_Chart.Plot.Title("Percentage of chores completed");
            this.Pie_Chart.Plot.Grid.MajorLineColor = Color.FromHex("#0e3d54");
            this.Pie_Chart.Plot.FigureBackground.Color = Color.FromHex("#07263b");
            this.Pie_Chart.Plot.DataBackground.Color = Color.FromHex("#0b3049");
            this.Pie_Chart.Plot.Axes.Color(Color.FromHex("#a0acb5"));

            // refresh all graphs so they display correctly
            this.Bar_choreWeight_Graph.Refresh();
            this.Bar_Chart.Refresh();
            this.Pie_Chart.Refresh();
        }

        /// <summary>
        /// show tutorial window if the tutorial button is clicked on stats page
        /// </summary>
        private void Tutorial_click(object sender, RoutedEventArgs e)
        {
            Statistics_tutorial tutorial = new Statistics_tutorial();
            tutorial.Show();
            this.Hide();
        }

        // adding sample data -  used to test graphs
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
            // intialise all components on chore completion
            var choreCompletionView = new ChoreCompletionView();
            var choreCompletionViewModel = (ChoreCompletionViewModel)choreCompletionView.DataContext;
            _choreCompletionViewModel = new ChoreCompletionViewModel(_viewModel.PersonList);

            var choreCompletionGrid = (Grid)ChoreCompletion.Content;
            choreCompletionGrid.Children.Add(choreCompletionView);
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // save weekly chores when save button is clicked
            _viewModel.SaveWeeklyChores();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            // clear weekly chores on clear button click
            _viewModel.ClearWeeklyChores();
        }


    }
}