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
        String name { get; }
        String id { get; }
        List<Chore> assignedChores { get; }
        int choresCompleted { get; }
        String password { get; }

        void completeChore();
    }
}
