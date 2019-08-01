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
    public class PaymentTestsAsync : TestBase
    {
        [TestMethod]
        public async Task PaymentTest_Card_PostAsync_200()
        {
            Case sampleCase = this.GenerateBlankCase();

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            Payment payment = new Payment();
            payment.BINNumber = "456789";

            Payment returnPayment = await ApiClient.PostPaymentAsync(returnCase.Id, payment);

            Assert.IsTrue(returnPayment.Id != Guid.Empty);
            Assert.AreEqual(payment.BINNumber, returnPayment.BINNumber);
        }

        [TestMethod]
        public async Task PaymentTest_NonCard_PostAsync_200()
        {
            Case sampleCase = this.GenerateBlankCase();

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            Payment payment = new Payment();
            payment.PaymentType = Enums.PaymentType.Bitcoin;
            payment.BINNumber = null;

            Payment returnPayment = await ApiClient.PostPaymentAsync(returnCase.Id, payment);

            Assert.IsTrue(returnPayment.Id != Guid.Empty);
            Assert.AreEqual(payment.PaymentType, returnPayment.PaymentType);
            Assert.IsTrue(payment.BINNumber == returnPayment.BINNumber);
        }

        [TestMethod]
        public async Task PaymentTest_Card_UpdateAsync_200()
        {
            Case sampleCase = this.GenerateSampleCase();

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            Payment payment = new Payment();
            payment.BINNumber = "456789";

            Payment returnPayment = await ApiClient.UpdatePaymentAsync(returnCase.Id, payment, returnCase.Payments[0].Id);

            Assert.IsTrue(returnPayment.Id != Guid.Empty);
            Assert.AreEqual(payment.BINNumber, returnPayment.BINNumber);
        }

        [TestMethod]
        public async Task PaymentTest_NonCard_UpdateAsync_200()
        {
            Case sampleCase = this.GenerateSampleCase();

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            Payment payment = new Payment();
            payment.PaymentType = Enums.PaymentType.Bitcoin;
            payment.BINNumber = null;

            Payment returnPayment = await ApiClient.UpdatePaymentAsync(returnCase.Id, payment, returnCase.Payments[0].Id);

            Assert.IsTrue(returnPayment.Id != Guid.Empty);
            Assert.AreEqual(payment.PaymentType, returnPayment.PaymentType);
            Assert.IsTrue(payment.BINNumber == returnPayment.BINNumber);
        }

        [TestMethod]
        public async Task PaymentTest_Card_GetAsync_200()
        {
            Case sampleCase = this.GenerateSampleCase();

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            Payment returnPayment = await ApiClient.GetPaymentAsync(returnCase.Id, returnCase.Payments[0].Id);

            Assert.IsTrue(returnPayment.Id != Guid.Empty);
            Assert.AreEqual(returnCase.Payments[0].BINNumber, returnPayment.BINNumber);
        }

        [TestMethod]
        public async Task PaymentTest_NonCard_GetAsync_200()
        {
            Case sampleCase = this.GenerateSampleCase();
            sampleCase.Payments[0].PaymentType = Enums.PaymentType.Bitcoin;
            sampleCase.Payments[0].BINNumber = null;

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            Payment returnPayment = await ApiClient.GetPaymentAsync(returnCase.Id, returnCase.Payments[0].Id);

            Assert.IsTrue(returnPayment.Id != Guid.Empty);
            Assert.AreEqual(returnCase.Payments[0].BINNumber, returnPayment.BINNumber);
            Assert.AreEqual(returnCase.Payments[0].PaymentType, returnPayment.PaymentType);
        }

        [TestMethod]
        public async Task PaymentTest_Card_GetAllAsync_200()
        {
            Case sampleCase = this.GenerateSampleCase();

            Payment payment = new Payment();
            payment.BINNumber = "445566";
            sampleCase.Payments.Add(payment);

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            IList<Payment> returnPayments = await ApiClient.GetPaymentAsync(returnCase.Id);

            Assert.IsTrue(returnPayments.Count > 1);
            Assert.AreEqual("123456", returnPayments[0].BINNumber);
            Assert.AreEqual("445566", returnPayments[1].BINNumber);
        }

        [TestMethod]
        public async Task PaymentTest_NonCard_GetAllAsync_200()
        {
            Case sampleCase = this.GenerateSampleCase();
            sampleCase.Payments.SingleOrDefault().PaymentType = Enums.PaymentType.Bitcoin;
            sampleCase.Payments.SingleOrDefault().BINNumber = null;

            Payment payment = new Payment();
            payment.PaymentType = Enums.PaymentType.PayPal;
            payment.BINNumber = null;
            sampleCase.Payments.Add(payment);

            Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

            IList<Payment> returnPayments = await ApiClient.GetPaymentAsync(returnCase.Id);

            Assert.IsTrue(returnPayments.Count > 1);
            Assert.AreEqual(sampleCase.Payments[0].BINNumber, returnPayments[0].BINNumber);
            Assert.AreEqual(sampleCase.Payments[0].PaymentType, returnPayments[0].PaymentType);
            Assert.AreEqual(payment.BINNumber, returnPayments[1].BINNumber);
            Assert.AreEqual(payment.PaymentType, returnPayments[1].PaymentType);
        }

        [TestMethod]
        public async Task PaymentTest_GetAllAsync_400()
        {
            HttpStatusCode responseCode = HttpStatusCode.OK;

            try
            {
                Case sampleCase = this.GenerateBlankCase();
                Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

                IList<Payment> payments = await ApiClient.GetPaymentAsync(returnCase.Id);
            }
            catch (TrustevHttpException ex)
            {
                string message = ex.Message;
                responseCode = ex.HttpResponseCode;
            }

            Assert.AreEqual(HttpStatusCode.BadRequest, responseCode);
        }

        [TestMethod]
        public async Task PaymentTest_PaymentTypes_Enums_PostAsync_200()
        {
            foreach(Enums.PaymentType item in Enum.GetValues(typeof(Enums.PaymentType)))
            {
                Case sampleCase = this.GenerateBlankCase();

                Case returnCase = await ApiClient.PostCaseAsync(sampleCase);

                Payment payment = new Payment();
                payment.PaymentType = item;

                Payment returnPayment = await ApiClient.PostPaymentAsync(returnCase.Id, payment);

                Assert.IsTrue(returnPayment.Id != Guid.Empty);
                Assert.AreEqual(payment.PaymentType, returnPayment.PaymentType);
            }
        }

        #region SetCaseContents
        private Case GenerateBlankCase()
        {
            Case sampleCase = new Case(Guid.NewGuid(), Guid.NewGuid().ToString()) { };

            return sampleCase;
        }

        private Case GenerateSampleCase()
        {
            Case sampleCase = new Case(Guid.NewGuid(), Guid.NewGuid().ToString())
            {
                Payments = new List<Payment>()
                {
                    new Payment()
                    {
                        BINNumber = "123456"
                    }
                }
            };

            return sampleCase;
        }
    }
    #endregion
}
