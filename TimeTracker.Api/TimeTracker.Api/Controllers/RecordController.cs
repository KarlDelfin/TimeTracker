using Microsoft.AspNetCore.Mvc;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Logics;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class RecordController : Controller
    {
        private readonly RecordLogic _logic;
        public RecordController(RecordLogic logic)
        {
            _logic = logic;
        }

        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetRecordByUserId(Guid userId, [FromQuery] PaginationRequest paginationRequest, [FromQuery] string? search = "")
        {
            try
            {
                var data = await _logic.GetRecordByUserId(userId, search);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddRecord(List<RecordDTO_POST> dto)
        {
            try
            {
                bool ok = await _logic.AddRecord(dto);
                if (ok)
                {
                    return Ok();
                }
                return BadRequest("Failed to add records");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpDelete("{recordId}")]
        public async Task<IActionResult> DeleteRecord(Guid recordId)
        {
            try
            {
                bool ok = await _logic.DeleteRecord(recordId);
                if (ok)
                {
                    return Ok();
                }
                return BadRequest("Failed to delete record");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
