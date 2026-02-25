using Microsoft.AspNetCore.Mvc;
using TimeTracker.Api.Logics;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class RoleController : Controller
    {
        private readonly RoleLogic _logic;
        public RoleController(RoleLogic logic)
        {
            _logic = logic;
        }

        [HttpGet]
        public async Task<IActionResult> GetRole([FromQuery] PaginationRequest paginationRequest, [FromQuery] string? search = "")
        {
            try
            {
                var data = await _logic.GetRole(search);
                var pagination = PaginationLogic.PaginateData(data, paginationRequest);
                return Ok(pagination);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + ex.StackTrace);
            }
        }
    }
}
