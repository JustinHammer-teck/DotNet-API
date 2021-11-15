using DotNet_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNet_API.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Post> Posts { get; set; }
    }
}