using API.Data;
using API.Helpers;
using API.Services;
using API.SignalR;
using AutoMapper;
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
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IPhotoService, PhotoService>();
            serviceCollection.AddScoped<ILikesRepository, LikesRepository>();
            serviceCollection.AddScoped<IMessageRepository, MessageRepository>();
            serviceCollection.AddScoped<LogUserActivity>();
            serviceCollection.AddSingleton<PresenceTracker>();

            serviceCollection.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            serviceCollection.Configure<CloudinarySettings>(configuration.GetSection("CloudinarySettings"));
            return serviceCollection;
        }
    }
}
