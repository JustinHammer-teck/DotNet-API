using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using DotNet_API.Contracts.V1;
using DotNet_API.Contracts.V1.Requests;
using DotNet_API.Contracts.V1.Responses;
using DotNet_API.Domain.Entities;
using DotNet_API.Extensions;
using DotNet_API.Infrastructure.Repositories;
using DotNet_API.Application.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet_API.Controllers.V1
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;

        public PostController(IPostRepository postRepository)
        {
            _postRepository = postRepository;
        }

        [HttpGet(ApiRoutes.Posts.GetAll)]
        public async Task<IActionResult> GetAll()
        {
            var post = await _postRepository.GetPostsAsync();

            return Ok(post);
        }

        [HttpGet(ApiRoutes.Posts.Get)]
        public async Task<IActionResult> Get([FromRoute] Guid postId)
        {
            var post = await _postRepository.GetPostByIdAsync(postId);

            if (post == null) return NotFound();

            return Ok(post);
        }

        [HttpPut(ApiRoutes.Posts.Put)]
        public async Task<IActionResult> Put([FromRoute] Guid postId, [FromBody] UpdatePostRepest request)
        {
            var userOwnsPost = await _postRepository.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do not own this Post"});
            }

            var post = await _postRepository.GetPostByIdAsync(postId);
            post.Title = request.Title;

            var updated = await _postRepository.UpdatePostAsync(post);
            if (updated) return Ok();
            return NotFound();
        }

        [HttpDelete(ApiRoutes.Posts.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid postId)
        {
            var userOwnsPost = await _postRepository.UserOwnsPostAsync(postId, HttpContext.GetUserId());

            if (!userOwnsPost)
            {
                return BadRequest(new {error = "You do not own this Post"});
            }
            
            var deleted = await _postRepository.DeletePostAsync(postId);
            if (deleted) return NoContent();
            return NotFound();
        }

        [HttpPost(ApiRoutes.Posts.Post)]
        public async Task<IActionResult> Post([FromBody] CreatePostRequest postRequest)
        {
            var post = new Post()
            {
                Title = postRequest.Title,
                UserId = HttpContext.GetUserId()
            };

            await _postRepository.CreatePostAsync(post);

            var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}";

            var locationUri = baseUrl + "/" + ApiRoutes.Posts.Get.Replace("{postId}", post.Id.ToString());

            var response = new PostResponse {Id = post.Id};

            return Created(locationUri, response);
        }
    }
}