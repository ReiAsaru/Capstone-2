using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class API_Transfer
    {
        public int From_User_Id { get; set; }
        public int To_User_Id { get; set; }
        public decimal Amount { get; set; }
        public int Transfer_Type { get; set; }
        public int Transfer_Status { get; set; }

    }
}
