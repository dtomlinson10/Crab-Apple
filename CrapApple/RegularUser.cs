using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal class RegularUser : User
    {
        public string name { get; set; }
        public string id { get; set; }
        public List<Chore> assignedChores { get; set; }
        public int choresCompleted { get; set; }
        public String password { get; set; }

        public RegularUser(string name, string id, string password)
        {
            this.name = name;
            this.id = id;
            this.assignedChores = new List<Chore>();
            this.choresCompleted = 0;
            this.password = password;
        }

        public void completeChore()
        {
            throw new NotImplementedException();
        }
    }
}
