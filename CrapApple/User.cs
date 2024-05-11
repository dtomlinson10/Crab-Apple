using System;
using System.Collections.Generic;

namespace CrapApple
{
    public interface User
    {
        public string Id { get; }
        public string Forename { get; }
        public string Surname { get; }
        public string Email { get; }
        public List<Chore> AssignedChores { get; }
        public List<Chore> CompletedChores { get; }
        public int TotalChores { get; }
        public string Password { get; }

        void CompleteChore(Chore choreToComplete, DBConnection db);
    }
}