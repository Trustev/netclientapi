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
    public class DecisionMultipleCredentialsTests : TestBase
    {
        private string otherUserName;

        [TestInitialize]
        public void InitializeTestMultipleCreds()
        {
            InitializeTest();
            // Other* represents another set of credentials for a different merchant
            otherUserName = ConfigurationManager.AppSettings["OtherUserName"];
            string otherPassword = ConfigurationManager.AppSettings["OtherPassword"];
            string otherSecret = ConfigurationManager.AppSettings["OtherSecret"];
            string otherPublicKey = ConfigurationManager.AppSettings["OtherPublicKey"];
            if (string.IsNullOrEmpty(altUrl))
            {
                ApiClient.SetUp(otherUserName, otherPassword, otherSecret, otherPublicKey, baseURL);
            }
            else
            {
                ApiClient.SetUp(otherUserName, otherPassword, otherSecret, otherPublicKey, altUrl);
            }
        }

        [TestMethod]
        public async Task Decision_Pass()
        {
            var session = new Session() {  };

            var result = await ApiClient.PostSessionAsync(session, firstUserName);

            Assert.IsNotNull(result);

            var sampleCase = this.GenerateTestPass(firstUserName, result.SessionId);

            var returnCase = await ApiClient.PostCaseAsync(sampleCase, firstUserName);

            var decision = await ApiClient.GetDecisionAsync(returnCase.Id, firstUserName);

            Assert.IsFalse(decision.Id == Guid.Empty);
            Assert.IsTrue(decision.Result == Enums.DecisionResult.Pass);
        }

        [TestMethod]
        public async Task Decision_Pass_Other_creds()
        {
            var session = new Session() { };

            var result = await ApiClient.PostSessionAsync(session, otherUserName);

            Assert.IsNotNull(result);

            var sampleCase = this.GenerateTestPass(otherUserName, result.SessionId);

            var returnCase = await ApiClient.PostCaseAsync(sampleCase, otherUserName);

            var decision = await ApiClient.GetDecisionAsync(returnCase.Id, otherUserName);

            Assert.IsFalse(decision.Id == Guid.Empty);
            Assert.IsTrue(decision.Result == Enums.DecisionResult.Fail);
        }

        [TestMethod]
        public async Task Decision_Pass_MultipleSessions()
        {
            var session = new Session() {  };
            var otherSession = new Session() {  };

            var result = await ApiClient.PostSessionAsync(session, firstUserName);

            Assert.IsNotNull(result);

            var otherResult = await ApiClient.PostSessionAsync(otherSession, otherUserName);

            Assert.IsNotNull(otherResult);

            var sampleCase = this.GenerateTestPass(firstUserName, result.SessionId);

            var otherCase = this.GenerateTestPass(otherUserName, otherResult.SessionId);

            var returnCase = await ApiClient.PostCaseAsync(sampleCase, firstUserName);

            var otherReturnCase = await ApiClient.PostCaseAsync(otherCase, otherUserName);

            var decision = await ApiClient.GetDecisionAsync(returnCase.Id, firstUserName);

            var otherDecision = await ApiClient.GetDecisionAsync(otherReturnCase.Id, otherUserName);

            Assert.IsFalse(decision.Id == Guid.Empty);
            Assert.IsTrue(decision.Result == Enums.DecisionResult.Pass);

            Assert.IsFalse(otherDecision.Id == Guid.Empty);
            Assert.IsTrue(otherDecision.Result == Enums.DecisionResult.Fail);
        }


        [TestMethod]
        [ExpectedException(typeof(TrustevHttpException))]
        public async Task AddCase_When_Wrong_UserName_Throws_TrustevHttpException()
        {
            var session = new Session() { };

            var result = await ApiClient.PostSessionAsync(session, firstUserName);

            Assert.IsNotNull(result);

            var sampleCase = this.GenerateTestPass(firstUserName, result.SessionId);

            var returnCase = await ApiClient.PostCaseAsync(sampleCase, firstUserName);

            var decision = await ApiClient.GetDecisionAsync(returnCase.Id, otherUserName);

        }

        [TestMethod]
        [ExpectedException(typeof(TrustevHttpException))]
        public async Task Decision_When_Wrong_UserName_Throws_TrustevHttpException()
        {
            var session = new Session() { };

            var result = await ApiClient.PostSessionAsync(session, firstUserName);

            Assert.IsNotNull(result);

            var sampleCase = this.GenerateTestPass(firstUserName, result.SessionId);

            var returnCase = await ApiClient.PostCaseAsync(sampleCase, otherUserName);
        }


        #region SetCaseContents
        private Case GenerateTestPass(string userName, Guid sessionId)
        {
            var sampleCase = new Case(Guid.NewGuid(), Guid.NewGuid().ToString() + userName + "pass")
            {
                Customer = new Customer()
                {
                    FirstName = Guid.NewGuid().ToString() + userName,
                    LastName = Guid.NewGuid().ToString() + userName
                }
            };
            sampleCase.SessionId = sessionId;

            return sampleCase;

        }

        #endregion
    }
}
