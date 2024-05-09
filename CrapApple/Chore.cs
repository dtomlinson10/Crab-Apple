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
        String ID { get; set; }
        String name { get; set; }
        String description { get; set; }
        double weight { get; set; }
        User assignedUser { get; set; }
        DateOnly dateOfCompletion { get; set; }
        bool isCompleted { get; set; }
        bool isLate { get; set; }

        public Chore(String ID, String name, String description, double weight, User assignedUser, DateOnly dateOfCompletion, bool isCompleted, bool isLate)
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

        public void addToDatabase(DbConnection connection)
        {
            // db connection needs to be implemented
            throw new NotImplementedException();
        }
    }
}
