using Microsoft.AspNetCore.Mvc;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Logics;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ActivityController : Controller
    {
        private readonly ActivityLogic _logic;
        public ActivityController(ActivityLogic logic)
        {
            _logic = logic;
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetActivityByUserId(Guid userId, [FromQuery] PaginationRequest paginationRequest, [FromQuery] string? search = "", [FromQuery] bool isFiltered = false)
        {
            try
            {
                var data = await _logic.GetActivityByUserId(userId, search, isFiltered);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddActivity(ActivityDTO_POST dto)
        {
            try
            {
                bool ok = await _logic.AddActivity(dto);
                if (ok)
                {
                    return Ok("success");
                }
                return Json("Failed to add activity");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPut("{activityId}")]
        public async Task<IActionResult> UpdateActivity(Guid activityId, ActivityDTO_PUT dto)
        {
            try
            {
                bool ok = await _logic.UpdateActivity(activityId, dto);
                if (ok)
                {
                    return Ok("success");
                }
                return Json("Failed to add activity");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpDelete("{activityId}")]
        public async Task<IActionResult> DeleteActivity(Guid activityId)
        {
            try
            {
                bool ok = await _logic.DeleteActivity(activityId);
                if (ok)
                {
                    return Ok("success");
                }
                return Json("Failed to add activity");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
