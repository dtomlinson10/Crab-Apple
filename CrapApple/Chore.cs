//V1

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    /// <summary>
    /// chore class for creating and saving new chores
    /// </summary>
    public class Chore : INotifyPropertyChanged
    {
        public String ID;
        public String Description;
        public double Weight;
        public User AssignedUser;
        public DateOnly DateOfCompletion;
        public bool IsCompleted;
        public bool IsLate;


        //below are properties for data encapsulation / easy initialization
        private bool _isChecked;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                _isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

        private int _estimatedTime;
        public int EstimatedTime
        {
            get { return _estimatedTime; }
            set
            {
                _estimatedTime = value;
                OnPropertyChanged(nameof(EstimatedTime));
            }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            //event to check if property has been changed
        }

        public Chore(String ID, String name, String description, int estimatedTime, User assignedUser, DateOnly dateOfCompletion, bool isCompleted, bool isLate)
        {
            this.ID = ID;
            this.Name = name;
            this.Description = description;
            this.EstimatedTime = estimatedTime;
            this.AssignedUser = assignedUser;
            this.DateOfCompletion = dateOfCompletion;
            this.IsCompleted = isCompleted;
            this.IsLate = isLate;
        }
    }
}
