using MiniLibraryManagementSystem.ModelServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly IRolesServices _rolesServices;
        public RolesController(IRolesServices rolesServices)
        {
            _rolesServices = rolesServices;
        }
        //[Authorize("Admin")]
        [HttpPost("AddRoles")]
        public async Task<IActionResult> AddRoles()
        {             
            try
            {
                await _rolesServices.AddRolesAsync();
                return Ok("Roles added successfully");

            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
