using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserDAO userDAO;
        private readonly IAccountDAO accountDAO;
        public UserController(IUserDAO _userDAO, IAccountDAO _accountDAO)
        {
            userDAO = _userDAO;
            accountDAO = _accountDAO;
        }

        private int? GetCurrentUserId()
        {
            string userId = User.FindFirst("sub")?.Value;
            if (string.IsNullOrWhiteSpace(userId)) return null;
            int.TryParse(userId, out int userIdInt);
            return userIdInt;
        }

        [HttpGet("balance")]
        public IActionResult GetBalance()
        {
            int? id = GetCurrentUserId();
            if (!id.HasValue)
            {
                return BadRequest();
            }
            Account account = accountDAO.GetAcctById(id.Value);
            return Ok(account.Balance);
        }

        [HttpGet("users")]
        public IActionResult ListUsers()
        {
            List<User> user = userDAO.GetUsers();
            if (user == null)
            {
                return StatusCode(500);  //checks to see list has values
            }

            foreach (User u in user)  //null out values that we don't need to return (password, salt and email)
            {
                u.PasswordHash = null;
                u.Salt = null;
                u.Email = null;                
            }
            return Ok(user);
        }
    }
}
