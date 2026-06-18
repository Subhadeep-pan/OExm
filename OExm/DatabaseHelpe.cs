using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Security.Cryptography;
using System.Text;


namespace OExm
{
    public static class DatabaseHelper
    {
        private static string connStr =
            ConfigurationManager.ConnectionStrings["ExamDbConnection"].ConnectionString;

        static DatabaseHelper()
        {
            try
            {
                ExecuteNonQuery("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentExams') AND name = 'EndTime') ALTER TABLE StudentExams ADD EndTime DATETIME NULL;");
                ExecuteNonQuery("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentExams') AND name = 'Score') ALTER TABLE StudentExams ADD Score DECIMAL(5,2) NULL;");
                ExecuteNonQuery("IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('StudentResponses') AND name = 'Status') ALTER TABLE StudentResponses ADD Status VARCHAR(50) NULL;");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static DataTable ExecuteQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        return dt;
                    }
                }
            }
        }



        public static object ExecuteScalar(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();

                    return cmd.ExecuteScalar();
                }
            }
        }


        public static int ExecuteNonQuery(string query, SqlParameter[] parameters = null)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes =
                    Encoding.UTF8.GetBytes(password);

                byte[] hashBytes =
                    sha256.ComputeHash(inputBytes);

                StringBuilder sb =
                    new StringBuilder();

                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string LegacyHashMD5(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();

                foreach (byte b in hashBytes)
                    sb.Append(b.ToString("x2"));

                return sb.ToString();
            }
        }
    }
}