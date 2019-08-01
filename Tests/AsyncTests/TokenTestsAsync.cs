using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustev.WebAsync;
using static Trustev.WebAsync.ApiClient;

namespace Tests.AsyncTests
{
    [TestClass]
    public class TokenTestsAsync : TestBase
    {
        [TestMethod]
        public async Task TestRenewApiToken()
        {
            var currentApiToken = await ApiClient.GetTokenAsync(firstUserName);
            var tokenResponse = new TokenResponse()
            {
                APIToken = currentApiToken,
                ExpireAt = DateTime.UtcNow.AddMinutes(-1)
            };
            ApiClient.AddOrUpdateToken(firstUserName, tokenResponse);
            var newApiToken = await ApiClient.GetTokenAsync(firstUserName);

            Assert.AreNotEqual(currentApiToken, newApiToken);
        }

        [TestMethod]
        public async Task TestKeepSameApiToken()
        {
            var currentApiToken = await ApiClient.GetTokenAsync(firstUserName);
            var newApiToken = await ApiClient.GetTokenAsync(firstUserName);

            Assert.AreEqual(currentApiToken, newApiToken);
        }
    }
}
