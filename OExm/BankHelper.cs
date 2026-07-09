using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace OExm
{
    // ================================================================
    // BankHelper
    // ----------------------------------------------------------------
    // One place for everything to do with dynamic bank tables.
    // Every page that touches a bank table calls these methods.
    // ================================================================

    public static class BankHelper
    {
        // Converts a bank name like "Operating System" into a safe SQL
        // table name like "Operating_System".
        // Removes anything that isn't a letter, digit or underscore.
        public static string ToTableName(string bankName)
        {
            if (string.IsNullOrWhiteSpace(bankName)) return "";

            string safe = bankName.Trim();
            safe = safe.Replace(" ", "_");
            safe = Regex.Replace(safe, @"[^a-zA-Z0-9_]", "");

            if (safe.Length == 0) return "";

            // Table names can't start with a digit.
            if (char.IsDigit(safe[0])) safe = "Bank_" + safe;

            return safe;
        }

        // Returns true if a bank table with this name already exists.
        public static bool TableExists(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return false;

            object result = DatabaseHelper.ExecuteScalar(
                "SELECT COUNT(*) FROM sys.tables WHERE name=@t",
                new SqlParameter[] { new SqlParameter("@t", tableName) });

            return Convert.ToInt32(result) > 0;
        }

        // Creates the bank table in SQL Server.
        // Safe: does nothing if it already exists.
        public static void CreateBankTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return;

            string sql = @"
                IF NOT EXISTS (SELECT * FROM sys.tables WHERE name='" + tableName + @"')
                CREATE TABLE [" + tableName + @"]
                (
                    SlNo     INT IDENTITY(1,1) PRIMARY KEY,
                    Question NVARCHAR(MAX)  NOT NULL,
                    M1       NVARCHAR(500)  NOT NULL,
                    M2       NVARCHAR(500)  NOT NULL,
                    M3       NVARCHAR(500)  NOT NULL,
                    M4       NVARCHAR(500)  NOT NULL,
                    Ans      NVARCHAR(500)  NOT NULL
                )";

            DatabaseHelper.ExecuteNonQuery(sql);
        }

        // Drops a bank table. Used only by DatabaseManager for super-admin.
        public static void DropBankTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName)) return;

            DatabaseHelper.ExecuteNonQuery(
                "IF EXISTS (SELECT * FROM sys.tables WHERE name='" + tableName + "') DROP TABLE [" + tableName + "]");
        }

        // Returns all rows from a bank table as a DataTable.
        public static DataTable GetAllRows(string tableName)
        {
            if (!TableExists(tableName)) return new DataTable();

            return DatabaseHelper.ExecuteQuery("SELECT * FROM [" + tableName + "] ORDER BY SlNo");
        }

        // Returns N random questions from a bank table.
        // Each row is returned with an extra "CorrectOption" column
        // (A/B/C/D) so ExamPortal can work with it exactly like before.
        public static DataTable GetRandomQuestions(string tableName, int count)
        {
            if (!TableExists(tableName)) return new DataTable();

            // Pull random rows, then figure out which option letter matches Ans.
            DataTable raw = DatabaseHelper.ExecuteQuery(
                "SELECT TOP (@n) * FROM [" + tableName + "] ORDER BY NEWID()",
                new SqlParameter[] { new SqlParameter("@n", count) });

            // Add a CorrectOption column (A/B/C/D) so ExamPortal works identically.
            raw.Columns.Add("CorrectOption", typeof(string));

            foreach (DataRow row in raw.Rows)
            {
                row["CorrectOption"] = DetermineCorrectOption(
                    row["Ans"].ToString(), row["M1"].ToString(),
                    row["M2"].ToString(), row["M3"].ToString(), row["M4"].ToString());
            }

            return raw;
        }

        // Single source of truth for "which option letter does Ans match".
        // Used anywhere a bank row needs to be scored, so ExamPortal and
        // Result always agree on the correct answer.
        public static string DetermineCorrectOption(string ans, string m1, string m2, string m3, string m4)
        {
            string a = (ans ?? "").Trim();
            if (a.Equals((m1 ?? "").Trim(), StringComparison.OrdinalIgnoreCase)) return "A";
            if (a.Equals((m2 ?? "").Trim(), StringComparison.OrdinalIgnoreCase)) return "B";
            if (a.Equals((m3 ?? "").Trim(), StringComparison.OrdinalIgnoreCase)) return "C";
            return "D";
        }

        // Returns a SlNo -> CorrectOption map for every row in the named bank
        // table. This is what scoring should use instead of joining to the
        // legacy Questions table, since the real answer key now lives only
        // in the dynamic bank tables.
        public static Dictionary<int, string> GetCorrectOptionMap(string tableName)
        {
            Dictionary<int, string> map = new Dictionary<int, string>();
            if (!TableExists(tableName)) return map;

            DataTable dt = DatabaseHelper.ExecuteQuery(
                "SELECT SlNo, M1, M2, M3, M4, Ans FROM [" + tableName + "]");

            foreach (DataRow row in dt.Rows)
            {
                int slNo = Convert.ToInt32(row["SlNo"]);
                map[slNo] = DetermineCorrectOption(
                    row["Ans"].ToString(), row["M1"].ToString(),
                    row["M2"].ToString(), row["M3"].ToString(), row["M4"].ToString());
            }

            return map;
        }

        // Given an exam's own StudentExamId, works out which bank table its
        // questions actually came from. Each Exam has exactly one
        // QuestionBankId, so there's no ambiguity here.
        public static string GetBankTableNameForAttempt(int studentExamId)
        {
            object bankName = DatabaseHelper.ExecuteScalar(
                @"SELECT qb.BankName
                  FROM StudentExams se
                  INNER JOIN Exams e ON se.ExamId = e.ExamId
                  LEFT JOIN QuestionBanks qb ON e.QuestionBankId = qb.BankId
                  WHERE se.StudentExamId = @se",
                new SqlParameter[] { new SqlParameter("@se", studentExamId) });

            return (bankName == null || bankName == DBNull.Value)
                ? ""
                : ToTableName(bankName.ToString());
        }

        // Returns a list of all bank table names currently in the database.
        // It cross-references sys.tables with QuestionBanks registry so we
        // only show tables that the admin created through the app.
        public static DataTable GetAllBanks()
        {
            return DatabaseHelper.ExecuteQuery(
                "SELECT BankId, BankName FROM QuestionBanks ORDER BY BankName");
        }

        // Inserts one question row into the named bank table.
        public static void InsertQuestion(string tableName, string question,
            string m1, string m2, string m3, string m4, string ans)
        {
            DatabaseHelper.ExecuteNonQuery(
                "INSERT INTO [" + tableName + "] (Question,M1,M2,M3,M4,Ans) VALUES (@q,@m1,@m2,@m3,@m4,@ans)",
                new SqlParameter[]
                {
                    new SqlParameter("@q",   question),
                    new SqlParameter("@m1",  m1),
                    new SqlParameter("@m2",  m2),
                    new SqlParameter("@m3",  m3),
                    new SqlParameter("@m4",  m4),
                    new SqlParameter("@ans", ans)
                });
        }

        // Deletes one row from a bank table by SlNo.
        public static void DeleteQuestion(string tableName, int slNo)
        {
            DatabaseHelper.ExecuteNonQuery(
                "DELETE FROM [" + tableName + "] WHERE SlNo=@s",
                new SqlParameter[] { new SqlParameter("@s", slNo) });
        }
    }
}
