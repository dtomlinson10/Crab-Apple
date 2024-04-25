using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal class Chore
    {
        string ID;
        string name;
        string description;
        float weight;
        User assignedUser;
        DateOnly dateOfCompletion;
        bool isCompleted;
        bool isLate;

        public Chore(string ID, string name, string description, float weight, User assignedUser, DateOnly dateOfCompletion)
        {
            this.ID = ID;
            this.name = name;
            this.description = description;
            this.weight = weight;
            this.assignedUser = assignedUser;
            this.dateOfCompletion = dateOfCompletion;
            this.isCompleted = false;
            this.isLate = false;
        }
    }
}
