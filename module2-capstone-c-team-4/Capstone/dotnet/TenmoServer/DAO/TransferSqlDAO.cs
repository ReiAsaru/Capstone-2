using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class TransferSqlDAO : ITransferDAO
    {
        private string connectionString;
        private IAccountDAO Acct;
        public TransferSqlDAO(string databaseConnectionString)
        {
            connectionString = databaseConnectionString;
        }
        private Transfer GetTransferFromReader(SqlDataReader reader)
        {
            Transfer transfer = new Transfer()
            {
                Transfer_Id = Convert.ToInt32(reader["transfer_id"]),
                Transfer_Type_Id = Convert.ToInt32(reader["transfer_type_id"]),
                Transfer_Status_Id = Convert.ToInt32(reader["transfer_status_id"]),
                Account_From = Convert.ToInt32(reader["account_from"]),
                Account_To = Convert.ToInt32(reader["account_to"]),
                Amount = Convert.ToDecimal(reader["amount"]),
            };
            return transfer;
        }
        public bool CreateTransfer(Account fromAcct, Account toAcct, TransferDTO amt)
        {
            bool success = false;
            //set status for both to and from to pending (transfer_status_id = 1 sql)
            //add amt to userIdTo
            //subtract amt from userIdFrom
            //Transfer returnTransfer = new Transfer();
            if (fromAcct.Balance < amt.Amount)
            {
                return false; //insufficient funds
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlStatement =@"INSERT INTO [dbo].[transfers]
                                         ([transfer_type_id]
                                         ,[transfer_status_id]
                                         ,[account_from]
                                         ,[account_to]
                                         ,[amount])
                                         VALUES
                                         (@transfer_type, @transfer_status, @user_id, @user_id_to, @amt) ";
                    if (amt.Transfer_Status == 2)
                    {
                        sqlStatement += @"UPDATE accounts SET balance = balance - @amt WHERE user_id = @user_id;
                        UPDATE accounts SET balance = balance + @amt WHERE user_id = @user_id_to; ";
                    }
                    conn.Open();
                    // first UPDATE stmt does the subtraction of balance - amt of the sender
                    // second UPDATE stmt does the addition of balance + amt of the reciver

                    SqlCommand cmd = new SqlCommand(sqlStatement, conn);
                    cmd.Parameters.AddWithValue("@user_id", fromAcct.User_Id);
                    cmd.Parameters.AddWithValue("@user_id_to", toAcct.User_Id);
                    cmd.Parameters.AddWithValue("@amt", amt.Amount);
                    cmd.Parameters.AddWithValue("@transfer_type", amt.Transfer_Type);
                    cmd.Parameters.AddWithValue("@transfer_status", amt.Transfer_Status);

                    int numberOfRowsEffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsEffected > 0)
                    {
                        success = true;
                    }                       
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return success;
        }
        //if ok return status approved
        //else return status rejected

        public Transfer GetTransferById(int transfer_id)
        {
            Transfer returnTransfer = new Transfer();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT transfer_id, transfer_type_id, " +
                                                    "transfer_status_id,account_from, account_to, amount " +
                                                    "FROM transfers " +
                                                    "WHERE transfer_id = transfer_id, conn");
                    cmd.Parameters.AddWithValue("@transfer_id", transfer_id);

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        returnTransfer.Transfer_Id = Convert.ToInt32(reader["transfer_id"]);
                        returnTransfer.Transfer_Type_Id = Convert.ToInt32(reader["transfer_type_id"]);
                        returnTransfer.Transfer_Status_Id = Convert.ToInt32(reader["transfer_status_id"]);
                        returnTransfer.Account_From = Convert.ToInt32(reader["account_from"]);
                        returnTransfer.Account_To = Convert.ToInt32(reader["account_to"]);
                        returnTransfer.Amount = Convert.ToDecimal(reader["amount"]);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return returnTransfer;
        }
        public List<Transfer> GetListOfTransfers(int userID, int status = 0)
        {
            List<Transfer> transferList = new List<Transfer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlStatement = @"SELECT t.transfer_id, t.transfer_type_id,  
                                          tt.transfer_type_desc, 
                                          ts.transfer_status_desc,
                                          t.account_from,
	                                      t.account_to, 
	                                      t.amount,
	                                      uf.username AS Acct_From_Username,
	                                      ut.username AS Acct_To_Username
                                          from transfers t
                                          join transfer_types tt on t.transfer_type_id = tt.transfer_type_id
                                          join transfer_statuses ts on t.transfer_status_id = ts.transfer_status_id
                                          join users uf on t.account_from = uf.user_id
                                          join users ut on t.account_to = ut.user_id
                                          WHERE ((account_from = @user_id) OR
                                          (account_to = @user_id))";
                    if (status > 0)
                    {
                        sqlStatement += "AND (t.transfer_status_id = @status)";  // filters on the status if provided
                    }
                    conn.Open();
                    SqlCommand cmd = new SqlCommand(sqlStatement, conn);
                    cmd.Parameters.AddWithValue("@user_id", userID);
                    if (status > 0)
                    {
                        cmd.Parameters.AddWithValue("@status", status);
                    }
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Transfer transfer = new Transfer();

                        transfer.Transfer_Id = Convert.ToInt32(reader["transfer_id"]);
                        transfer.Transfer_Type_Id = Convert.ToInt32(reader["transfer_type_id"]);
                        transfer.Transfer_Type_Desc = Convert.ToString(reader["transfer_type_desc"]);
                        transfer.Transfer_Status_Desc = Convert.ToString(reader["transfer_status_desc"]);
                        transfer.Amount = Convert.ToDecimal(reader["amount"]);
                        transfer.Account_From_Username = Convert.ToString(reader["Acct_From_Username"]);
                        transfer.Account_To_Username = Convert.ToString(reader["Acct_To_Username"]);
                        transfer.Account_From = Convert.ToInt32(reader["account_from"]);
                        transfer.Account_To = Convert.ToInt32(reader["account_to"]);
                        transferList.Add(transfer);
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return transferList;
        }
        public bool UpdateTransfer(Account fromAcct, Account toAcct, Transfer transfer)
        {
            bool success = false;
            //set status for both to and from to pending (transfer_status_id = 1 sql)
            //add amt to userIdTo
            //subtract amt from userIdFrom
            //Transfer returnTransfer = new Transfer();
            if (fromAcct.Balance < transfer.Amount)
            {
                return false; //insufficient funds
            }
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string sqlStatement = @"UPDATE [dbo].[transfers]
                                          SET transfer_status_id = @transfer_status
                                          WHERE transfer_id = @transfer_id; ";
                    if (transfer.Transfer_Status_Id == 2)
                    {
                        sqlStatement += @"UPDATE accounts SET balance = balance - @amt WHERE user_id = @user_id;
                        UPDATE accounts SET balance = balance + @amt WHERE user_id = @user_id_to; ";
                    }
                    conn.Open();
                    // first UPDATE stmt does the subtraction of balance - amt of the sender
                    // second UPDATE stmt does the addition of balance + amt of the reciver

                    SqlCommand cmd = new SqlCommand(sqlStatement, conn);
                    cmd.Parameters.AddWithValue("@user_id", fromAcct.User_Id);
                    cmd.Parameters.AddWithValue("@user_id_to", toAcct.User_Id);
                    cmd.Parameters.AddWithValue("@transfer_status", transfer.Transfer_Status_Id);
                    cmd.Parameters.AddWithValue("@transfer_id", transfer.Transfer_Id);
                    cmd.Parameters.AddWithValue("@amt", transfer.Amount);
                    int numberOfRowsEffected = cmd.ExecuteNonQuery();
                    if (numberOfRowsEffected > 0)
                    {
                        success = true;
                    }
                }
            }
            catch (SqlException)
            {
                throw;
            }
            return success;
        }
    }
}

