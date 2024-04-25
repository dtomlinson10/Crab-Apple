using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal class RegularUser : User
    {
        public string id { get; set; }
        public string forename { get; set; }
        public string surname { get; set; }
        public string email { get; set; }
        public String password { get; set; }
        public List<Chore> completedChores { get; set; }
        public List<Chore> assignedChores { get; set; }
        public int choresCompleted { get; set; }
        public int totalChores { get; set; }

        public RegularUser(string id, string forename, string surname, string email, string password)
        {
            this.id = id;
            this.forename = forename;
            this.surname = surname;
            this.email = email;
            this.password = password;
            this.assignedChores = new List<Chore>();
            this.completedChores = new List<Chore>();
            this.choresCompleted = 0;
            this.totalChores = 0;
        }

        public void completeChore()
        {
            throw new NotImplementedException();
        }
    }
}
