using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace TestServer.Models
{
    public class ClaimModel
    {
        public string Type { get; set; }
        public string Value { get; set; }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimModel"/>
        /// </summary>
        public ClaimModel(Claim claim)
        {
            Type = claim.Type;
            Value = claim.Value;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ClaimModel"/>
        /// </summary>
        public ClaimModel()
        {

        }
    }
}
