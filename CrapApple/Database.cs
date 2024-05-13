using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;

namespace CrapApple
{
    public class DBConnection
    {
        private SQLiteConnection? m_sqliteConnection;

        public void Connect(string crabapple)
        {
            m_sqliteConnection = new SQLiteConnection($"Data Source={crabapple}; foreign_keys=true;");

            try
            {
                m_sqliteConnection.Open();
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"SQLite Could not connect to {crabapple}.");
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception in DBConnction::Connect().");
                Debug.WriteLine(ex.Message);
            }
        }

        public void Disconnect()
        {
            if (m_sqliteConnection != null)
            {
                m_sqliteConnection.Close();
            }
        }

        public bool RunSQL(string sql)
        {
            if (m_sqliteConnection != null)
            {
                try
                {
                    SQLiteCommand SQLcommand = m_sqliteConnection.CreateCommand();
                    SQLcommand.CommandText = sql;
                    SQLcommand.ExecuteNonQuery();
                    return true;
                }
                catch (SQLiteException ex)
                {
                    Debug.WriteLine($"SQLite Could not run query: {sql}.");
                    Debug.WriteLine(ex.Message);
                    return false;
                }
            }
            else
            {
                Debug.WriteLine($"Can't run '{sql}'. You need to connect to a database first.");
                return false;
            }
        }

        public SQLiteDataReader? RunSQLQuery(string sql)
        {
            SQLiteDataReader? dataReader = null;
            if (m_sqliteConnection != null)
            {
                try
                {
                    SQLiteCommand SQLcommand = m_sqliteConnection.CreateCommand();
                    SQLcommand.CommandText = sql;
                    dataReader = SQLcommand.ExecuteReader(); //small change here, this way can use data reader in other parts of code / pass it
                }
                catch (SQLiteException ex)
                {
                    Debug.WriteLine($"SQLite Could not run query: {sql}.");
                    Debug.WriteLine(ex.Message);
                }
            }
            else
            {
                Debug.WriteLine($"Can't run '{sql}'. You need to connect to a database first.");
            }
            return dataReader;
        }

        public void AddUser(User user)
        {
            if (user is RegularUser regularUser)
            {
                string sql = $"INSERT INTO Users(forename,surname,email,assignedChores,completedChores,totalChores,role,password) VALUES ('{regularUser.Forename}','{regularUser.Surname}','{regularUser.Email}','{regularUser.AssignedChores.Count}','{regularUser.CompletedChores.Count}','{regularUser.TotalChores}','{"regularUser"}','{regularUser.Password}');";

                // Connect to the database
                DBConnection conn = new DBConnection();
                conn.Connect("Database/crabapple.db");

                // Run the SQL
                if (conn.RunSQL(sql) == false)
                {
                    Debug.WriteLine("Could not insert in AddUser");
                }

                // Disconnect
                conn.Disconnect();
            }
        }

        public void ModifyUser(User user)
        {
            string sql;
            if (user is Admin)
            {
                Admin adminUser = (Admin)user;
                sql = $"UPDATE Users SET forename = '{adminUser.Forename}', surname = '{adminUser.Surname}', email = '{adminUser.Email}', assignedChores = '{adminUser.AssignedChores}', completedChores = '{adminUser.CompletedChores}', totalChores = '{adminUser.TotalChores}', role = 'admin', password = '{adminUser.Password}' WHERE id = '{adminUser.Id}';";
            }
            else
            {
                RegularUser regularUser = (RegularUser)user;
                sql = $"UPDATE Users SET forename = '{regularUser.Forename}', surname = '{regularUser.Surname}', email = '{regularUser.Email}', assignedChores = '{regularUser.AssignedChores}', completedChores = '{regularUser.CompletedChores}', totalChores = '{regularUser.TotalChores}', role = 'user', password = '{regularUser.Password}' WHERE id = '{regularUser.Id}';";
            }

            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            if (conn.RunSQL(sql) == false)
            {
                Debug.WriteLine("Could not modify user in ModifyUser");
            }

            conn.Disconnect();
        }

        public void DeleteUser(string ID)
        {
            string sql = $"DELETE FROM Users WHERE id = '{ID}';";

            // Connect to the database
            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            // Run the SQL
            if (conn.RunSQL(sql) == false)
            {
                Debug.WriteLine("Could not delete a user in DeleteUser");
            }

            // Disconnect
            conn.Disconnect();
        }

        public ObservableCollection<User> GetUsers()
        {
            ObservableCollection<User> users = new ObservableCollection<User>();
            string sql = "SELECT * FROM Users;";

            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            SQLiteDataReader? result = conn.RunSQLQuery(sql);

            if (result != null)
            {
                while (result.Read())
                {
                    if (result.GetString(7) == "admin")
                    {
                        // Split assignedChores and completedChores into ids
                        string[] assignedChoreIds = result.GetString(8).Split(',');
                        string[] completedChoreIds = result.GetString(9).Split(',');

                        List<Chore> assignedChores = new List<Chore>();
                        List<Chore> completedChores = new List<Chore>();

                        // Retrieve chores from database based on ids
                        foreach (string choreId in assignedChoreIds)
                        {
                            // Fetch chore from database using choreId and add to assignedChores
                           // Chore chore = FetchChoreFromDatabase(choreId);
                            //if (chore != null)
                               // assignedChores.Add(chore);
                        }

                        foreach (string choreId in completedChoreIds)
                        {
                            // Fetch chore from database using choreId and add to completedChores
                           // Chore chore = FetchChoreFromDatabase(choreId);
                           // if (chore != null)
                               // completedChores.Add(chore);
                        }

                       // Create Admin object and add to users
                        users.Add(new Admin(
                            result.GetValue(0).ToString(),
                            result.GetString(1),
                            result.GetString(2),
                            result.GetString(3),
                            result.GetString(4),
                            completedChores,
                            assignedChores,
                            (int)result.GetValue(7)
                        ));
                    }
                    else
                    {
                        // Create RegularUser object and add to users
                        users.Add(new RegularUser(
                            result.GetValue(0).ToString(),
                            result.GetString(1),
                            result.GetString(2),
                            result.GetString(3),
                            result.GetString(8)
                        ));
                    }
                }
            }
            else
            {
                Debug.WriteLine("Cannot get list of all students.");
            }

            return users;
        }

        // Helper method to fetch chore from the database
        private void FetchChoreFromDatabase(string choreId)
        {
           
        }


        public void AddChore(Chore chore)
        {
            string sql = $"INSERT INTO Chores(name,description,weight,assignedUser,dateOfCompletion,isCompleted,isLate) VALUES ('{chore.Name}','{chore.Description}','{chore.Weight}','{chore.AssignedUser?.Id ?? ""}','{chore.DateOfCompletion}','{chore.IsCompleted}','{chore.IsLate}');";

            // Connect to the database
            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            // Run the SQL
            if (conn.RunSQL(sql) == false)
            {
                Debug.WriteLine("Could not insert into Chore");
            }

            // Disconnect
            conn.Disconnect();
        }

        public void ModifyChore(Chore chore)
        {
            string sql = $"UPDATE Chores SET name = '{chore.Name}', description = {chore.Description}, weight = {chore.Weight}, assignedUser = '{chore.AssignedUser?.Id ?? ""}', dateOfCompletion = '{chore.DateOfCompletion}', isCompleted = '{chore.IsCompleted}', isLate = '{chore.IsLate}' WHERE id = '{chore.ID}';";

            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            if (conn.RunSQL(sql) == false)
            {
                Debug.WriteLine("Could not modify chore in ModifyChore");
            }

            conn.Disconnect();
        }

        public void DeleteChore(string ID)
        {
            string sql = $"DELETE FROM Chores WHERE id = '{ID}';";

            // Connect to the database
            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            // Run the SQL
            if (conn.RunSQL(sql) == false)
            {
                Debug.WriteLine("Could not delete a chore in Chore");
            }

            // Disconnect
            conn.Disconnect();
        }

        public ObservableCollection<Chore> GetChores(ObservableCollection<User> userList)
        {
            ObservableCollection<Chore> chores = new ObservableCollection<Chore>();
            string sql = $"SELECT * FROM Chores;";

            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            SQLiteDataReader? result = conn.RunSQLQuery(sql);

            if (result != null)
            {
                while (result.Read())
                {
                    // Do the same in reverse for getUsers
                    // Fix the issue with DateTime -> DateOnly
                    String userID = result.GetString(4);
                    User user = userList.FirstOrDefault(u => u.Id == userID);
                    DateOnly dateOnly = DateOnly.FromDateTime(result.GetDateTime(5));
                    chores.Add(new Chore(result.GetValue(0).ToString(), result.GetString(1), result.GetString(2), (int)result.GetDouble(3), user, dateOnly, result.GetBoolean(6), result.GetBoolean(7)));
                }
            }
            else
            {
                Debug.WriteLine("Cannot get list of all chores.");
            }

            return chores;
        }
    }
}