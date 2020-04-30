namespace CoreyAndRick.Project.Website.Models
{
    public class ContactEditorModel
    {
        public bool IsKnown { get; set; }
        public bool AvatarIsSet => Avatar?.Picture != null && !string.IsNullOrWhiteSpace(Avatar?.MimeType);

        public IdentifierModel Identifier { get; set; }
        public AvatarModel Avatar { get; set; }
        public PersonalInformationModel PersonalInformation { get; set; }
        public EmailModel DefaultEmail { get; set; }
        public bool SetAvatarFacet { get; set; }

        public ContactEditorModel()
        {
            Identifier = new IdentifierModel();
            Avatar = new AvatarModel();
            PersonalInformation = new PersonalInformationModel();
            DefaultEmail = new EmailModel();
        }
    }
}
