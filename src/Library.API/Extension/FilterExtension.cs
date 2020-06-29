using Library.API.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace Library.API.Extension
{
    public static class FilterExtension
    {       
        public static IServiceCollection AddResponseWithLink(this IServiceCollection services)
        {
            services.AddScoped<LinkFilter>();

            return services;
        }        
    }
}
