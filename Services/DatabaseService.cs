using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlowStudent.Services
{
    public class DatabaseService
    {
        private string connectionString = "Server=LAPTOP-352KBCVK\\SQLEXPRESS;Database=TaskFlow;Trusted_Connection=True;";

        public bool ValidateUser(string username, string password)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var cmd = new SqlCommand("SELECT COUNT(*) FROM Users WHERE Username = @u AND Password = @p", conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);
                int result = (int)cmd.ExecuteScalar();
                return result > 0;
            }
        }
    }
}
