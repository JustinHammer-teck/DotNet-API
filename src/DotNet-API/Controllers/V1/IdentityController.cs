using System;
using System.Linq;
using System.Threading.Tasks;
using DotNet_API.Application.Repositories;
using DotNet_API.Contracts.V1;
using DotNet_API.Contracts.V1.Requests;
using DotNet_API.Contracts.V1.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_API.Controllers.V1
{
    public class IdentityController : Controller
    {
        private readonly IIdentityRepository _identityRepository;

        public IdentityController(IIdentityRepository identityRepository)
        {
            _identityRepository = identityRepository;
        }


        [HttpPost(ApiRoutes.Indentity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailResponse()
                {
                    Errors = ModelState.Values.SelectMany(x => x.Errors.Select(c => c.ErrorMessage))
                });
            }

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

        [HttpPost(ApiRoutes.Indentity.Login)]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            var authResponse = await _identityRepository.LoginAsync(request.Email, request.Password);

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