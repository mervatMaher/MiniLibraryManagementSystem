using MiniLibraryManagementSystem.ModelServices.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using MiniLibraryManagementSystem.Filters;

namespace MiniLibraryManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiVersion("3.0")]
    [Authorize]
    [ValidateModelFilter]
    [ServiceFilter(typeof(JwtExceptionFilter))]


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
