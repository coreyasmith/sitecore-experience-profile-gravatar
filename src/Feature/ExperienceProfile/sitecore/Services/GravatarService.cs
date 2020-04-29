using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using CoreyAndRick.Feature.ExperienceProfile.Models;

namespace CoreyAndRick.Feature.ExperienceProfile.Services
{
    public class GravatarService : IGravatarService
    {
        private const string BaseGravatarUrl = "https://gravatar.com/avatar";

        private static readonly HttpClient HttpClient = new HttpClient();

        public GravatarResponse GetAvatar(string email, GravatarOptions options)
        {
            var formattedEmail = FormatEmail(email);
            var emailHash = ConvertEmailToMd5(formattedEmail);
            var gravatarUrl = GetGravatarUrl(emailHash, options);

            var response = Task.Run(() => HttpClient.GetAsync(gravatarUrl)).Result;
            var gravatar = new GravatarResponse
            {
                Content = response.Content.ReadAsByteArrayAsync().Result,
                MimeType = response.Content.Headers.ContentType.MediaType
            };
            return gravatar;
        }

        private static string FormatEmail(string email)
        {
            return email.Trim().ToLower();
        }

        private static string ConvertEmailToMd5(string email)
        {
            using (var md5 = MD5.Create())
            {
                var md5Bytes = md5.ComputeHash(Encoding.UTF8.GetBytes(email));
                var sb = new StringBuilder();
                foreach (var md5Byte in md5Bytes)
                {
                    sb.Append(md5Byte.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        private static string GetGravatarUrl(string emailHash, GravatarOptions options)
        {
            return $"{BaseGravatarUrl}/{emailHash}?{ToQueryString(options)}";
        }

        private static string ToQueryString(GravatarOptions options)
        {
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            if (options.Size > 0)
            {
                queryString["s"] = options.Size.ToString();
            }
            if (!string.IsNullOrWhiteSpace(options.DefaultImage))
            {
                queryString["d"] = options.DefaultImage;
            }
            if (options.ForceDefault)
            {
                queryString["f"] = "y";
            }
            if (!string.IsNullOrWhiteSpace(options.Rating))
            {
                queryString["r"] = options.Rating;
            }
            return queryString.ToString();
        }
    }
}
