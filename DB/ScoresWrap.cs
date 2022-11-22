using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

namespace MedEye.DB
{
    public struct Scores
    {
        public int UserId;
        public int GameId;
        public DateTime DateCompletion;
        public int Level;
        public double Score;
        public double MeanDeviationsX;
        public double MeanDeviationsY;
        public double MinDeviationsX;
        public double MinDeviationsY;
        public double MaxDeviationsX;
        public double MaxDeviationsY;

        public override string ToString()
        {
            return "Всего очков: " + Math.Abs(Score) + "\n" +
                   "Минимальное отклонение по горизонтали: " + Math.Round(MinDeviationsX, 1) + "\n" +
                   "Минимальное отклонение по вертикали: " + Math.Round(MinDeviationsY, 1) + "\n" +
                   "Среднее отклонение по горизонтали: " + Math.Round(MeanDeviationsX, 1) + "\n" +
                   "Среднее отклонение по вертикали: " + Math.Round(MeanDeviationsY, 1) + "\n" +
                   "Максимальное отклонение по горизонтали: " + Math.Round(MaxDeviationsX, 1) + "\n" +
                   "Максимальное отклонение по вертикали: " + Math.Round(MaxDeviationsY, 1);
        }
    }

    public class ScoresWrap
    {
        public static void AddScores(Scores scores)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source = ..\\..\\..\\DB\\medeye.db"))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql = $"INSERT INTO [Scores] ([user_id], [game_id], [date], [level], [score], " +
                                          $"[mean_deviation_x], [mean_deviation_y], [min_deviation_x], " +
                                          $"[min_deviation_y], [max_deviation_x], [max_deviation_y])" +
                                          $"VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", scores.UserId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", scores.GameId));
                    cmd.Parameters.Add(
                        new SQLiteParameter("@p3", scores.DateCompletion.ToString(CultureInfo.InvariantCulture))
                    );
                    cmd.Parameters.Add(new SQLiteParameter("@p4", scores.Level));
                    cmd.Parameters.Add(new SQLiteParameter("@p5", scores.Score));
                    cmd.Parameters.Add(new SQLiteParameter("@p6", scores.MeanDeviationsX));
                    cmd.Parameters.Add(new SQLiteParameter("@p7", scores.MeanDeviationsY));
                    cmd.Parameters.Add(new SQLiteParameter("@p8", scores.MinDeviationsX));
                    cmd.Parameters.Add(new SQLiteParameter("@p9", scores.MinDeviationsY));
                    cmd.Parameters.Add(new SQLiteParameter("@p10", scores.MaxDeviationsX));
                    cmd.Parameters.Add(new SQLiteParameter("@p11", scores.MaxDeviationsY));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void UpdateScores(Scores scores)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source = ..\\..\\..\\DB\\medeye.db"))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql =
                        $"UPDATE [Scores] SET [level] = @p4, [score] = @p5, [mean_deviation_x] = @p6, " +
                        $"[mean_deviation_y] = @p7, [min_deviation_x] = @p8, [min_deviation_y] = @p9, " +
                        $"[max_deviation_x] = @p10, [max_deviation_y] = @p11 " +
                        $"WHERE [user_id] = @p1 AND [game_id] = @p2 AND [date] = @p3";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", scores.UserId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", scores.GameId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", scores.DateCompletion));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", scores.Level));
                    cmd.Parameters.Add(new SQLiteParameter("@p5", scores.Score));
                    cmd.Parameters.Add(new SQLiteParameter("@p6", scores.MeanDeviationsX));
                    cmd.Parameters.Add(new SQLiteParameter("@p7", scores.MeanDeviationsY));
                    cmd.Parameters.Add(new SQLiteParameter("@p8", scores.MinDeviationsX));
                    cmd.Parameters.Add(new SQLiteParameter("@p9", scores.MinDeviationsY));
                    cmd.Parameters.Add(new SQLiteParameter("@p10", scores.MaxDeviationsX));
                    cmd.Parameters.Add(new SQLiteParameter("@p11", scores.MaxDeviationsY));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static void DeleteScores(Scores scores)
        {
            using (SQLiteConnection conn = new SQLiteConnection("data source = ..\\..\\..\\DB\\medeye.db"))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSql = $"DELETE FROM [Scores] " +
                                          $"WHERE [user_id] = @p1 AND [game_id] = @p2 AND [date] = @p3";
                    cmd.CommandText = strSql;
                    cmd.Connection = conn;
                    cmd.Parameters.Add(new SQLiteParameter("@p1", scores.UserId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", scores.GameId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", scores.DateCompletion));
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
        }

        public static List<Scores> GetScores(int userId, int gameId = -1)
        {
            var scoresList = new List<Scores>();
            using (SQLiteConnection conn = new SQLiteConnection("data source = ..\\..\\..\\DB\\medeye.db"))
            {
                using (SQLiteCommand cmd = new SQLiteCommand())
                {
                    const string strSqlWithGameId = $"SELECT [user_id], [game_id], [date], [level], [score], " +
                                                    $"[mean_deviation_x], [mean_deviation_y], [min_deviation_x], " +
                                                    $"[min_deviation_y], [max_deviation_x], [max_deviation_y] " +
                                                    $"FROM [Scores] WHERE [user_id] = @p1 AND [game_id] = @p2";
                    const string strSqlWithoutGameId = $"SELECT [user_id], [game_id], [date], [level], [score], " +
                                                       $"[mean_deviation_x], [mean_deviation_y], [min_deviation_x], " +
                                                       $"[min_deviation_y], [max_deviation_x], [max_deviation_y] " +
                                                       $"FROM [Scores] WHERE [user_id] = @p1";
                    cmd.Connection = conn;
                    if (gameId == -1)
                    {
                        cmd.CommandText = strSqlWithoutGameId;
                        cmd.Parameters.Add(new SQLiteParameter("@p1", userId));
                    }
                    else
                    {
                        cmd.CommandText = strSqlWithGameId;
                        cmd.Parameters.Add(new SQLiteParameter("@p1", userId));
                        cmd.Parameters.Add(new SQLiteParameter("@p1", gameId));
                    }

                    conn.Open();

                    var sqliteDataReader = cmd.ExecuteReader();
                    while (sqliteDataReader.Read())
                    {
                        var scores = new Scores
                        {
                            UserId = sqliteDataReader.GetInt32("[user_id]"),
                            GameId = sqliteDataReader.GetInt32("[game_id]"),
                            DateCompletion = sqliteDataReader.GetDateTime("[date]"),
                            Level = sqliteDataReader.GetInt32("[level]"),
                            Score = sqliteDataReader.GetDouble("[score]"),
                            MeanDeviationsX = sqliteDataReader.GetDouble("[mean_deviation_x]"),
                            MeanDeviationsY = sqliteDataReader.GetDouble("[mean_deviation_y]"),
                            MinDeviationsX = sqliteDataReader.GetDouble("[min_deviation_x]"),
                            MinDeviationsY = sqliteDataReader.GetDouble("[min_deviation_y]"),
                            MaxDeviationsX = sqliteDataReader.GetDouble("[max_deviation_x]"),
                            MaxDeviationsY = sqliteDataReader.GetDouble("[max_deviation_y]")
                        };
                        scoresList.Add(scores);
                    }

                    conn.Close();
                }
            }

            return scoresList;
        }
    }
}