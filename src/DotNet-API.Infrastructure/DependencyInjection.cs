using DotNet_API.Application.Repositories;
using DotNet_API.Infrastructure.Repositories;
using DotNet_API.Application.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNet_API.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddScoped<IIdentityRepository, IdentityRepository>();
            
            services.AddScoped<IPostRepository, PostRepository>();
            
            return services;
        }
    }
}