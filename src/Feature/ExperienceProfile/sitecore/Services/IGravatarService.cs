using CoreyAndRick.Feature.ExperienceProfile.Models;

namespace CoreyAndRick.Feature.ExperienceProfile.Services
{
    public interface IGravatarService
    {
        GravatarResponse GetAvatar(string email, GravatarOptions options);
    }
}
