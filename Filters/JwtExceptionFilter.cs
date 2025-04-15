using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace MiniLibraryManagementSystem.Filters
{
    public class JwtExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "Token expired"
                });
            }
            
            else if (context.Exception is SecurityTokenNotYetValidException)
            {
                context.Result = new UnauthorizedObjectResult(new
                {
                    Message = "Token not yet valid"
                });
            }
            
        }
    }
}
