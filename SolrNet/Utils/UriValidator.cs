using System;
using SolrNet.Exceptions;

namespace SolrNet.Utils {
    public static class UriValidator {
        /// <summary>
        /// Validates that <paramref name="s"/> if a valid HTTP / HTTPS URI.
        /// Otherwise throws <see cref="InvalidURLException"/>
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <exception cref="InvalidURLException"></exception>
        public static string ValidateHTTP(string s) {
            try {
                var u = new Uri(s);
                if (u.Scheme != Uri.UriSchemeHttp && u.Scheme != Uri.UriSchemeHttps)
                    throw new InvalidURLException("Only HTTP or HTTPS protocols are supported");
                return s;
            } catch (ArgumentException e) {
                throw new InvalidURLException(string.Format("Invalid URL '{0}'", s), e);
            } catch (UriFormatException e) {
                throw new InvalidURLException(string.Format("Invalid URL '{0}'", s), e);
            }
        }

        public static int UriLength(UriBuilder u) =>
            u.Scheme.Length + 
            3 +
            u.Host.Length +
            (u.Port <= 0 ? 0 : (u.Port.ToString().Length + 1)) +
            u.UserName.Length +
            (u.UserName.Length > 0 ? 1 : 0) +
            u.Password.Length +
            (u.Password.Length > 0 ? 1 : 0) +
            u.Path.Length +
            u.Query.Length +
            u.Fragment.Length;

    }
}
