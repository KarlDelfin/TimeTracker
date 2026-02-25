using Microsoft.AspNetCore.Mvc;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Logics;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class StatusLogController : Controller
    {
        private readonly StatusLogLogic _logic;
        public StatusLogController(StatusLogLogic logic)
        {
            _logic = logic;
        }

        [HttpGet("Record/{recordId}")]
        public async Task<IActionResult> GetStatusLogByRecordId(Guid recordId, [FromQuery] PaginationRequest paginationRequest)
        {
            try
            {
                var data = await _logic.GetStatusLogByRecordId(recordId);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetStatusLogByRecordId(StatusLogDTO_POST dto)
        {
            try
            {
                bool isSuccess = await _logic.UpdateStatus(dto);
                if (isSuccess)
                {
                    return Ok("success");
                }
                return Json("Failed to update record status");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
