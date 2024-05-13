//V1

using System;
using System.Collections.Generic;

namespace CrapApple
{
    public class Admin : User
    {
        public string Id { get; set; }
        public string Forename { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<Chore>? CompletedChores { get; set; }
        public List<Chore>? AssignedChores { get; set; }
        public List<Chore> OptionalChores { get; set; } = new List<Chore>();
        public int TotalChores { get; set; }

        public Admin(string id, string forename, string surname, string email, string password)
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

        public Admin(string id, string forename, string surname, string email, string password, List<Chore> completedChores, List<Chore> assignedChores, int totalChores)
        {
            Id = id;
            Forename = forename;
            Surname = surname;
            Email = email;
            Password = password;
            CompletedChores = completedChores;
            AssignedChores = assignedChores;
            TotalChores = totalChores;
        }

        ///
        public void CompleteChore(Chore choreToComplete, DBConnection db)
        {
            choreToComplete.IsCompleted = true;
            AssignedChores.Remove(choreToComplete);
            OptionalChores.Remove(choreToComplete);
            CompletedChores.Add(choreToComplete);

            db.ModifyChore(choreToComplete);

            // Log the completed chore in the database
            string sql = $"INSERT INTO CompletedChores(choreId, userId, completionDate) VALUES ({choreToComplete.ID}, {Id}, '{DateTime.Today.ToString("yyyy-MM-dd")}');";
            db.RunSQL(sql);
        }
        ///
    }
}