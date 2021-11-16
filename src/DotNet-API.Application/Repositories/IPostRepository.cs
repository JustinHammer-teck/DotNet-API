using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNet_API.Domain.Entities;

namespace DotNet_API.Repositories
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post> GetPostByIdAsync(Guid postId);
        Task<bool> CreatePostAsync(Post post);
        Task<bool> UpdatePostAsync(Post postToUpdate);
        Task<bool> DeletePostAsync(Guid postId);
    }
}