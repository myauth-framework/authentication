namespace MyAuth.HeaderAuthentication
{
    static class ClaimsBlackList
    {
        public static readonly string[] Claims = new[] { "iss", "sub", "aud", "exp", "nbf", "iat", "jti", "roles" };
    }
}