using Microsoft.AspNetCore.Mvc;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Logics;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserLogic _logic;
        public UserController(UserLogic userLogic)
        {
            _logic = userLogic;
        }

        // GET USERS
        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery] PaginationRequest paginationRequest, [FromQuery] string? search = "")
        {
            try
            {
                var data = await _logic.GetUser(search);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        // LOGIN USER
        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(UserDTO_LOGIN dto)
        {
            try
            {
                var data = await _logic.LoginUser(dto);
                if (data != null)
                {
                    return Ok(data);
                }
                return Json("Invalid Email or Password");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        // REGISTER ACCOUNT
        [HttpPost]
        public async Task<IActionResult> RegisterUser([FromForm] UserDTO_POST dto)
        {
            try
            {
                string response = await _logic.RegisterUser(dto);
                if (response == "success")
                {
                    return Ok("success");
                }
                if (response == "exists")
                {
                    return Json("Account already exists!");
                }
                return Json("Failed to register account!");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        // UPDATE USER ACCOUNT
        [HttpPut("User/{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromForm] UserDTO_POST dto)
        {
            try
            {
                bool isSuccess = await _logic.UpdateUser(userId, dto);
                if (isSuccess)
                {
                    return Ok("success");
                }
                return Json("Failed to update account");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        // SEND CODE
        [HttpGet("Email/{email}")]
        public async Task<IActionResult> SendCode(string email)
        {
            try
            {
                var data = await _logic.SendCode(email);

                if (data != null)
                {
                    return Ok(data);
                }
                return Json("Failed to send verification code");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        // VERIFY CODE
        [HttpGet("Email/{email}/VerificationCode/{verificationCode}")]
        public async Task<IActionResult> VerifyCode(string email, int verificationCode)
        {
            try
            {
                var data = await _logic.VerifyCode(email, verificationCode);

                if (data != null)
                {
                    return Ok(data);
                }
                return Json("error");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        // RESET PASSWORD
        [HttpPut("Email/{email}/Password/{password}")]
        public async Task<IActionResult> ResetPassword(string email, string password)
        {
            try
            {
                bool isSuccess = await _logic.ResetPassword(email, password);

                if (isSuccess)
                {
                    return Ok("success");
                }
                return Json("Failed to reset password");

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
