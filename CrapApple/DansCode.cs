using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private bool loggedIn = true;
        private List<people> person = new List<people>();
        private List<people> userInfo = new List<people>();
        private List<string> box = new List<string>();
        private List<int> choresCompleted = new List<int>();
        public MainWindow()
        {
            InitializeComponent();
            refreshtable();
            refreshUserInfo();
            addInfoToBoxes();

            // add the amount of chores completed to the list for each person
            foreach (var i in person)
            {
                // choresCompleted.Add(person.chores_completed);
            }

            //check if the user is logged in
            if (loggedIn)
            {
                // show the leaderboard and headings if the user is logged in
                ShowAdminFunctionality(this);
            }
            else
            {
                // hide the leaderboard and headings if the user is not logged in
                HideAdminFunctionality(this);
            }
        }

        // function to hide some of the layout and show an error message
        private void HideAdminFunctionality(MainWindow mainWindow)
        {
            // setting the property visibility to hidden or visible depending on the required display
            leaderboard_display.Visibility = Visibility.Hidden;
            leaderboard_heading.Visibility = Visibility.Hidden;
            streak_counter_heading.Visibility = Visibility.Hidden;
            notloggedInError.Visibility = Visibility.Visible;
            notSelectedError.Visibility = Visibility.Hidden;
            loggedIn = false;
        }

        // algorithm to sort the list of people according to the amount of chores they have completed
        public int[] SortArray(int[] array, int left, int right)
        {
            if (left < right)
            {
                int middle = left + (right - left) / 2;
                SortArray(array, left, middle);
                SortArray(array, middle + 1, right);
                MergeArray(array, left, middle, right);
            }
            return array;
        }
        // second function to merge the list of people together so the leaderboard can be displayed 
        // according to completed chores
        public void MergeArray(int[] array, int left, int middle, int right)
        {
            var leftArrayLength = middle - left + 1;
            var rightArrayLength = right - middle;
            var leftTempArray = new int[leftArrayLength];
            var rightTempArray = new int[rightArrayLength];
            int i, j;

            for (i = 0; i < leftArrayLength; ++i)
                leftTempArray[i] = array[left + i];
            for (j = 0; j < rightArrayLength; ++j)
                rightTempArray[j] = array[middle + 1 + j];

            i = 0;
            j = 0;
            int k = left;

            while (i < leftArrayLength && j < rightArrayLength)
            {
                if (leftTempArray[i] <= rightTempArray[j])
                {
                    array[k++] = leftTempArray[i++];
                }
                else
                {
                    array[k++] = rightTempArray[j++];
                }
            }

            while (i < leftArrayLength)
            {
                array[k++] = leftTempArray[i++];
            }

            while (j < rightArrayLength)
            {
                array[k++] = rightTempArray[j++];
            }
        }

        // function to show layout and remove the error message
        private void ShowAdminFunctionality(MainWindow mainwindow)
        {
            // setting the property visibility to hidden or visible depending on the required display
            leaderboard_display.Visibility = Visibility.Visible;
            leaderboard_heading.Visibility = Visibility.Visible;
            streak_counter_heading.Visibility = Visibility.Visible;
            notloggedInError.Visibility = Visibility.Hidden;
            loggedIn = true;
        }

        // function to add info to all the boxes about the user that has been selected
        private void addInfoToBoxes()
        {

            // clearing the list box before all the names of the textboxes
            // can be added
            box.Clear();

            // adding all the user info boxes to a list of strings called box
            box.Add("firstname_display");
            box.Add("lastname_display");
            box.Add("idDisplay");
            box.Add("choresCompleted_display");
            box.Add("choresassigned_display");
            box.Add("email_display");
            box.Add("chorestotal_display");

            // for loop to add the info of the selected person
            // to the user info boxes
            for (int i = 0; i < box.Count; i++)
            {
                //  box[i].Text = 
            }
        }

        // function to refresh the table for the leaderboard so it displays a list of the people in the database
        private void refreshtable()
        {
            person.Clear();
            // adding a new person to the list person 
            person.Add(new people()
            {
                // assigning corresponding details with fields
                // set in the class people
                firstname = "daniel",
                lastname = "tomlinson",
                chores_completed = 10,
                id = 1,
                totalChore = 12,
                chores_assigned = 15,
                email = "dtomlinson10@outlook.com"
            });

            // adding a second person to the list person
            person.Add(new people()
            {
                // assigning corresponding details with the fields 
                // set in the people class
                firstname = "john",
                lastname = "felix",
                chores_completed = 16,
                id = 2,
                totalChore = 18,
                chores_assigned = 19,
                email = "johnfelix10@outlook.com"
            });

            // displaying the firstname of the people in the list 
            // in the combo box for selection
            this.names_display.ItemsSource = person;
            this.names_display.DisplayMemberPath = "firstname";

            // displaying all the people in the leaderboard 
            // with all the deatils from the database
            this.leaderboard_display.ItemsSource = person;
        }

        // function to refresh the user ifo details
        public void refreshUserInfo()
        {
            // adding the selected user's details to userInfo list 
            // so the textboxes can update and show the correct display
            userInfo.Add(new people()
            {
                // assigning the corresponding details with the fields
                // set within the class people 
                firstname = "Daniel",
                lastname = "Tomlinson",
                chores_completed = 13,
                id = 2,
                totalChore = 17,
                chores_assigned = 20,
                email = "danieltomlinson10@outlook.com"
            });

            // getting the details from the userInfo list to assign 
            // variables for refernce when setting the text property of the textboxes.
            //  Name = userInfo.firstname;

            // assigning the variables to the text property of each textbox in user info
            //this.firstname_display.Text = Name;




        }

        //total points of chores textbox
        private void chores_total_change(object sender, TextChangedEventArgs e)
        {
            //change on button press of change user

        }

        private void names_display_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //string returned_value = DbConnection y = new DbConnection("");
            object v = names_display.SelectedValue;
            Console.WriteLine(v);
            if (v != null || v as string != "")
            {
                foreach (var i in person)
                {
                    // loop through the person list and add the info of the selected person 
                    // to each textbox

                }
                // show the layout with the laeaderboard and headings and hide the error messages
                notSelectedError.Visibility = Visibility.Hidden;
                leaderboard_display.Visibility = Visibility.Visible;
                leaderboard_heading.Visibility = Visibility.Visible;
                streak_counter_heading.Visibility = Visibility.Visible;
                notloggedInError.Visibility = Visibility.Visible;

            }
            else
            {
                // check if the user is logged in
                if (loggedIn == false)
                {
                    // show the not logged in error and hide everything else
                    // if the user is not logged in
                    notSelectedError.Visibility = Visibility.Hidden;
                }
                else
                {
                    //  show the leaderboard and headings and hide the error message 
                    // if the user is loggged in
                    notSelectedError.Visibility = Visibility.Visible;
                    leaderboard_display.Visibility = Visibility.Hidden;
                    leaderboard_heading.Visibility = Visibility.Hidden;
                    streak_counter_heading.Visibility = Visibility.Hidden;
                    notloggedInError.Visibility = Visibility.Hidden;
                }

            }
        }
    }
}
