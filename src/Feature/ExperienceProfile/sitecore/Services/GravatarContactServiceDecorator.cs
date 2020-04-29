using System;
using System.Collections.Generic;
using CoreyAndRick.Feature.ExperienceProfile.Models;
using Sitecore.Abstractions;
using Sitecore.Cintel.ContactService;
using Sitecore.Cintel.ContactService.Model;
using Sitecore.Cintel.Utility;
using Sitecore.Marketing.Core;
using Sitecore.XConnect;
using Sitecore.XConnect.Collection.Model;

namespace CoreyAndRick.Feature.ExperienceProfile.Services
{
    public class GravatarContactServiceDecorator : IContactService
    {
        protected GravatarOptions Options { get; set; }

        private readonly IContactService _contactService;
        private readonly IGravatarService _gravatarService;
        private readonly BaseLog _log;

        public GravatarContactServiceDecorator(
          IContactService contactService,
          IGravatarService gravatarService,
          BaseLog log)
        {
            _contactService = contactService ?? throw new ArgumentNullException(nameof(contactService));
            _gravatarService = gravatarService ?? throw new ArgumentNullException(nameof(gravatarService));
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IContact Get(Guid contactId)
        {
            return _contactService.Get(contactId);
        }

        public Facet GetFacet(Guid contactId, string facetName)
        {
            return _contactService.GetFacet(contactId, facetName);
        }

        public IDictionary<string, Facet> GetFacets(Guid contactId, params string[] facetNames)
        {
            return _contactService.GetFacets(contactId, facetNames);
        }

        public IMarketingImage GetImage(Guid contactId, int width, int height)
        {
            var contactEmail = GetContactEmail(contactId);
            if (string.IsNullOrWhiteSpace(contactEmail))
            {
                return _contactService.GetImage(contactId, width, height);
            }

            var contactAvatar = GetContactAvatar(contactId, width, height);
            if (contactAvatar != null)
            {
                return contactAvatar;
            }

            var gravatar = GetImageFromGravatar(contactEmail, width);
            if (gravatar == null)
            {
                return _contactService.GetImage(contactId, width, height);
            }
            return ResizeImage(gravatar.Content, width, height, gravatar.MimeType);
        }

        protected virtual string GetContactEmail(Guid contactId)
        {
            var emailAddressList = GetFacet(contactId, EmailAddressList.DefaultFacetKey) as EmailAddressList;
            var preferredEmail = emailAddressList?.PreferredEmail?.SmtpAddress;
            return preferredEmail;
        }

        protected virtual IMarketingImage GetContactAvatar(Guid contactId, int width, int height)
        {
            var contactAvatar = GetFacet(contactId, Avatar.DefaultFacetKey) as Avatar;
            var picture = contactAvatar?.Picture;
            if (picture == null || picture.Length <= 0) return null;

            var mimeType = contactAvatar.MimeType ?? string.Empty;
            return ResizeImage(picture, width, height, mimeType);
        }

        private static IMarketingImage ResizeImage(byte[] image, int width, int height, string mimeType)
        {
            if (width == 0 || height == 0)
            {
                return new MarketingImage(image, mimeType);
            }

            var resizedImage = ImageHelper.ResizeImage(image, width, height, mimeType);
            return new MarketingImage(resizedImage, mimeType);
        }

        private GravatarResponse GetImageFromGravatar(string email, int width)
        {
            try
            {
                var presetOptions = Options ?? GravatarOptions.Default;
                var gravatarOptions = new GravatarOptions
                {
                    DefaultImage = presetOptions.DefaultImage,
                    ForceDefault = presetOptions.ForceDefault,
                    Rating = presetOptions.Rating,
                    Size = presetOptions.Size
                };
                if (width > 0) gravatarOptions.Size = width;
                return _gravatarService.GetAvatar(email, gravatarOptions);
            }
            catch (Exception ex)
            {
                _log.Error("Error occurred getting contact's image from Gravatar.", ex, this);
                return null;
            }
        }

        public bool Anonymize(Guid contactId)
        {
            return _contactService.Anonymize(contactId);
        }
    }
}
