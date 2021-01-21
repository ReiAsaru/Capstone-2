using System;
using System.Collections.Generic;
using System.Text;

namespace TenmoClient.Data
{
    public class API_Transfer_Detail
    {
        public int transfer_Id { get; set; }
        public int transfer_Type_Id { get; set; }
        public string transfer_Type_Desc { get; set; }
        public int transfer_Status_Id { get; set; }
        public string transfer_Status_Desc { get; set; }
        public int account_From { get; set; }
        public string account_From_Username { get; set; }
        public int account_To { get; set; }
        public string account_To_Username { get; set; }
        public decimal amount { get; set; }

    }
}
