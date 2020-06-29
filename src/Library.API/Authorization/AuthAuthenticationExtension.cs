using Library.API.Authorization.Service;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Authorization
{
    public static class AuthAuthenticationExtension
    {
        public static int ServiceTST { get; private set; }

        public static IServiceCollection AddSTS(this IServiceCollection services, string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("The url parameter cannot be null or empty.");
            }

            services.AddHttpClient("sts", (sp, client) =>
            {
                client.BaseAddress = new Uri(url);
                client.DefaultRequestHeaders.Add("accept", "application/json");
            });

            services.AddTransient<IServiceSTS, ServiceSTS>();

            return services;
        }
    }
}
