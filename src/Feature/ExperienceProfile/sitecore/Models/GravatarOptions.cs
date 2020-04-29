namespace CoreyAndRick.Feature.ExperienceProfile.Models
{
    public class GravatarOptions
    {
        public static GravatarOptions Default = new GravatarOptions
        {
            Size = 170,
            DefaultImage = "robohash",
            ForceDefault = false,
            Rating = "g"
        };

        public int Size { get; set; }
        public string DefaultImage { get; set; }
        public bool ForceDefault { get; set; }
        public string Rating { get; set; }
    }
}
