using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    public interface User
    {
        public String id { get; }
        public String forename { get; }
        public String surname { get; }
        public String email { get; }
        public List<Chore> assignedChores { get; }
        public List<Chore> completedChores { get; }
        public int totalChores { get; }
        public String password { get; }

        void completeChore();
    }
}
