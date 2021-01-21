using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Data;


namespace TenmoClient
{
    public class AuthService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        //login endpoints
        public bool Register(LoginUser registerUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login/register");
            request.AddJsonBody(registerUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return false;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return false;
            }
            else
            {
                return true;
            }
        }

        public API_User Login(LoginUser loginUser)
        {
            RestRequest request = new RestRequest(API_BASE_URL + "login");
            request.AddJsonBody(loginUser);
            IRestResponse<API_User> response = client.Post<API_User>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                if (!string.IsNullOrWhiteSpace(response.Data.Message))
                {
                    Console.WriteLine("An error message was received: " + response.Data.Message);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return null;
            }
            else
            {
                client.Authenticator = new JwtAuthenticator(response.Data.Token);
                return response.Data;
            }
        }

        public decimal GetBalance(int userId)
        {
            RestRequest requestBalance = new RestRequest(API_BASE_URL + "user/balance");
            IRestResponse<decimal> response = client.Get<decimal>(requestBalance);    

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                //response not received
                Console.WriteLine("An error occurred communicating with the server.");
                return 0;
            }
            else if (!response.IsSuccessful)
            {
                //response non-2xx
                Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                return 0;
            }
            else
            {
                //success
                return response.Data;
            }
        }

        public List<User> ListUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "user/users");
            IRestResponse<List<User>> response = client.Get<List<User>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                //response not received
                ProcessErrorResponse(response);
                return null;
            }
            else
            {
                //success
                return response.Data;
            }

        }
        private void ProcessErrorResponse(IRestResponse response)
        {
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("Error occurred - unable to reach server.");
            }
            else if (!response.IsSuccessful)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Console.WriteLine("Authorization Require. Please Log In");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    Console.WriteLine("You do not have permissions to do that");
                }
                else
                {
                    Console.WriteLine("Error occurred - received non-success response: " + (int)response.StatusCode);
                }
            }
        }

    }
}
