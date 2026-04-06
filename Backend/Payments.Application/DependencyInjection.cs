using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Interfaces;
using Payments.Application.Services;

namespace Payments.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPaymentService, PaymentService>();
        return services;
    }
}
