using Microsoft.Extensions.DependencyInjection;
using Refit;

namespace PaymentService.ClientHttp;

public static class PaymentClientExtensions
{
    public static IServiceCollection AddPaymentClient(this IServiceCollection services, string baseUrl)
    {
        services.AddRefitClient<IPaymentClient>()
            .ConfigureHttpClient(c => c.BaseAddress = new Uri(baseUrl));
        return services;
    }
}
