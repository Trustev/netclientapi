namespace Trustev.Domain.Entities
{
    using System;

    public class DigitalAuthenticationResult
    {
        public string Id { get; set; }

        public DateTime Timestamp { get; set; }

        public OTPResult OTP { get; set; }

        public KBAResult KBA { get; set; }

        public DocumentAuthenticationResult DocumentAuthentication { get; set; }
    }
}