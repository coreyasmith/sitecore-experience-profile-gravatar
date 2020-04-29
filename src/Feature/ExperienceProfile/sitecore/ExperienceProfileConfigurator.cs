using CoreyAndRick.Feature.ExperienceProfile.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.Abstractions;
using Sitecore.Cintel.ContactService;
using Sitecore.DependencyInjection;

namespace CoreyAndRick.Feature.ExperienceProfile
{
    public class ExperienceProfileConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            // Sitecore's default IContactService implementation
            serviceCollection.AddSingleton<XdbContactService>();

            serviceCollection.AddSingleton<IGravatarService, GravatarService>();
            serviceCollection.AddSingleton<IContactService>(serviceProvider =>
            {
                var contactService = serviceProvider.GetRequiredService<XdbContactService>();
                var gravatarService = serviceProvider.GetRequiredService<IGravatarService>();
                var log = serviceProvider.GetRequiredService<BaseLog>();
                return new GravatarContactServiceDecorator(contactService, gravatarService, log);
            });
        }
    }
}
