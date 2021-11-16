using System;
using System.Threading.Tasks;
using DotNet_API.Application.Repositories;
using DotNet_API.Contracts.V1;
using DotNet_API.Contracts.V1.Requests;
using DotNet_API.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_API.Controllers.V1
{
    public class IndentityController : Controller
    {
        private readonly IIdentityRepository _identityRepository;

        public IndentityController(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }


        [HttpPost(ApiRoutes.Indentity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var authResponse = await _identityRepository.RegisterAsync(request.Email, request.Password);

            if (!authResponse.Success)
            {
                return BadRequest(new AuthFailResponse()
                {
                    Errors = authResponse.Errors
                });
            }

            return Ok(new AuthSuccessResponse()
            {
                Token = authResponse.Token
            });
        }
    }
}