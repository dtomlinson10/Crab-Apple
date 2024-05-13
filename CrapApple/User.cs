using System;
using System.Collections.Generic;

namespace CrapApple
{
    /// <summary>
    /// user interface for admin and regular user to be generated from
    /// </summary>
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
        List<Chore> OptionalChores { get; set; }
        void CompleteChore(Chore choreToComplete, DBConnection db);
    }
}