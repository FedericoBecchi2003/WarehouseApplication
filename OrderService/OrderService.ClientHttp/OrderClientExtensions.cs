using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace OrderService.ClientHttp;

public static class OrderClientExtensions
{
    public static IServiceCollection AddOrderClient(this IServiceCollection services, string baseUrl)
    {
        // Registra IOrderClient nel sistema di Dependency Injection di .NET
        services.AddRefitClient<IOrderClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        return services;
    }
}