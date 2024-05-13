using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;

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
                string assignedChoresIds = string.Join(",", adminUser.AssignedChores.Select(chore => chore.ID));
                string completedChoresIds = string.Join(",", adminUser.CompletedChores.Select(chore => chore.ID));
                sql = $"UPDATE Users SET forename = '{adminUser.Forename}', surname = '{adminUser.Surname}', email = '{adminUser.Email}', assignedChores = '{assignedChoresIds}', completedChores = '{completedChoresIds}', totalChores = '{adminUser.TotalChores}', role = 'admin', password = '{adminUser.Password}' WHERE userId = '{adminUser.Id}';";
            }                                                                                                       
            else
            {
                RegularUser regularUser = (RegularUser)user;
                string assignedChoresIds = string.Join(",", regularUser.AssignedChores.Select(chore => chore.ID));
                string completedChoresIds = string.Join(",", regularUser.CompletedChores.Select(chore => chore.ID));
                sql = $"UPDATE Users SET forename = '{regularUser.Forename}', surname = '{regularUser.Surname}', email = '{regularUser.Email}', assignedChores = '{regularUser.AssignedChores}', completedChores = '{regularUser.CompletedChores}', totalChores = '{regularUser.TotalChores}', role = 'user', password = '{regularUser.Password}' WHERE userId = '{regularUser.Id}';";
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
                    List<Chore> assignedChores = new List<Chore>();
                    List<Chore> completedChores = new List<Chore>();
                    if (!result.IsDBNull(4) && !result.IsDBNull(5))
                    {
                        Debug.WriteLine(result.GetString(4));
                        // Split assignedChores and completedChores into ids
                        string[] assignedChoreIds = result.GetString(4).Split(',');
                        string[] completedChoreIds = result.GetString(5).Split(',');

                        // Retrieve chores from database based on ids
                        foreach (string choreId in assignedChoreIds)
                        {
                            // Fetch chore from database using choreId and add to assignedChores
                            Chore chore = GetChore(choreId);
                            if (chore != null)
                                assignedChores.Add(chore);
                        }

                        foreach (string choreId in completedChoreIds)
                        {
                            // Fetch chore from database using choreId and add to completedChores
                            Chore chore = GetChore(choreId);
                            if (chore != null)
                                completedChores.Add(chore);
                        }
                    }
                    else
                    {
                        completedChores = new List<Chore>();
                        assignedChores = new List<Chore>();
                    }

                    if (result.GetString(7) == "admin")
                    {
                        // Create Admin object and add to users
                        users.Add(new Admin(
                            result.GetValue(0).ToString(),
                            result.GetString(1),
                            result.GetString(2),
                            result.GetString(3),
                            result.GetString(8),
                            completedChores,
                            assignedChores,
                            Convert.ToInt32(result.GetValue(6))
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
                            result.GetString(8),
                            completedChores,
                            assignedChores,
                            Convert.ToInt32(result.GetInt32(6))
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
        private Chore GetChore(string choreId)
        {
            string sql = $"SELECT * FROM Chores WHERE ChoreID={choreId}";

            // Connect to the database
            DBConnection conn = new DBConnection();
            conn.Connect("Database/crabapple.db");

            SQLiteDataReader? result = conn.RunSQLQuery(sql);
            DateOnly dateOnly = DateOnly.Parse(result.GetString(5));
            if (result != null)
            {
                while (result.Read())
                {
                    Chore chore = new Chore(result.GetValue(0).ToString(), result.GetString(1), result.GetString(2), (int)result.GetValue(3), null, dateOnly, Convert.ToBoolean(result.GetValue(6)), Convert.ToBoolean(result.GetValue(7)));
                    return chore;
                }
            }
            else
            {
                Debug.WriteLine("Cannot get chore.");
            }
            return null;

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
                    Debug.WriteLine(result.GetString(5));
                    DateOnly dateOnly = DateOnly.Parse(result.GetString(5));
                    chores.Add(new Chore(result.GetValue(0).ToString(),
                        result.GetString(1),
                        result.GetString(2),
                        (int)result.GetDouble(3),
                        user!,
                        dateOnly,
                        Convert.ToBoolean(result.GetValue(6)),
                        Convert.ToBoolean(result.GetValue(7))));
                }
            }
            else
            {
                Debug.WriteLine("Cannot get list of all chores.");
            }

            return chores;
        }

        /// <summary>
        /// adds weekly chores to weekly chore table
        /// </summary>
        public void AddWeeklyChore(Chore chore)
        {
            string sql = $"INSERT INTO WeeklyChores(name, estimatedTime) VALUES ('{chore.Name}','{chore.EstimatedTime}');";

            RunSQL(sql);
        }

        public ObservableCollection<CompletedChoreLog> GetCompletedChoreLogs()
        {
            ObservableCollection<CompletedChoreLog> completedChoreLogs = new ObservableCollection<CompletedChoreLog>();

            string sql = @"
        SELECT c.name AS ChoreName, u.forename AS UserName, cc.completionDate AS CompletionDate
        FROM CompletedChores cc
        JOIN Chores c ON cc.choreId = c.choreId
        JOIN Users u ON cc.userId = u.userId
        ORDER BY cc.completionDate DESC;";

            SQLiteDataReader? result = RunSQLQuery(sql);

            if (result != null)
            {
                while (result.Read())
                {
                    completedChoreLogs.Add(new CompletedChoreLog
                    {
                        ChoreName = result.GetString(0),
                        UserName = result.GetString(1),
                        CompletionDate = result.GetDateTime(2)
                    });
                }
            }

            return completedChoreLogs;
        }
    }
}