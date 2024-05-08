using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal class Chore
    {
        String ID;
        String forename;
        String surname;
        String description;
        float weight;
        User assignedUser;
        DateOnly dateOfCompletion;
        bool isCompleted;
        bool isLate;

        public Chore(String ID, String forename,String surname, String description, float weight, User assignedUser, DateOnly dateOfCompletion, bool isCompleted, bool isLate)
        {
            this.ID = ID;
            this.forename = forename;
            this.surname = surname;
            this.description = description;
            this.weight = weight;
            this.assignedUser = assignedUser;
            this.dateOfCompletion = dateOfCompletion;
            this.isCompleted = false;
            this.isLate = false;
        }

        public Chore(string v1, string v2, string v3, string v4, double v5, User daniel, DateOnly date_completed, bool v6, bool v7)
        {
        }

        public void addToDatabase(DbConnection connection)
        {
            // db connection needs to be implemented
            throw new NotImplementedException();
        }
    }
}
