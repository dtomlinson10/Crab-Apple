using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrapApple
{
    internal interface User
    {
        String id { get; }
        String forename { get; }
        String surname { get; }
        String email { get; }
        List<Chore> assignedChores { get; } // NOTE: THIS MAY NOT BE VIABLE TO PUSH AND PULL TO THE DATABASE!
        List<Chore> completedChores { get; }
        int choresCompleted { get; }
        int totalChores { get; }
        String password { get; }

        void completeChore();
    }
}
