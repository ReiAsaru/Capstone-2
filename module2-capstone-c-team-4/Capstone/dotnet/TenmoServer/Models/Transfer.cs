using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenmoServer.Models
{
    public class Transfer
    {
        public int Transfer_Id { get; set; }
        public int Transfer_Type_Id { get; set; }
        public string Transfer_Type_Desc { get; set; }
        public int Transfer_Status_Id { get; set; }
        public string Transfer_Status_Desc { get; set; }
        public int Account_From { get; set; }
        public string Account_From_Username { get; set; }
        public int Account_To { get; set; }
        public string Account_To_Username { get; set; }
        public decimal Amount { get; set; }
    }

}
