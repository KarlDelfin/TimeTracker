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


        [HttpGet]
        public async Task<IActionResult> GetRecord([FromQuery] PaginationRequest paginationRequest, [FromQuery] string? search = "")
        {
            try
            {
                var data = await _logic.GetRecord(search);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpGet("{recordId}")]
        public async Task<IActionResult> GetRecordByRecordId(Guid recordId)
        {
            try
            {
                var data = await _logic.GetRecordByRecordId(recordId);
                return Ok(data);
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
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
                    return Ok("success");
                }
                return Json("Failed to add records");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }

        [HttpPut("{recordId}")]
        public async Task<IActionResult> ReassignRecord(Guid recordId, RecordDTO_REASSIGN dto)
        {
            try
            {
                bool ok = await _logic.ReassignRecord(recordId, dto);
                if (ok)
                {
                    return Ok("success");
                }
                return Json("Failed to reassign record");
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
                    return Ok("success");
                }
                return Json("Failed to delete record");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
