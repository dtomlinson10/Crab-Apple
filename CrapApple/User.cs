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
        List<Chore> OptionalChores { get; set; }
        //void CompleteChore(Chore choreToComplete, DBConnection db);

        public void CompleteChore(Chore choreToComplete, DBConnection db)
        {
            //choreToComplete.IsCompleted = true;
            //AssignedChores.Remove(choreToComplete);
            //OptionalChores.Remove(choreToComplete);
            //CompletedChores.Add(choreToComplete);

            //db.ModifyChore(choreToComplete);

            // Log the completed chore in the database
            //string sql = $"INSERT INTO CompletedChores(choreId, userId, completionDate) VALUES ({choreToComplete.ID}, {Id}, '{DateTime.Today.ToString("yyyy-MM-dd")}');";
            //db.RunSQL(sql);///
        }
    }
}