namespace MyAuth.HeaderAuthentication
{
    /// <summary>
    /// Contains constant definitions
    /// </summary>
    public class HeaderBasedDefinitions
    {
        public const string AuthenticationScheme = "Header";

        public const string UserIdHeaderName = "X-User-Id";
        public const string UserClaimsHeaderName = "X-User-Claims";
    }
}
