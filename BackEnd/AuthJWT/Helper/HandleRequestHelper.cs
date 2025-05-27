using AuthJWT.Domain.Contracts;
using AuthJWT.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace AuthJWT.Helpers
{
    public static class HandleRequestHelper
    {
        public static async Task<IActionResult> HandleRequestAsync(Func<Task<IActionResult>> action)
        {
            try
            {
                return await action();
            }
            catch (ResourceNotFoundException ex)
            {
                return new NotFoundObjectResult(ApiResponse<string>.Failure(ex.Message));
            }
            catch (DatabaseBadRequestException ex)
            {
                return new BadRequestObjectResult(ApiResponse<string>.Failure(ex.Message));
            }
            catch (Exception ex)
            {
                return new ObjectResult(ApiResponse<string>.Failure("An unexpected error occurred: " + ex.Message))
                {
                    StatusCode = 500
                };
            }
        }
    }
}