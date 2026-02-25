using Microsoft.AspNetCore.Mvc;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Logics;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class UserAssignmentController : Controller
    {
        private readonly UserAssignmentLogic _logic;
        public UserAssignmentController(UserAssignmentLogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserAssignments([FromQuery]  string? search, [FromQuery] PaginationRequest paginationRequest)
        {
            try
            {
                var data = await _logic.GetUserAssignments(search);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserAssignment(List<UserAssignmentDTO_POST> dto)
        {
            try
            {
                bool isSuccess = await _logic.AssignUserAssignment(dto);
                if (isSuccess)
                {
                    return Ok("success");
                }
                return Json("Failed to assign user");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPut("{userAssignmentId}")]
        public async Task<IActionResult> UpdateUserAssignment(Guid userAssignmentId, UserAssignmentDTO_PUT dto)
        {
            try
            {
                bool isSuccess = await _logic.UpdateUserAssignment(userAssignmentId, dto);
                if (isSuccess)
                {
                    return Ok("success");
                }
                return Json("Failed to updated user");
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpDelete("{userAssignmentId}")]
        public async Task<IActionResult> DisableUserAssignment(Guid userAssignmentId)
        {
            try
            {
                bool isSuccess = await _logic.DisableUserAssignment(userAssignmentId);
                if (isSuccess)
                {
                    return Ok("User deleted successfully");
                }
                return BadRequest("Failed to disable user");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
