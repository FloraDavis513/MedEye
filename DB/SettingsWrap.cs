using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

namespace MedEye.DB
{
    public struct Settings
    {
        public int UserId;
        public int GameId;
        public int Priority;
        public int Distance;
        public bool IsRed;
        public int Frequency;
        public int FlickerMode;
        public int RedBrightness;
        public int BlueBrightness;
        public int Level;
        public int ExerciseDuration;

        public Settings SetPriority(int priority)
        {
            return this with { Priority = priority };
        }
    }

    public static class SettingsWrap
    {
        public static void AddSettings(Settings settings)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql = $"INSERT INTO [Settings] ([user_id], [game_id], [priority], [distance], " +
                                          $"[is_red], [frequency], [flicker_mode], [blue_brightness], " +
                                          $"[red_brightness], [level], [timer]) " +
                                          $"VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", settings.UserId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", settings.GameId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", settings.Priority));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", settings.Distance));
                    cmd.Parameters.Add(new SQLiteParameter("@p5", settings.IsRed));
                    cmd.Parameters.Add(new SQLiteParameter("@p6", settings.Frequency));
                    cmd.Parameters.Add(new SQLiteParameter("@p7", settings.FlickerMode));
                    cmd.Parameters.Add(new SQLiteParameter("@p8", settings.BlueBrightness));
                    cmd.Parameters.Add(new SQLiteParameter("@p9", settings.RedBrightness));
                    cmd.Parameters.Add(new SQLiteParameter("@p10", settings.Level));
                    cmd.Parameters.Add(new SQLiteParameter("@p11", settings.ExerciseDuration));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void UpdateSettings(Settings settings)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql = $"UPDATE [Settings] SET [distance] = @p4, [is_red] = @p5, " +
                                          $"[frequency] = @p6, [flicker_mode] = @p7, [blue_brightness] = @p8, " +
                                          $"[red_brightness] = @p9, [level] = @p10, [timer] = @p11 " +
                                          $"WHERE [user_id] = @p1 AND [game_id] = @p2 AND [priority] = @p3";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", settings.UserId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", settings.GameId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", settings.Priority));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", settings.Distance));
                    cmd.Parameters.Add(new SQLiteParameter("@p5", settings.IsRed));
                    cmd.Parameters.Add(new SQLiteParameter("@p6", settings.Frequency));
                    cmd.Parameters.Add(new SQLiteParameter("@p7", settings.FlickerMode));
                    cmd.Parameters.Add(new SQLiteParameter("@p8", settings.BlueBrightness));
                    cmd.Parameters.Add(new SQLiteParameter("@p9", settings.RedBrightness));
                    cmd.Parameters.Add(new SQLiteParameter("@p10", settings.Level));
                    cmd.Parameters.Add(new SQLiteParameter("@p11", settings.ExerciseDuration));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void DeleteSettings(Settings settings)
        {
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql = $"DELETE FROM [Settings] WHERE [user_id] = @p1 " +
                                          $"AND [game_id] = @p2 AND [priority] = @p3";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", settings.UserId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", settings.GameId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", settings.Priority));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static List<Settings> GetSettings(int userId)
        {
            var settingsList = new List<Settings>();
            using (SQLiteConnection conn = new SQLiteConnection(DBConst.DB_PATH))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql = "SELECT [user_id], [game_id], [priority], [distance], " +
                                          $"[is_red], [frequency], [flicker_mode], [blue_brightness], " +
                                          $"[red_brightness], [level], [timer]" +
                                          $" FROM [Settings] WHERE [user_id] = @p1";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", userId));
                    conn.Open();

                    var sqliteDataReader = cmd.ExecuteReader();
                    while (sqliteDataReader.Read())
                    {
                        var settings = new Settings
                        {
                            UserId = sqliteDataReader.GetInt32("user_id"), 
                            GameId = sqliteDataReader.GetInt32("game_id"),
                            Priority = sqliteDataReader.GetInt32("priority"),
                            Distance = sqliteDataReader.GetInt32("distance"),
                            IsRed = sqliteDataReader.GetBoolean("is_red"),
                            Frequency = sqliteDataReader.GetInt32("frequency"),
                            FlickerMode = sqliteDataReader.GetInt32("flicker_mode"),
                            RedBrightness = sqliteDataReader.GetInt32("red_brightness"),
                            BlueBrightness = sqliteDataReader.GetInt32("blue_brightness"),
                            Level = sqliteDataReader.GetInt32("level"),
                            ExerciseDuration = sqliteDataReader.GetInt32("timer"),
                        };
                        settingsList.Add(settings);
                    }
                    conn.Close();
                }
            }
            return settingsList;
        }
    }
}