namespace Trustev.Domain.Entities
{
    using System;

    /// <summary>
    /// Document Authentication Result
    /// </summary>
    public class DocumentAuthenticationResult
    {
        /// <summary>
		/// Id
		/// </summary>
		public Guid Id { get; set; }

        /// <summary>
        /// The Url
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// The timestamp
        /// </summary>
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public DocumentAuthenticationStatus Status { get; set; }
    }
}
