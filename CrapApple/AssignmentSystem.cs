//V1, only changed namespace variable stuff

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;

namespace CrapApple
{
    public class AssignmentSystem
    {
        public String systemID;
        public List<Chore> choreList;
        public List<User> userList;
        public DBConnection conn;

        public AssignmentSystem(String systemID, ObservableCollection<Chore> choreList, ObservableCollection<User> userList)
        {
            this.systemID = systemID;
            this.userList = new List<User>(userList);
            this.choreList = new List<Chore>(choreList);
            this.conn = new DBConnection();
        }

        /// <summary>
        /// ///////////////
        /// </summary>


        public void rankChores(List<Chore> choresList)
        {

        }

        public void autoAssignChores(List<Chore> choresList, List<User> userList)
        {
            double totalWeight = choresList.Sum(c => c.Weight);
            double averageWeightPerUser = totalWeight / userList.Count;

            choresList.Sort((c1, c2) => c2.Weight.CompareTo(c1.Weight));
            int currentUserIndex = 0;


            foreach (Chore chore in choresList)
            {
                if (currentUserIndex >= userList.Count)
                {
                    currentUserIndex = 0;
                }


                User currentUser = userList[currentUserIndex];
                if (currentUser is RegularUser regularUser)

                {
                    regularUser.AssignedChores.Add(chore);
                    currentUserIndex++;
                    Debug.WriteLine($"Assigned chore {chore.Name} to user {regularUser.Forename} {regularUser.Surname}");
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (User user in userList)
            {

                if (user is RegularUser regularUser)
                {
                    sb.Append($"{regularUser.Forename} {regularUser.Surname} has been assigned the following chores: ");
                    foreach (Chore chore in regularUser.AssignedChores)
                    {
                        sb.Append($"{chore.Name}, ");
                    }


                    sb.Append("\n");
                }
            }
            MessageBox.Show(sb.ToString(), "Success!");

            // Commit changes to the database
            foreach (var user in userList)
            {
                conn.ModifyUser(user);
            }
        }
    }
}