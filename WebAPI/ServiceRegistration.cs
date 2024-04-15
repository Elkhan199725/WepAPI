using WebAPI.Business.Implementations;

namespace WebAPI;

public static class ServiceRegistration
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IGenreService, GenreService>();
    }
}
