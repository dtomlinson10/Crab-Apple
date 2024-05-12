using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;

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
                string sql = $"INSERT INTO Users(userId,forename,surname,email,assignedChores,completedChores,totalChores,role,password) VALUES ('{regularUser.Id}', '{regularUser.Forename}','{regularUser.Surname}','{regularUser.Email}','{regularUser.AssignedChores.Count}','{regularUser.CompletedChores.Count}','{regularUser.TotalChores}','{"regularUser"}','{regularUser.Password}');";

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

        public void GetUsers()
        {
            string sql = $"SELECT * FROM Users;";

            DBConnection conn = new DBConnection();
            if (conn.RunSQL(sql) == false)
            {
                Debug.WriteLine("Could not get users in GetUsers");
            }

            // Generate a list from the result of RunSQL
            List<User> users = new List<User>();
            SQLiteDataReader? dataReader = conn.RunSQLQuery(sql);
            
        }

        public void AddChore(Chore chore)
        {
            string sql = $"INSERT INTO Chores(choreId,name,description,weight,assignedUser,dateOfCompletion,isCompleted,isLate) VALUES ('{chore.ID}', '{chore.Name}','{chore.Description}','{chore.Weight}','{chore.AssignedUser?.Id ?? ""}','{chore.DateOfCompletion}','{chore.IsCompleted}','{chore.IsLate}');";

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
    }
}