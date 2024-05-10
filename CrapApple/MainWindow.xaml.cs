using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ScottPlot;
using ScottPlot.AxisPanels;
using ScottPlot.WPF;

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
        private List<String> rewardsList { get; set; }
        public AssignmentSystem assignmentSystem { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            choreList = new List<Chore>();
            personList = new List<User>();
            todays_date = new DateOnly();
            rewardsList = new List<String>();
            assignmentSystem = new AssignmentSystem("AS01", choreList, personList);



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
            addGraph();
            addRewardsDisplay();
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

            assignmentSystem.autoAssignChores(selectedChores, selectedUsers);
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
        // motivation tab
        private void addRewardsDisplay()
        {
            rewardsList.Add("1 chore off!");
            rewardsList.Add("10 points off voucher!");
            rewardsList.Add("free pizza!");
            rewardsList.Add("30 points off voucher!");
            rewardsList.Add("2 small chores off!");
            Rewards_display.ItemsSource = rewardsList;
        }
        //stats tab
        private void addGraph()
        {
            double[] datax = { 1, 2, 3, 4, 5 };
            double[] datay = { 5, 20, 15, 20, 25 };
            this.WpfPlot1.Plot.Add.Scatter(datax, datay);
            this.WpfPlot2.Plot.Add.Bars(datax);
            this.WpfPlot3.Plot.Add.Pie(datay);
            this.WpfPlot1.Refresh();


        }
    }
}
