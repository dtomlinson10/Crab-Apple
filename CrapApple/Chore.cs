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
        public String ID { get; set; }
        public String name { get; set; }
        public String description { get; set; }
        public double weight { get; set; }
        public User assignedUser { get; set; }
        public DateOnly dateOfCompletion { get; set; }
        public bool isCompleted { get; set; }
        public bool isLate { get; set; }

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
