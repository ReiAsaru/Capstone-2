using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class TransferController : ControllerBase
    {
        private readonly ITransferDAO transferDAO;
        private readonly IAccountDAO accountDAO;
        public TransferController(ITransferDAO _transferDAO, IAccountDAO _accountDAO)
        {
            transferDAO = _transferDAO;
            accountDAO = _accountDAO;
        }
        [HttpPost("transfer")]
        
        public ActionResult<bool> CreateTransfer(TransferDTO transfer)
        {
            Account fromAcct = accountDAO.GetAcctById(transfer.From_User_Id);
            Account toAcct = accountDAO.GetAcctById(transfer.To_User_Id);
            bool result = transferDAO.CreateTransfer(fromAcct, toAcct, transfer);
            return Ok(result);
        }

        private int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }
        [HttpGet("{id}")]
        public IActionResult GetTransferList(int id)
        {
            List<Transfer> result = transferDAO.GetListOfTransfers(id);
            return Ok(result);
        }

        [HttpGet("pending/{id}")]
        public IActionResult GetPendingTransfers(int id)
        {
            List<Transfer> result = transferDAO.GetListOfTransfers(id, 1);
            return Ok(result);
        }
        [HttpPost("update")]
        public ActionResult<bool> UpdateTransfer(Transfer transfer)
        {
            Account fromAcct = accountDAO.GetAcctById(transfer.Account_From);
            Account toAcct = accountDAO.GetAcctById(transfer.Account_To);
            bool result = transferDAO.UpdateTransfer(fromAcct, toAcct, transfer);
            return Ok(result);
        }
    }
}
