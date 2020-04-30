using System;
using System.Linq;
using CoreyAndRick.Feature.ExperienceProfile.Models;
using CoreyAndRick.Feature.ExperienceProfile.Services;
using CoreyAndRick.Project.Website.Models;
using Sitecore.Analytics;
using Sitecore.Analytics.Model;
using Sitecore.Analytics.Tracking;
using Sitecore.XConnect;
using Sitecore.XConnect.Client;
using Sitecore.XConnect.Client.Configuration;
using Sitecore.XConnect.Collection.Model;
using DataAccessConstants = Sitecore.Analytics.XConnect.DataAccess.Constants;
using XConnectConstants = Sitecore.XConnect.Constants;
using XConnectContact = Sitecore.XConnect.Contact;

namespace CoreyAndRick.Project.Website.Repositories
{
    public class ContactRepository
    {
        private readonly ContactManager _contactManager;
        private readonly IGravatarService _gravatarService;

        public ContactRepository(
          ContactManager contactManager,
          IGravatarService gravatarService)
        {
            _contactManager = contactManager ?? throw new ArgumentNullException(nameof(contactManager));
            _gravatarService = gravatarService ?? throw new ArgumentNullException(nameof(gravatarService));
        }

        public ContactEditorModel GetModel()
        {
            var model = new ContactEditorModel();
            using (var client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var xConnectContact = GetCurrentContact(client);
                model.IsKnown = ContactIsKnown();
                model.Identifier = GetIdentifier(xConnectContact);
                model.Avatar = GetAvatar(xConnectContact);
                model.PersonalInformation = GetPersonalInfo(xConnectContact);
                model.DefaultEmail = GetEmailAddress(xConnectContact);
            }
            return model;
        }

        public void IdentifyContact(ContactEditorModel model)
        {
            Tracker.Current.Session.IdentifyAs(model.Identifier.IdentifierSource, model.Identifier.Identifier);
        }

        public void UpdateContact(ContactEditorModel model)
        {
            using (var client = SitecoreXConnectClientConfiguration.GetClient())
            {
                var xConnectContact = GetCurrentContact(client);
                UpdatePersonalInfo(client, xConnectContact, model);
                UpdateEmailAddress(client, xConnectContact, model);
                if (model.SetAvatarFacet)
                {
                    UpdateAvatar(client, xConnectContact, model);
                }
                client.Submit();
            }
            ReloadTrackerContact(Tracker.Current.Contact.ContactId);
        }

        private static bool ContactIsKnown()
        {
            return Tracker.Current.Contact.IdentificationLevel == ContactIdentificationLevel.Known;
        }

        private static IdentifierModel GetIdentifier(XConnectContact contact)
        {
            var identifier = contact.Identifiers.FirstOrDefault(i => i.Source != DataAccessConstants.IdentifierSource
                                                                             && i.Source != XConnectConstants.AliasIdentifierSource);
            return new IdentifierModel
            {
                Identifier = identifier?.Identifier,
                IdentifierSource = identifier?.Source
            };
        }

        private static AvatarModel GetAvatar(XConnectContact contact)
        {
            var avatar = contact.Avatar();
            if (avatar == null) return new AvatarModel();
            return new AvatarModel
            {
                Picture = avatar.Picture,
                MimeType = avatar.MimeType
            };
        }

        private static PersonalInformationModel GetPersonalInfo(XConnectContact contact)
        {
            var personalInfo = contact.Personal();
            if (personalInfo == null) return new PersonalInformationModel();
            return new PersonalInformationModel
            {
                FirstName = personalInfo.FirstName,
                LastName = personalInfo.LastName
            };
        }

        private static EmailModel GetEmailAddress(XConnectContact contact)
        {
            var preferredEmail = contact.Emails()?.PreferredEmail;
            if (preferredEmail == null) return new EmailModel();
            return new EmailModel
            {
                EmailAddress = preferredEmail.SmtpAddress
            };
        }

        private XConnectContact GetCurrentContact(IXdbContext client)
        {
            var trackerContact = Tracker.Current.Contact;
            if (trackerContact.IsNew)
            {
                trackerContact.ContactSaveMode = ContactSaveMode.AlwaysSave;
                _contactManager.SaveContactToCollectionDb(trackerContact);
            }

            var contactExpandOptions = GetContactExpandOptions();
            var trackerIdentifier = new IdentifiedContactReference(DataAccessConstants.IdentifierSource, trackerContact.ContactId.ToString("N"));

            var xConnectContact = client.Get(trackerIdentifier, contactExpandOptions);
            if (xConnectContact == null) throw new InvalidOperationException($"Could not retrieve contact with identifier {trackerIdentifier.Identifier} from xConnect.");
            return xConnectContact;
        }

        private static ContactExpandOptions GetContactExpandOptions()
        {
            return new ContactExpandOptions(
              PersonalInformation.DefaultFacetKey,
              EmailAddressList.DefaultFacetKey,
              Avatar.DefaultFacetKey);
        }

        private static void UpdatePersonalInfo(
          IXdbContext client,
          XConnectContact contact,
          ContactEditorModel model)
        {
            var personalInformation = model.PersonalInformation;

            var personalInfo = contact.Personal() ?? new PersonalInformation();
            personalInfo.FirstName = personalInformation.FirstName;
            personalInfo.LastName = personalInformation.LastName;
            client.SetFacet(contact, PersonalInformation.DefaultFacetKey, personalInfo);
        }

        private static void UpdateEmailAddress(
          IXdbContext client,
          XConnectContact contact,
          ContactEditorModel model)
        {
            var homeEmail = new EmailAddress(model.DefaultEmail.EmailAddress, true);
            var emailAddresses = contact.Emails() ?? new EmailAddressList(homeEmail, "Home");
            emailAddresses.PreferredEmail = homeEmail;
            client.SetFacet(contact, EmailAddressList.DefaultFacetKey, emailAddresses);
        }

        private void UpdateAvatar(
          XConnectClient client,
          XConnectContact contact,
          ContactEditorModel model)
        {
            var emailAddress = model.DefaultEmail.EmailAddress;
            if (string.IsNullOrWhiteSpace(emailAddress)) return;

            var gravatar = _gravatarService.GetAvatar(emailAddress, GravatarOptions.Default);
            if (gravatar == null) return;

            var avatar = contact.Avatar() ?? new Avatar(gravatar.MimeType, gravatar.Content);
            avatar.MimeType = gravatar.MimeType;
            avatar.Picture = gravatar.Content;
            client.SetFacet(contact, Avatar.DefaultFacetKey, avatar);
        }

        private void ReloadTrackerContact(Guid contactId)
        {
            _contactManager.RemoveFromSession(contactId);
            Tracker.Current.Session.Contact = _contactManager.LoadContact(contactId);
        }
    }
}
