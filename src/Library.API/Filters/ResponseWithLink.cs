using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Library.API.Filters
{
    public class ResponseWithLinkAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable => false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var provider = serviceProvider.GetService<LinkFilter>();

            if (provider == null)
            {
                throw new ArgumentException("Service LinkFilter cannot be null");
            }

            return provider;
        }       
    }

   

    
}
