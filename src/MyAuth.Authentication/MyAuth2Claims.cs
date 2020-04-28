using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace MyAuth.Authentication
{
    class MyAuth2Claims : Collection<Claim>
    {
        private static readonly char[] ClaimTypeSeparators = ":-".ToCharArray();

        public MyAuth2Claims(IEnumerable<Claim> initial)
            :base(new List<Claim>(initial))
        {
            
        }

        public MyAuth2Claims()
        {
            
        }

        public static MyAuth2Claims LoadFomHeaders(IHeaderDictionary headerDictionary)
        {
            if (headerDictionary == null) throw new ArgumentNullException(nameof(headerDictionary));
            
            var res= new List<Claim>();

            foreach (var pair in headerDictionary.Where(h => h.Key.StartsWith("X-Claim-")))
            {
                string headerKey = pair.Key;

                if (headerKey == MyAuth2HeaderNames.Roles)
                {
                    var roleClaims = pair.Value.ToString()
                        .Split(',', StringSplitOptions.RemoveEmptyEntries)
                        .Select(r => new Claim(ClaimTypes.Role, HttpUtility.UrlDecode(r)));
                    
                    res.AddRange(roleClaims);
                }
                else
                {
                    string claimType;
                    switch (headerKey)
                    {
                        case MyAuth2HeaderNames.UserId:
                            claimType = ClaimTypes.NameIdentifier;
                            break;
                        case MyAuth2HeaderNames.Role:
                            claimType = ClaimTypes.Role;
                            break;
                        case MyAuth2HeaderNames.UserName:
                            claimType = ClaimTypes.Name;
                            break;
                        default:
                            claimType = headerKey.Substring(8).ToLower();
                            break;
                    }

                    string claimValue = HttpUtility.UrlDecode(pair.Value);

                    res.Add(new Claim(claimType, claimValue));
                }
            }

            return new MyAuth2Claims(res);
        }

        public IEnumerable<KeyValuePair<string, string>> ToHeaders()
        {
            var roles = this
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            var notRoles = this
                .Where(c => c.Type != ClaimTypes.Role)
                .ToArray();

            foreach (var claim in notRoles)
            {
                string claimKey;

                switch (claim.Type)
                {
                    case ClaimTypes.NameIdentifier: claimKey = MyAuth2HeaderNames.UserId; break;
                    case ClaimTypes.Name: claimKey = MyAuth2HeaderNames.UserName; break;
                    default:
                    {
                        var words = claim.Type
                            .Split(ClaimTypeSeparators, StringSplitOptions.RemoveEmptyEntries)
                            .Select(w => w.Remove(1).ToUpper() + w.Substring(1));
                        claimKey = "X-Claim-" + string.Join('-', words);
                    }
                    break;
                }

                yield return new KeyValuePair<string, string>(claimKey, claim.Value);
            }

            yield return new KeyValuePair<string, string>(MyAuth2HeaderNames.Roles, string.Join(',', roles));
        }
    }
}
