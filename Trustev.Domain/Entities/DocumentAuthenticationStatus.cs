
namespace Trustev.Domain.Entities
{
    /// <summary>
    /// Document authentication status
    /// </summary>
    public enum DocumentAuthenticationStatus
    {
        /// <summary>
        /// Case is Eligible and DocumentAuthentication Offered 
        /// </summary>
        Offered = 1,

        /// <summary>
        /// Case is Ineligible and DocumentAuthentication was not Offered
        /// </summary>
        Ineligible = 2,

        /// <summary>
        /// Final state of Abandoned
        /// </summary>
        Error = 3,

        /// <summary>
        /// DocumentAuthentication is not Configured
        /// </summary>
        NotConfigured = 4
    }
}
