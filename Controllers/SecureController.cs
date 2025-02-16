using MiniLibraryManagementSystem.ModelServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]

    public class SecureController : ControllerBase
    {
        private readonly IAccountServices _accountServices; 
        public SecureController(IAccountServices accountServices)
        {
            _accountServices = accountServices;
        }
        [HttpGet("GetSecureData")]
        
        public  IActionResult GetSecureData()
        {
            return Ok("This is a secure data");
        }
    }
}
