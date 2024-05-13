//class is similar to ChoreCompletionViewModel.cs but is specifically for the ChoreCompletion.xaml view
//has an additional method to load completed chore logs directly from the database using SQL queries

using CrapApple;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Linq;

namespace CrapApple
{
    public class ChoreCompletionViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<User> PersonList { get; }

        private User _selectedUser;
        public User SelectedUser
        {
            get { return _selectedUser; }
            set
            {
                _selectedUser = value;
                OnPropertyChanged(nameof(SelectedUser));
                UpdateChores();
            }
        }

        public ObservableCollection<Chore> AssignedChores { get; set; }
        public ObservableCollection<Chore> OptionalChores { get; set; }
        public ObservableCollection<Chore> CompletedChores { get; set; }

        public ICommand MarkAsCompleteCommand { get; }

        public string EndOfWeekAlertMessage { get; set; }

        public ObservableCollection<CompletedChoreLog> CompletedChoreLogs { get; set; }

        public ChoreCompletionViewModel(ObservableCollection<User> personList)
        {
            PersonList = personList;
            AssignedChores = new ObservableCollection<Chore>();
            OptionalChores = new ObservableCollection<Chore>();
            CompletedChores = new ObservableCollection<Chore>();
            CompletedChoreLogs = new ObservableCollection<CompletedChoreLog>();

            MarkAsCompleteCommand = new RelayCommand(MarkAsComplete);

            // Check for end of week and update alert message
            if (IsEndOfWeek())
            {
                EndOfWeekAlertMessage = "End of week! Please complete your chores.";
            }
        }

        private void LoadCompletedChoreLogs()
        {
            CompletedChoreLogs.Clear();

            var db = new DBConnection();
            db.Connect("Database/crabapple.db");

            string sql = @"
                SELECT c.name AS ChoreName, u.forename AS UserName, cc.completionDate AS CompletionDate
                FROM CompletedChores cc
                JOIN Chores c ON cc.choreId = c.choreId
                JOIN Users u ON cc.userId = u.userId
                ORDER BY cc.completionDate DESC;";


            var result = db.RunSQLQuery(sql);
            while (result.Read())
            {
                CompletedChoreLogs.Add(new CompletedChoreLog
                {
                    ChoreName = result.GetString(0),
                    UserName = result.GetString(1),
                    CompletionDate = result.GetDateTime(2)
                });
            }

            db.Disconnect();
        }

        private void UpdateChores()
        {
            AssignedChores.Clear();
            OptionalChores.Clear();
            CompletedChores.Clear();

            if (SelectedUser != null)
            {
                foreach (var chore in SelectedUser.AssignedChores)
                {
                    AssignedChores.Add(chore);
                }
                foreach (var chore in SelectedUser.OptionalChores)
                {
                    OptionalChores.Add(chore);
                }

                foreach (var chore in SelectedUser.CompletedChores)
                {
                    CompletedChores.Add(chore);
                }
            }
        }

        private void MarkAsComplete(object parameter)
        {
            var completedAssignedChores = AssignedChores.Where(c => c.IsCompleted).ToList();
            var completedOptionalChores = OptionalChores.Where(c => c.IsCompleted).ToList();

            foreach (var chore in completedAssignedChores)
            {
                AssignedChores.Remove(chore);
                CompletedChores.Add(chore);
                SelectedUser.CompleteChore(chore, new DBConnection());
            }

            foreach (var chore in completedOptionalChores)
            {
                OptionalChores.Remove(chore);
                CompletedChores.Add(chore);
                SelectedUser.CompleteChore(chore, new DBConnection());
            }
        }

        private bool IsEndOfWeek()
        {
            // Logic to check if it's the end of the week
            // For example, you can check if today is Sunday
            return DateTime.Today.DayOfWeek == DayOfWeek.Sunday;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}