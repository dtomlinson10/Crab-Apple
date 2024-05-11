using System;
using System.Collections.Generic;

namespace CrapApple
{
    public class RegularUser : User
    {
        public string Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Chore> CompletedChores { get; set; }
        public List<Chore> AssignedChores { get; set; }
        public int TotalChores { get; set; }

        public RegularUser(string id, string forename, string surname, string email, string password)
        {
            Id = id;
            Forename = forename;
            Surname = surname;
            Email = email;
            Password = password;
            AssignedChores = new List<Chore>();
            CompletedChores = new List<Chore>();
            TotalChores = 0;
        }

        public void CompleteChore(Chore choreToComplete, DBConnection db)
        {
            choreToComplete.IsCompleted = true;
            AssignedChores.Remove(choreToComplete);
            CompletedChores.Add(choreToComplete);

            db.RunSQL("UPDATE Chores SET IsCompleted = 1 WHERE Id = " + choreToComplete.ID);
        }
    }
}