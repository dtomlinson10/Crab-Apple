using System;
using System.Collections.Generic;

namespace CrapApple
{
    public interface User
    {
        string Id { get; }
        string Forename { get; }
        string Surname { get; }
        string Email { get; }
        List<Chore> AssignedChores { get; }
        List<Chore> CompletedChores { get; }
        int TotalChores { get; }
        string Password { get; }

        void CompleteChore();
    }
}