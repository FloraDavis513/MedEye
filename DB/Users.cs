using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MedEye.DB
{
    public struct Gamer
    {
        public int id { get; set; }
        public string first_name { get; set; }
        public string second_name { get; set; }
        public string last_name { get; set; }
        public string birth_date { get; set; }
        public string sex { get; set; }
    }


    static class Users
    {
        private static SortedDictionary<string, int> users = new SortedDictionary<string, int>();

        static Users()
        {
            var gamer = GetUserById(-1);
            if (gamer.id != -1)
            {
                AddUser(new Gamer
                {
                    id = -1,
                    first_name = "default",
                    second_name = "default",
                    last_name = "default",
                    birth_date = "01.01.0001",
                    sex = "default"
                });
            }
        }

        public static void AddUser(Gamer gamer)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string strSql = "INSERT INTO [Users] ([id], [first_name], [second_name], [last_name], " +
                                    "[birth_date], [sex]) VALUES(@param6, @param1, @param2, @param3, @param4, @param5)";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@param1", gamer.first_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param2", gamer.second_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param3", gamer.last_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param4", gamer.birth_date));
                    cmd.Parameters.Add(new SQLiteParameter("@param5", gamer.sex));
                    cmd.Parameters.Add(new SQLiteParameter("@param6", gamer.id));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void AddUser(string first_name, string second_name, string last_name, string birth_date,
            string sex)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string strSql = "INSERT INTO [Users] ([first_name], [second_name], [last_name], [birth_date], " +
                                    "[sex]) VALUES(@param1, @param2, @param3, @param4, @param5)";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@param1", first_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param2", second_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param3", last_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param4", birth_date));
                    cmd.Parameters.Add(new SQLiteParameter("@param5", sex));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void UpdateUser(int user_id, string first_name, string second_name, string last_name,
            string birth_date, string sex)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string strSql = "UPDATE [Users] SET [first_name] = @param1, [second_name] = @param2, " +
                                    "[last_name] = @param3, [birth_date] = @param4, [sex] = @param5 WHERE [id] = @param6";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@param1", first_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param2", second_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param3", last_name));
                    cmd.Parameters.Add(new SQLiteParameter("@param4", birth_date));
                    cmd.Parameters.Add(new SQLiteParameter("@param5", sex));
                    cmd.Parameters.Add(new SQLiteParameter("@param6", user_id));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void DeleteUserById(int user_id)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string strSql = "DELETE FROM [Users] WHERE [id] = @param1";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@param1", user_id));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static SortedDictionary<string, int> GetUserList()
        {
            users.Clear();
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string strSql = "SELECT [id], [first_name], [second_name], [last_name] FROM [Users];";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteDataReader sqlite_datareader;
                    sqlite_datareader = cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        var user_id = sqlite_datareader.GetInt32("id");
                        var first_name = sqlite_datareader.GetValue("first_name");
                        var second_name = sqlite_datareader.GetValue("second_name");
                        var last_name = sqlite_datareader.GetValue("last_name");
                        var name = $"{last_name} {first_name} {second_name}";

                        if (user_id == -1) continue;

                        users.Add(name, user_id);
                    }

                    conn.Close();
                }
            }

            return users;
        }

        public static Gamer GetUserById(int user_id)
        {
            var gamer = new Gamer();
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    string strSql = "SELECT * FROM [Users] WHERE id = @param1;";
                    cmd.CommandText = strSql;
                    cmd.Parameters.Add(new SQLiteParameter("@param1", user_id));
                    cmd.Connection = conn;
                    conn.Open();

                    SQLiteDataReader sqlite_datareader;
                    sqlite_datareader = cmd.ExecuteReader();
                    while (sqlite_datareader.Read())
                    {
                        gamer.id = sqlite_datareader.GetInt32("id");
                        gamer.first_name = sqlite_datareader.GetString("first_name");
                        gamer.second_name = sqlite_datareader.GetString("second_name");
                        gamer.last_name = sqlite_datareader.GetString("last_name");
                        gamer.sex = sqlite_datareader.GetString("sex");
                        gamer.birth_date = sqlite_datareader.GetString("birth_date");
                    }

                    conn.Close();
                }
            }

            return gamer;
        }
    }
}