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
        public List<Chore> CompletedChores { get; set; }
        public List<Chore> AssignedChores { get; set; }
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

        public void CompleteChore()
        {
            throw new NotImplementedException();
        }

        public void AccessDatabase()
        {
            throw new NotImplementedException();
        }
    }
}