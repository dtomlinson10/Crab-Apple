//class manages the data and logic for the Chore Completion View in the app
//handles the list of users, assigned chores, optional chores, and completed chores.
//also provides functionality to mark chores as complete, load completed chore logs from the database,
//and display an alert message if it's the end of the week

using CrapApple;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using System.Linq;

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

    public ChoreCompletionViewModel(ObservableCollection<User> personList)
    {
        PersonList = personList;
        AssignedChores = new ObservableCollection<Chore>();
        OptionalChores = new ObservableCollection<Chore>();
        CompletedChores = new ObservableCollection<Chore>();
        CompletedChoreLogs = new ObservableCollection<CompletedChoreLog>();

        MarkAsCompleteCommand = new RelayCommand(MarkAsComplete);

        // Load completed chore logs
        LoadCompletedChoreLogs();

        // Check for end of week and update alert message
        if (IsEndOfWeek())
        {
            EndOfWeekAlertMessage = "End of week! Please complete your chores.";
        }
    }


    public ObservableCollection<CompletedChoreLog> CompletedChoreLogs { get; set; }

    private void LoadCompletedChoreLogs()
    {
        CompletedChoreLogs.Clear();

        var db = new DBConnection();
        db.Connect("Database/crabapple.db");

        CompletedChoreLogs = db.GetCompletedChoreLogs();

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
        //logic to check if it's the end of the week
        return DateTime.Today.DayOfWeek == DayOfWeek.Sunday;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}