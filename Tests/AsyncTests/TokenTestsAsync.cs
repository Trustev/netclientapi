using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Trustev.Domain;
using Trustev.Domain.Entities;
using Trustev.Domain.Exceptions;
using Trustev.WebAsync;

namespace Tests.AsyncTests
{
    [TestClass]
    public class TokenTestsAsync : TestBase
    {
        [TestMethod]
        public async Task TestRenewApiToken()
        {
            var currentApiToken = await ApiClient.GetTokenAsync();
            ApiClient.ExpireAt = DateTime.UtcNow.AddMinutes(-1);
            var newApiToken = await ApiClient.GetTokenAsync();

            Assert.AreNotEqual(currentApiToken, newApiToken);
        }

        [TestMethod]
        public async Task TestKeepSameApiToken()
        {
            var currentApiToken = await ApiClient.GetTokenAsync();
            var newApiToken = await ApiClient.GetTokenAsync();

            Assert.AreEqual(currentApiToken, newApiToken);
        }
    }
}
