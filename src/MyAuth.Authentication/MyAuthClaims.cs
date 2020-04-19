using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace MyAuth.Authentication
{
    class MyAuthClaims : Collection<Claim>
    {
        private const string ParseRegex =
            "(?<key>[\\w_\\:\\/\\-]+)=\\\"(?<val>[\\s\\w\\d\\-_\\,\\.\\\\\\/(\\\")а-яА-ЯёЁ]+)\\\"";

        public MyAuthClaims(IEnumerable<Claim> initial)
            :base(new List<Claim>(initial))
        {
            
        }

        public MyAuthClaims()
        {
            
        }

        public string Serialize()
        {
            var roles = this
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToArray();

            var notRoles = this
                .Where(c => c.Type != ClaimTypes.Role)
                .ToArray();

            var notRolesStrings = notRoles.Select(cl =>
                $"{(cl.Type == ClaimTypes.NameIdentifier ? "sub" : cl.Type)}=\"{NormVal(cl.Value)}\"");
            
            var sb = new StringBuilder(string.Join(',', notRolesStrings));

            if (roles.Length != 0)
            {
                var rolesString = $"roles=\"{string.Join(',', roles)}\"";
                sb.Append(sb.Length != 0 ? "," : "");
                sb.Append(rolesString);
            }

            return sb.ToString();

            
        }

        public static MyAuthClaims Deserialize(string strVal)
        {
            var res= new List<Claim>();

            if (!string.IsNullOrWhiteSpace(strVal))
            {
                var matches = Regex.Matches(strVal, ParseRegex);
                if(matches.Count == 0)
                    throw new FormatException("Claims has invalid format");

                var reader = new ClaimsReader(strVal);
                while (!reader.Eof())
                {
                    var key = PrepareKeyAfterDeserialize(reader.ReadKey());
                    var claims = PrepareValueAfterDeserialize(key, reader.ReadValue());
                    
                    res.AddRange(claims);
                }

                var idClaim = res.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if(idClaim == null)
                    throw new FormatException("Required claim 'sub' not found");

                if (res.All(c => c.Type != ClaimTypes.Name))
                    res.Add(new Claim(ClaimTypes.Name, idClaim.Value));


            }

            return new MyAuthClaims(res);
        }

        private static IEnumerable<Claim> PrepareValueAfterDeserialize(string key, string value)
        {
            var normVal = HttpUtility.UrlDecode(NormVal(value));

            if (key == ClaimTypes.Role)
            {
                return normVal
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => new Claim(key, v.Trim()));
            }

            return Enumerable.Repeat(new Claim(key, normVal), 1);
        }

        private static string PrepareKeyAfterDeserialize(string key)
        {
            var normKey = key.Trim();
            string resKey = null;

            switch (normKey.ToLower())
            {
                case "sub":
                    resKey = ClaimTypes.NameIdentifier;
                    break;
                case "name":
                    resKey = ClaimTypes.Name;
                    break;
                case "role":
                case "roles":
                    resKey = ClaimTypes.Role;
                    break;
                default:
                    resKey = normKey;
                    break;
            }

            return resKey;
        }

        static string NormVal(string val) => val
            .Trim()
            .Replace("\"", "\\\"")
            .Replace("\\\\\"", "\\\"");
    }
}
