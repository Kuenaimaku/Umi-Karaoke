using KaraokeWebApi.Services;

namespace KaraokeWebApi.Core
{
    public static class DependencyInjectionSetup
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ISongsService, SongsService>();
            services.AddScoped<IQueueService, QueueService>();

            return services;
        }
    }
}
