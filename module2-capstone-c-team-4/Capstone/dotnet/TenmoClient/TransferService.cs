using RestSharp;
using System;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Data;
using TenmoServer.Models;

namespace TenmoClient
{
    public class TransferService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public bool CreateTransfer(API_Transfer transfer, string message)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transfer/transfer");
            request.AddJsonBody(transfer);
            IRestResponse<bool> response = client.Post<bool>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error message was received during transfer");
                return false;
            }
            else
            {
                Console.WriteLine(message);
                return true;
            }
        }
        public List<API_Transfer_Detail> GetTranferList(int userId, int status = 0)
        {
            RestRequest request = null;   
            if (status == 1)
            {
                request = new RestRequest(API_BASE_URL + "transfer/pending/" + userId);
            }
            else
            {
                request = new RestRequest(API_BASE_URL + "transfer/" + userId);
            }
            IRestResponse<List<API_Transfer_Detail>> response = client.Get<List<API_Transfer_Detail>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error message was received during transfer");
                return null;
            }
            else
            {
                return response.Data;
            }
        }

        internal List<API_Transfer_Detail> GeListPriorTransfers(int userId)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transfer/prior/" + userId);
            IRestResponse<List<API_Transfer_Detail>> response = client.Get<List<API_Transfer_Detail>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error message was received during transfer");
                return null;
            }
            else
            {
                return response.Data;
            }


        }

        internal bool UpdateTransfer(API_Transfer_Detail transferDetail)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "transfer/update");
            request.AddJsonBody(transferDetail);
            IRestResponse<bool> response = client.Post<bool>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                Console.WriteLine("An error message was received during transfer");
                return false;
            }
            else
            {
                Console.WriteLine("Transfer has been successfully updated");
                return true;
            }

        }
    }
}
