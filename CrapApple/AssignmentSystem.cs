using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace CrapApple
{
    public class AssignmentSystem
    {
        public String systemID;
        public List<Chore> choreList;
        public List<User> userList;
        public AssignmentSystem(String systemID, List<Chore> choreList, List<User> userList)
        {
            this.systemID = systemID;
            this.userList = userList;
            this.choreList = choreList;
        }

        public void rankChores(List<Chore> choresList)
        {

        }

        public void autoAssignChores(List<Chore> choresList, List<User> userList)
        {
            double totalWeight = choresList.Sum(c => c.weight);
            double averageWeightPerUser = totalWeight / userList.Count;

            choresList.Sort((c1, c2) => c2.weight.CompareTo(c1.weight));
            int currentUserIndex = 0;

            foreach (Chore chore in choresList)
            {
                if (currentUserIndex >= userList.Count)
                {
                    currentUserIndex = 0;
                }

                User currentUser = userList[currentUserIndex];
                currentUser.assignedChores.Add(chore);
                currentUserIndex++;
                Debug.WriteLine($"Assigned chore {chore.name} to user {currentUser.forename} {currentUser.surname}");
            }

            StringBuilder sb = new StringBuilder();
            foreach (User user in userList)
            {
                sb.Append($"{user.forename} {user.surname} has been assigned the following chores: ");
                foreach (Chore chore in user.assignedChores)
                {
                    sb.Append($"{chore.name}, ");
                }
                sb.Append("\n");
            }
            MessageBox.Show(sb.ToString(), "Success!");

            // Commit changes to the database

        }
    }
}
