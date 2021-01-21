using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface ITransferDAO
    {
        Transfer GetTransferById(int transfer_id);
        bool CreateTransfer(Account userIdFrom, Account userIdTo, TransferDTO transfer);
        List<Transfer> GetListOfTransfers(int userID, int status = 0);
        bool UpdateTransfer(Account fromAcct, Account toAcct, Transfer transfer);
    }
}
