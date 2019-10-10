namespace Trustev.WebAsync
{
    using System;

    [Serializable]
    internal sealed class SiteCredential
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string Secret { get; set; }

        public string PublicKey { get; set; }
    }
}
