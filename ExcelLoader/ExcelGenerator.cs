using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MedEye.ExcelLoader
{
    internal class ExcelGenerator
    {
        static public void GenerateExcelByUserId(int user_id)
        {
            DataSet ds = new DataSet("New_DataSet");
            DataTable dt = new DataTable("New_DataTable");

            ds.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;
            dt.Locale = System.Threading.Thread.CurrentThread.CurrentCulture;

            SQLiteConnection conn = new SQLiteConnection("data source = ..\\..\\..\\DB\\medeye.db");
            conn.Open();

            string sql = "SELECT [first_name] FROM [Users] WHERE id = @param1;";
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            cmd.Parameters.Add(new SQLiteParameter("@param1", user_id));
            SQLiteDataAdapter adptr = new SQLiteDataAdapter();

            adptr.SelectCommand = cmd;
            adptr.Fill(dt);
            conn.Close();

            ds.Tables.Add(dt);
            ExcelLibrary.DataSetHelper.CreateWorkbook("MyExcelFile.xls", ds);
        }
    }
}
