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
        String id { get; }
        String forename { get; }
        String surname { get; }
        String email { get; }
        List<Chore> assignedChores { get; }
        List<Chore> completedChores { get; }
        int totalChores { get; }
        String password { get; }

        void completeChore();
    }
}
