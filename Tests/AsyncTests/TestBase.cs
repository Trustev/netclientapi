using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustev.Domain;
using Trustev.WebAsync;


namespace Tests.AsyncTests
{
    public class TestBase
    {
        protected string firstUserName;

        [TestInitialize]
        public void InitializeTest()
        {
            firstUserName = ConfigurationManager.AppSettings["UserName"];
            string password = ConfigurationManager.AppSettings["Password"];
            string secret = ConfigurationManager.AppSettings["Secret"];
            string publicKey = ConfigurationManager.AppSettings["PublicKey"];
            string altUrl = ConfigurationManager.AppSettings["AltUrl"];

            if (string.IsNullOrEmpty(altUrl))
            {
                Enums.BaseUrl baseURL;
                Enum.TryParse(ConfigurationManager.AppSettings["BaseURL"], out baseURL);
                ApiClient.SetUp(firstUserName, password, secret, publicKey, baseURL);
            }
            else
            {
                ApiClient.SetUp(firstUserName, password, secret, publicKey, altUrl);
            }
        }
    }
}
