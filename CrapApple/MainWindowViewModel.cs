//handles logic, properties, and commands for managing the chores & UI.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;

namespace CrapApple
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        //add properties for the currently logged-in user and admin flag
        private User _currentUser;
        public User CurrentUser
        {
            get { return _currentUser; }
            set
            {
                _currentUser = value;
                OnPropertyChanged(nameof(CurrentUser));
            }
        }

        private bool _isAdmin;
        public bool IsAdmin
        {
            get { return _isAdmin; }
            set
            {
                _isAdmin = value;
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        // collection of common chores
        public ObservableCollection<Chore> CommonChores { get; set; }

        // collection of weekly chores
        private ObservableCollection<Chore> _weeklyChoresList;
        public ObservableCollection<Chore> WeeklyChoresList
        {
            get { return _weeklyChoresList; }
            set
            {
                _weeklyChoresList = value;
                OnPropertyChanged(nameof(WeeklyChoresList));
                SaveWeeklyChores();
            }
        }

        // name of custom chore
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

        // name of new common chore
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

        //properties for visibilites (whether the user can see the tabs or not. Regular user should only see weeklygrid, admin can see all.
        private Visibility _commonChoresTabVisibility = Visibility.Visible;
        public Visibility CommonChoresTabVisibility
        {
            get { return _commonChoresTabVisibility; }
            set
            {
                _commonChoresTabVisibility = value;
                OnPropertyChanged(nameof(CommonChoresTabVisibility));
            }
        }
        private Visibility _customChoresTabVisibility = Visibility.Visible;
        public Visibility CustomChoresTabVisibility
        {
            get { return _customChoresTabVisibility; }
            set
            {
                _customChoresTabVisibility = value;
                OnPropertyChanged(nameof(CustomChoresTabVisibility));
            }
        }
        private Visibility _weeklyChoresTabVisibility = Visibility.Visible;
        public Visibility WeeklyChoresTabVisibility
        {
            get { return _weeklyChoresTabVisibility; }
            set
            {
                _weeklyChoresTabVisibility = value;
                OnPropertyChanged(nameof(WeeklyChoresTabVisibility));
            }
        }
        //end of visibility properties

        // commands for buttons
        public ICommand AddCheckedToWeeklyChoresCommand { get; private set; }
        public ICommand AddCustomChoreCommand { get; private set; }
        public ICommand ClearWeeklyChoresCommand { get; private set; }
        public ICommand AddCommonChoreCommand { get; private set; }
        public ICommand DeleteCheckedCommonChoresCommand { get; private set; }

        // event for notifying property changes
        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindowViewModel()
        {
            // init collections and load data
            CommonChores = new ObservableCollection<Chore>();
            _weeklyChoresList = new ObservableCollection<Chore>();
            LoadCommonChores();
            LoadWeeklyChores();

            // commands setup
            AddCheckedToWeeklyChoresCommand = new RelayCommand(AddCheckedToWeeklyChores);
            AddCustomChoreCommand = new RelayCommand(AddCustomChore, CanAddCustomChore);
            ClearWeeklyChoresCommand = new RelayCommand(ClearWeeklyChores);
            AddCommonChoreCommand = new RelayCommand(AddCommonChore, CanAddCommonChore);
            DeleteCheckedCommonChoresCommand = new RelayCommand(DeleteCheckedCommonChores, CanDeleteCheckedCommonChores);

            // Set the initial visibility of chore management controls based on the user role
            SetChoreManagementControlsVisibility();
        }

        // Method to set the visibility of chore management controls based on the user role
        private void SetChoreManagementControlsVisibility()
        {
            // Check if the CurrentUser is an instance of the Admin class
            bool isAdmin = CurrentUser is Admin;

            CommonChoresTabVisibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            CustomChoresTabVisibility = isAdmin ? Visibility.Visible : Visibility.Collapsed;
            WeeklyChoresTabVisibility = Visibility.Visible; // Weekly chores tab is always visible
        }

        // Update the CanExecute methods for the commands
        private bool CanAddCommonChore(object parameter)
        {
            return IsAdmin && !string.IsNullOrWhiteSpace(NewCommonChoreName);
        }

        private bool CanDeleteCheckedCommonChores(object parameter)
        {
            return IsAdmin && CommonChores.Any(c => c.IsChecked);
        }

        private bool CanAddCustomChore(object parameter)
        {
            return IsAdmin && !string.IsNullOrWhiteSpace(CustomChoreName);
        }

        // adds a new common chore
        private void AddCommonChore(object parameter)
        {
            CommonChores.Add(new Chore("", NewCommonChoreName, "", 0, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            NewCommonChoreName = string.Empty;
        }

        // deletes checked common chores
        private void DeleteCheckedCommonChores(object parameter)
        {
            var checkedChores = CommonChores.Where(c => c.IsChecked).ToList();
            foreach (var chore in checkedChores)
            {
                CommonChores.Remove(chore);
            }
        }

        // loads weekly chores from the database
        private void LoadWeeklyChores()
        {
            // TODO: Load weekly chores from the SQLite database
        }

        // saves weekly chores to the database
        private void SaveWeeklyChores()
        {
            // TODO: Save weekly chores to the SQLite database
            OnPropertyChanged(nameof(WeeklyChoresList));
        }

        // loads common chores
        private void LoadCommonChores()
        {
            CommonChores.Add(new Chore("", "Vacuum the carpets", "", 30, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            CommonChores.Add(new Chore("", "Clean the bathroom", "", 45, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            CommonChores.Add(new Chore("", "Do the dishes", "", 20, null, DateOnly.FromDateTime(DateTime.Now), false, false));
            CommonChores.Add(new Chore("", "Laundry", "", 60, null, DateOnly.FromDateTime(DateTime.Now), false, false));
        }

        // adds checked common chores to weekly chores
        private void AddCheckedToWeeklyChores(object parameter)
        {
            var checkedChores = CommonChores.Where(c => c.IsChecked).ToList();
            int count = 0;

            foreach (var chore in checkedChores)
            {
                if (!WeeklyChoresList.Any(c => c.Name == chore.Name))
                {
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

            SaveWeeklyChores();
        }

        // adds a custom chore to weekly chores
        private void AddCustomChore(object parameter)
        {
            if (!string.IsNullOrWhiteSpace(CustomChoreName))
            {
                var dialog = new CustomChoreDialog();
                if (dialog.ShowDialog() == true)
                {
                    var estimatedTime = dialog.EstimatedTime;
                    if (estimatedTime >= 0)
                    {
                        WeeklyChoresList.Add(new Chore("", CustomChoreName, "", estimatedTime, null, DateOnly.FromDateTime(DateTime.Now), false, false));
                        CustomChoreName = string.Empty;
                        MessageBox.Show($"Custom chore '{CustomChoreName}' added successfully!");
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

            SaveWeeklyChores();
        }

        // notifies property change
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // clears weekly chores
        private void ClearWeeklyChores(object parameter)
        {
            WeeklyChoresList.Clear();
            SaveWeeklyChores();
        }
    }
}
