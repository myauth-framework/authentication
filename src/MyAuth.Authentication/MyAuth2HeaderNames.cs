namespace MyAuth.Authentication
{
    /// <summary>
    /// Contains predefined MyAuth v2 header names 
    /// </summary>
    public static class MyAuth2HeaderNames
    {
        /// <summary>
        /// User Id
        /// </summary>
        public const string UserId = "X-Claim-User-Id";
        /// <summary>
        /// User name
        /// </summary>
        public const string UserName = "X-Claim-Name";
        /// <summary>
        /// Single role
        /// </summary>
        public const string Role = "X-Claim-Role";
        /// <summary>
        /// Multiple roles
        /// </summary>
        public const string Roles = "X-Claim-Roles";
    }
}
