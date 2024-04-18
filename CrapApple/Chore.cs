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
        float weight;
        User assignedUser;
        DateOnly dateOfCompletion;
        bool completed;

        public Chore(string ID, string name, float weight, User assignedUser, DateOnly dateOfCompletion, bool completed)
        {
            this.ID = ID;
            this.name = name;
            this.weight = weight;
            this.assignedUser = assignedUser;
            this.dateOfCompletion = dateOfCompletion;
            this.completed = completed;
        }
    }
}
