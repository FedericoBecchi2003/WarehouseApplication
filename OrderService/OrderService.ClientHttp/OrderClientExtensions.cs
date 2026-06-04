using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace OrderService.ClientHttp;

public static class OrderClientExtensions
{
    public static IServiceCollection AddOrderClient(this IServiceCollection services, string baseUrl)
    {
        services.AddRefitClient<IOrderClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));

        return services;
    }
}
