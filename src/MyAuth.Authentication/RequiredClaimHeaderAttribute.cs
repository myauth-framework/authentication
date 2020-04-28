using Microsoft.AspNetCore.Mvc;

namespace MyAuth.Authentication
{
    /// <summary>
    /// Determines controller method input parameter which bound to required Claim header
    /// </summary>
    public class RequiredClaimHeaderAttribute : FromHeaderAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="RequiredClaimHeaderAttribute"/>
        /// </summary>
        public RequiredClaimHeaderAttribute(string headerName)
        {
            Name = headerName;
        }
    }
}
