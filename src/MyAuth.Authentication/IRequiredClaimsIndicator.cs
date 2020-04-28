namespace MyAuth.Authentication
{
    /// <summary>
    /// Specifies controller method input parameter which indicates that required claim headers has specified 
    /// </summary>
    public interface IRequiredClaimsIndicator
    {
        /// <summary>
        /// Indicates that required claim headers has specified 
        /// </summary>
        bool RequiredClaimHeadersHasSpecified();
    }
}
