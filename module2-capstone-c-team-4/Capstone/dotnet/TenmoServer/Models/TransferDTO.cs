using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class TransferDTO
    {
        public int From_User_Id { get; set; }
        public int To_User_Id { get; set; }
        public decimal Amount { get; set; }
        public int Transfer_Type { get; set; }
        public int Transfer_Status { get; set; }
    }
}
