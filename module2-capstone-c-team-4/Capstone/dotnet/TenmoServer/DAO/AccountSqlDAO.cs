using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSqlDAO : IAccountDAO
    {
        private string connectionString;
        public AccountSqlDAO(string databaseConnectionString)
        {
            connectionString = databaseConnectionString;
        }
        public Account GetAcctById(int userId)
        {
            Account acct = new Account();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT account_id, user_id, balance FROM accounts WHERE user_id = @user_id", conn);
                    cmd.Parameters.AddWithValue("@user_id", userId);
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        acct.Account_Id = Convert.ToInt32(reader["account_id"]);
                        acct.User_Id = Convert.ToInt32(reader["user_id"]);
                        acct.Balance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return acct;
        }
    }
}
