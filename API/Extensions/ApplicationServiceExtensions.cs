using API.Data;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace API.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.AddDbContext<DataContext>(opts =>
            {
                opts.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            });

            serviceCollection.AddScoped<ITokenService, TokenService>();

            return serviceCollection;
        }
    }
}
