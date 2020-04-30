using CoreyAndRick.Project.Website.Controllers;
using CoreyAndRick.Project.Website.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Analytics.Tracking;
using Sitecore.DependencyInjection;

namespace CoreyAndRick.Project.Website
{
    public class WebsiteConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<ContactController>();
            serviceCollection.AddSingleton<ContactRepository>();

            // Sitecore's ContactManager implementation isn't registered with the container
            serviceCollection.AddSingleton(serviceProvider =>
            {
                var factory = serviceProvider.GetRequiredService<BaseFactory>();
                return (ContactManager)factory.CreateObject("tracking/contactManager", true);
            });
        }
    }
}
