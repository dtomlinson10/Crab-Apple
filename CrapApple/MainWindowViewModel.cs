//file for keeping UI logic organised, also lets us bind commands etc to data (buttons, etc).

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace CrapApple
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        //collection that provides notifications when items are added, removed, or changed. Used because provides methods for binding UI such as Add, Remove or Clear
        //look into MVVM architecture, just separating UI and logic using data binding & notifications
        public ObservableCollection<User> PersonList { get; set; }
        public ObservableCollection<Chore> ChoreList { get; set; }
        public ObservableCollection<Chore> CommonChores { get; set; }
        public ObservableCollection<Chore> WeeklyChoresList { get; set; }
        public AssignmentSystem AssignmentSystem { get; set; }
        public ObservableCollection<Admin> AdminList { get; set; }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin)); //notifies UI when IsAdmin property changes
            }
        }


        private RegularUser _selectedUser;
        public RegularUser SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
            }
        }

        private string _customChoreName;
        public string CustomChoreName
        {
            get { return _customChoreName; }
            set
            {
                _customChoreName = value;
                OnPropertyChanged(nameof(CustomChoreName));
            }
        }
        private string _newCommonChoreName;
        public string NewCommonChoreName
        {
            get { return _newCommonChoreName; }
            set
            {
                _newCommonChoreName = value;
                OnPropertyChanged(nameof(NewCommonChoreName));
            }
        }

        //ICommand is an interface that defines a way to invoke a method, easy way to use command enabling, disabling, passing etc
        public ICommand AddCheckedToWeeklyChoresCommand { get; private set; }
        public ICommand AddCustomChoreCommand { get; private set; }
        public ICommand ClearWeeklyChoresCommand { get; private set; }
        public ICommand AddCommonChoreCommand { get; private set; }
        public ICommand DeleteCheckedCommonChoresCommand { get; private set; }

        //propertyChanged is an event that is called when a property value changes
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()

        {
            PersonList = new ObservableCollection<User>();
            ChoreList = new ObservableCollection<Chore>();
            CommonChores = new ObservableCollection<Chore>();
            WeeklyChoresList = new ObservableCollection<Chore>();
            AssignmentSystem = new AssignmentSystem("AS01", ChoreList, PersonList);




            LoadCommonChores();

            //relayCommand is a class that implements the ICommand interface
            AddCheckedToWeeklyChoresCommand = new RelayCommand(AddCheckedToWeeklyChores);
            AddCustomChoreCommand = new RelayCommand(AddCustomChore, CanAddCustomChore);
            ClearWeeklyChoresCommand = new RelayCommand(_ => ClearWeeklyChores()); // Fixed to remove the parameter
            //ClearWeeklyChoresCommand = new RelayCommand()
            AddCommonChoreCommand = new RelayCommand(AddCommonChore, CanAddCommonChore);
            DeleteCheckedCommonChoresCommand = new RelayCommand(DeleteCheckedCommonChores, CanDeleteCheckedCommonChores);
        }



        public void GenerateChores(int choresToGenerate, DBConnection conn)
        {
            for (int i = 0; i < choresToGenerate; i++)
            {
                ChoreGenerationScript choreGenerationScript = new ChoreGenerationScript();
                Chore chore = choreGenerationScript.GenerateChore(i);

                ChoreList.Add(chore);
                conn.AddChore(chore);
            }
        }



        private bool CanAddCommonChore(object parameter)
        {
            //checks if the user is an admin and the NewCommonChoreName is not empty
            return IsAdmin && !string.IsNullOrWhiteSpace(NewCommonChoreName);
        }


        private void AddCommonChore(object parameter)
        {
            var dialog = new CustomChoreDialog(); //opens a custom dialog
            if (dialog.ShowDialog() == true) //if the user clicks OK on the dialog
            {
                var estimatedTime = dialog.EstimatedTime;
                if (estimatedTime >= 0)
                {
                    //creates a new Chore object and adds it to the CommonChores
                    CommonChores.Add(new Chore("", NewCommonChoreName, "", estimatedTime, null, DateOnly.FromDateTime(DateTime.Now), false, false));
                    NewCommonChoreName = string.Empty;
                }
                else
                {
                    MessageBox.Show("Please enter a valid estimated time.");
                }
            }
        }

        private bool CanDeleteCheckedCommonChores(object parameter)
        {
            //checks if the user is an admin and if any common chores are ticked
            return IsAdmin && CommonChores.Any(c => c.IsChecked);
        }

        private void DeleteCheckedCommonChores(object parameter)
        {
            var checkedChores = CommonChores.Where(c => c.IsChecked).ToList();
            foreach (var chore in checkedChores)
            {
                CommonChores.Remove(chore);
            }
        }

        private void LoadCommonChores()
        //default chores
        {
            CommonChores.Add(new Chore("", "Vacuum the carpets", "", 30, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            CommonChores.Add(new Chore("", "Clean the bathroom", "", 45, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            CommonChores.Add(new Chore("", "Do the dishes", "", 20, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            CommonChores.Add(new Chore("", "Laundry", "", 60, null, DateOnly.FromDateTime(DateTime.Now), false, false));
        }

        private void AddCheckedToWeeklyChores(object parameter)
        {
            var checkedChores = CommonChores.Where(c => c.IsChecked).ToList();
            int count = 0;

            foreach (var chore in checkedChores)
            {
                if (!WeeklyChoresList.Any(c => c.Name == chore.Name))
                {
                    //adds to weekly chore if not already there
                    WeeklyChoresList.Add(new Chore("", chore.Name, "", chore.EstimatedTime, null, DateOnly.FromDateTime(DateTime.Now), false, false));
                    count++;
                }
                chore.IsChecked = false;
            }

            if (count > 0)
            {
                MessageBox.Show($"{count} chore(s) added to the weekly chores list.");
            }
            else
            {
                MessageBox.Show("The checked chore(s) are already in the weekly chores list.");
            }
        }




        private bool CanAddCustomChore(object parameter)
        {
            //checks if not empty
            return IsAdmin && !string.IsNullOrWhiteSpace(CustomChoreName);
        }

        public void AddCustomChore(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(CustomChoreName))
            {
                var dialog = new CustomChoreDialog();
                if (dialog.ShowDialog() == true)
                {
                    var estimatedTime = dialog.EstimatedTime;
                    if (estimatedTime >= 0)
                    {
                        WeeklyChoresList.Add(new Chore("", CustomChoreName, "", estimatedTime, null, DateOnly.FromDateTime(DateTime.Now), false, false) { Name = CustomChoreName });
                        MessageBox.Show($"Custom chore '{CustomChoreName}' added successfully!");
                        CustomChoreName = string.Empty;
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid estimated time.");
                    }
                }
            }
            else
            {

                MessageBox.Show("Please enter a chore name.");
            }
        }



        public void ClearWeeklyChores()
        {
            WeeklyChoresList.Clear();
            MessageBox.Show("Weekly chores cleared successfully!");
        }

        /// <summary>
        /// method updated 
        /// </summary>
        public void SaveWeeklyChores()
        {
            DBConnection db = new DBConnection();
            db.Connect("Database/crabapple.db");

            foreach (var chore in WeeklyChoresList)
            {
                db.AddWeeklyChore(chore);
            }

            db.Disconnect();

            // Distribute weekly chores among users' optional chores
            DistributeWeeklyChores(WeeklyChoresList.ToList(), PersonList.ToList());

            // Clear the weekly chores list after saving
            WeeklyChoresList.Clear();

            // Refresh the UI or perform any necessary logic
            OnPropertyChanged(nameof(WeeklyChoresList));
        }

        public void DistributeWeeklyChores(List<Chore> weeklyChores, List<User> users)
        {
            var random = new Random();
            var shuffledChores = weeklyChores.OrderBy(x => random.Next()).ToList();

            int userIndex = 0;
            foreach (var chore in shuffledChores)
            {
                if (userIndex >= users.Count)
                {
                    userIndex = 0;
                }

                users[userIndex].OptionalChores.Add(chore);
                userIndex++;
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            //event for when property changes
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}