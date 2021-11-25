using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNet_API.Domain.Entities;
using DotNet_API.Infrastructure.Data;
using DotNet_API.Application.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DotNet_API.Infrastructure.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PostRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Post>> GetPostsAsync()
        {
            return await _dbContext.Posts.ToListAsync();
        }

        public async Task<Post> GetPostByIdAsync(Guid postId)
        {
            return await _dbContext.Posts.SingleOrDefaultAsync(x => x.Id == postId);
        }

        public async Task<bool> CreatePostAsync(Post post)
        {
            await _dbContext.Posts.AddAsync(post);

            var created = await _dbContext.SaveChangesAsync();

            return created > 0;
        }

        public async Task<bool> UpdatePostAsync(Post postToUpdate)
        {
            _dbContext.Posts.Update(postToUpdate);

            var updated = await _dbContext.SaveChangesAsync();

            return updated > 0;
        }

        public async Task<bool> DeletePostAsync(Guid postId)
        {
            var post = await GetPostByIdAsync(postId);

            if (post == null)
            {
                return false;
            }
            
            _dbContext.Posts.Remove(post);

            var deleted = await _dbContext.SaveChangesAsync();

            return deleted > 0;
        }

        public async Task<bool> UserOwnsPostAsync(Guid postId, string userId)
        {
            var post = await _dbContext.Posts.AsNoTracking().SingleOrDefaultAsync(x => x.Id == postId);

            if (post == null)
            {
                return false;
            }

            if (post.UserId != userId)
            {
                return false;
            }

            return true;
        }
    }
}