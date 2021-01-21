using System;
using System.Collections.Generic;
using TenmoClient.Data;
using System.Web;
using System.Net;
using RestSharp;
using System.IO;
using System.Net.Http;
using System.Linq;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly TransferService transferService = new TransferService();

        static void Main(string[] args)
        {
            Run();
        }
        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        API_User user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }
        
        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your all transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Approve or Reject Pending transfer");
                Console.WriteLine("7. Log in as different user");
                Console.WriteLine("8. View transfer detail");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    decimal balance = authService.GetBalance(userId);
                    Console.WriteLine("Your current account balance is: $" + balance);
                }
                else if (menuSelection == 2)
                {
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    List<API_Transfer_Detail> transferList = transferService.GetTranferList(userId);
                    if (transferList != null)
                    {
                        Console.WriteLine("ID\tFrom/To\tAmount");
                        foreach (var transfer in transferList)
                        {
                            if (transfer.account_From != userId)
                            {
                                Console.WriteLine($"{transfer.transfer_Id}\tFrom:  {transfer.account_From_Username}\t{transfer.amount.ToString("c2")}");
                            }
                            else
                            {
                                Console.WriteLine($"{transfer.transfer_Id}\tTo:  {transfer.account_To_Username}\t{transfer.amount.ToString("c2")}");
                            }
                        }
                    }
                }
                else if (menuSelection == 3)
                {
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    List<API_Transfer_Detail> transferList = transferService.GetTranferList(userId, 1);
                    if (transferList != null)
                    {
                        Console.WriteLine("ID\tFrom/To\tAmount");
                        foreach (var transfer in transferList)
                        {
                            if (transfer.account_From != userId)
                            {
                                Console.WriteLine($"{transfer.transfer_Id}\tFrom:  {transfer.account_From_Username}\t{transfer.amount.ToString("c2")}");
                            }
                            else
                            {
                                Console.WriteLine($"{transfer.transfer_Id}\tTo:  {transfer.account_To_Username}\t{transfer.amount.ToString("c2")}");
                            }
                        }
                    }
                }
                else if (menuSelection == 4)
                {
                    List<User> userList = ShowUserList();  //Gets list of users that money can be sent to                    
                    int userIdTo = GetUserId(userList, "Choose the Id of who to send money to:  ");  //Get the user to send TE Bucks to and catches errors for invalid entrys
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    decimal balance = authService.GetBalance(userId);
                    decimal amount = GetAmountToSend(balance);
                    API_Transfer transferDTO = new API_Transfer();
                    transferDTO.From_User_Id = userId;
                    transferDTO.To_User_Id = userIdTo;
                    transferDTO.Amount = amount;
                    transferDTO.Transfer_Status = 2;
                    transferDTO.Transfer_Type = 2;
                    bool result = transferService.CreateTransfer(transferDTO, "Funds successfully sent!");
                }
                else if (menuSelection == 5)
                {
                    List<User> userList = ShowUserList();  //Gets list of users that money can be sent to
                    int userIdFrom = GetUserId(userList, "Enter ID of user you are requesting from (0 to cancel):  ");  //Get the user to send TE Bucks to and catches errors for invalid entries
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    decimal amount = GetAmountToRequest();
                    API_Transfer transferDTO = new API_Transfer();
                    transferDTO.From_User_Id = userIdFrom;
                    transferDTO.To_User_Id = userId;
                    transferDTO.Amount = amount;
                    transferDTO.Transfer_Status = 1;
                    transferDTO.Transfer_Type = 1;
                    bool result = transferService.CreateTransfer(transferDTO, "Request was sent successfully.");
                }
                else if (menuSelection == 7)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new API_User()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else if (menuSelection == 6)
                {
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    List<API_Transfer_Detail> transferList = transferService.GetTranferList(userId, 1);  //get all pending requests
                    transferList = transferList.Where(t => t.account_From == userId).ToList();
                    int transferID = GetTransferId(transferList);
                    if (transferID != 0)
                    {
                        API_Transfer_Detail transferDetail = transferList.Where(t => t.transfer_Id == transferID).FirstOrDefault();
                        bool approve = GetApproval();
                        if (approve)
                        {
                            transferDetail.transfer_Status_Id = 2;  //approved
                        }
                        else
                        {
                            transferDetail.transfer_Status_Id = 3;  //rejected
                        }
                        transferService.UpdateTransfer(transferDetail);
                    }
                }
                else if (menuSelection == 8)
                {
                    //view transfer detail                    
                    int userId = UserService.GetUserId();  //GetUserId is contained in the UserService.cs
                    List<API_Transfer_Detail> transferList = transferService.GetTranferList(userId);
                    int transferID = GetTransferId(transferList);

                    API_Transfer_Detail transferDetail = transferList.Where(t => t.transfer_Id == transferID).FirstOrDefault();
                    Console.WriteLine("Transfer Detail");
                    Console.WriteLine($"ID: {transferID}");
                    if (transferDetail.account_From == userId)
                    {
                        Console.WriteLine($"From: Me {transferDetail.account_From_Username}");
                        Console.WriteLine($"To: {transferDetail.account_To_Username}");
                    }
                    else
                    {
                        Console.WriteLine($"From: {transferDetail.account_From_Username}");
                        Console.WriteLine($"To: Me {transferDetail.account_To_Username}");
                    }
                    Console.WriteLine($"Type: {transferDetail.transfer_Type_Desc}");
                    Console.WriteLine($"Status: {transferDetail.transfer_Status_Desc}");
                    Console.WriteLine($"Amount: {transferDetail.amount.ToString("c2")}");
                }
                else  //option 0
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
        private static bool GetApproval()
        {
            Console.WriteLine("Approve (Y/N)?");
            string resultStr = Console.ReadLine();
            if (resultStr.ToUpper() == "Y")
            {
                return true;
            }
            else if (resultStr.ToUpper() == "N")
            {
                return false;
            }
            else
            {
                Console.WriteLine("Invalid Entry");
                return GetApproval();
            }               
        }

        private static int GetTransferId(List<API_Transfer_Detail> transferList)
        {
            Console.WriteLine("Enter transfer ID or Enter 0 to Exit");
            string transferIdStr = Console.ReadLine();
            if (int.TryParse(transferIdStr, out int transferId))
            {
                if (transferId == 0)
                    return transferId;
                if (transferList.Any(t => t.transfer_Id == transferId))
                {
                    return transferId;
                }
                else
                {
                    Console.WriteLine("Transfer not found");
                    return GetTransferId(transferList);
                }
            }
            else
            {
                Console.WriteLine("Invalid Entry");
                return GetTransferId(transferList);
            }
        }

        private static decimal GetAmountToRequest()
        {
            Console.WriteLine("Enter amount to request");
            string amountTorequestStr = Console.ReadLine();
            if (decimal.TryParse(amountTorequestStr, out decimal amountToRequest))
            {
                if (amountToRequest > 0)
                {
                    return amountToRequest;
                }
                else
                {
                    Console.WriteLine("Can not request a negative or zero TE Bucks");
                    return GetAmountToRequest();
                }
            }
            else
            {
                Console.WriteLine("Invalid Entry");
                return GetAmountToRequest();
            }

        }

        private static decimal GetAmountToSend(decimal balance)
        {
            Console.WriteLine("Enter amount to send");
            string amountToSendStr = Console.ReadLine();
            if (decimal.TryParse(amountToSendStr, out decimal amountToSend))
            {
                if (amountToSend >= balance || amountToSend <= 0)
                {
                    Console.WriteLine("Invalid Entry");
                    return GetAmountToSend(balance);                    
                }
                else
                {
                    return amountToSend;
                }
            }
            else
            {
                Console.WriteLine("Invalid Entry");
                return GetAmountToSend(balance);
            }
        }

        private static int GetUserId(List<User> userList, string prompt)
        {
            Console.WriteLine(prompt);
            string userIdToStr = Console.ReadLine();
            if (int.TryParse(userIdToStr, out int userIdTo))
            {
                if (userList.Any(u => u.UserId == userIdTo))
                {
                    return userIdTo;
                }
                else
                {
                    Console.WriteLine("UserId not found");
                    return GetUserId(userList, prompt);
                }
            }
            else
            {
                Console.WriteLine("Please enter an interger for the UserId");
                return GetUserId(userList, prompt);
            }
        }

        private static List<User> ShowUserList()
        {
            //show a list of users
            List<User> userList = authService.ListUsers();
            int currentUserId = UserService.GetUserId();
            userList = userList.Where(u => u.UserId != currentUserId).ToList();
            Console.WriteLine("ID     NAME");

            foreach (User u in userList)
            {
               Console.WriteLine(u.UserId + "     " + u.Username);
            }
            return userList;
        }
    }
}
