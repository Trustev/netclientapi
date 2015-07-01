﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Trustev.Domain;
using Trustev.Domain.Entities;
using Trustev.Domain.Exceptions;

namespace Trustev.Web
{
    /// <summary>
    /// The ApiClient Class is used to store you trustev credentials
    /// </summary>
    public static class ApiClient
    {
        internal static string UserName { get; set; }
        internal static string Password { get; set; }
        internal static string Secret { get; set; }
        internal static string BaseUrl { get; set; }
        internal static string APIToken { get; set; }
        internal static DateTime ExpiryDate { get; set; }

        /// <summary>
        /// Initialize the trustev class by passing in you UserName, Secret and Password. If you do not have these then please contact integrationteam@trustev.com.
        /// </summary>
        /// <param name="userName">You ApiClient UserName</param>
        /// <param name="password">You ApiClient Password</param>
        /// <param name="secret">You ApiClient Secret</param>
        public static void SetUp(string userName, string password, string secret)
        {
            UserName = userName;
            Password = password;
            Secret = secret;
            BaseUrl = "https://app.trustev.com/api/v2.0";
        }

        /// <summary>
        /// Post your Case to the TrustevClient Api
        /// </summary>
        /// <param name="kase">Your Case which you want to post</param>
        /// <returns>The Case along with the Id that TrustevClient have assigned it</returns>
        public static Case PostCase(Case kase)
        {
            string uri = string.Format(Constants.URI_CASE_POST, BaseUrl);

            Case response = PerformHttpCall<Case>(uri, HttpMethod.Post, kase);

            return response;
        }

        /// <summary>
        /// Update your Case with caseId provided with the new Case object
        /// </summary>
        /// <param name="kase">Your Case which you want to Put and update the exisiting case with</param>
        /// <param name="caseId">The Case Id of the case you want to update. TrustevClient will have assigned this Id and returned it in the response Case from the Case post Method</param>
        /// <returns></returns>
        public static Case UpdateCase(Case kase, string caseId)
        {
            string uri = string.Format(Constants.URI_CASE_UPDATE, BaseUrl, caseId);

            Case response = PerformHttpCall<Case>(uri, HttpMethod.Put, kase);

            return response;
        }

        /// <summary>
        /// Get the Case with the Id caseId
        /// </summary>
        /// <param name="caseId">The case Id of the Case you want to get. TrustevClient will have assigned this Id and returned it in the response Case from the Case post Method</param>
        /// <returns></returns>
        public static Case GetCase(string caseId)
        {
            string uri = string.Format(Constants.URI_CASE_GET, BaseUrl, caseId);

            Case response = PerformHttpCall<Case>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a Decision on a Case with Id caseId.
        /// </summary>
        /// <param name="caseId">The Id of a Case which you have already posted to the TrustevClient API.</param>
        /// <returns></returns>
        public static Decision GetDecision(string caseId)
        {
            string uri = string.Format(Constants.URI_DECISON_GET, BaseUrl, caseId);

            Decision decision = PerformHttpCall<Decision>(uri, HttpMethod.Get, null);

            decision.CaseId = caseId;

            return decision;
        }

        /// <summary>
        /// This method will post the Case you provide and then get a Decison for that case. 
        /// </summary>
        /// <param name="kase">A TrustevClient Case which you have not already posted which you want a Decsion for.</param>
        /// <returns></returns>
        public static Decision GetDecison(Case kase)
        {
            Case returnCase = PostCase(kase);

            Decision decision = GetDecision(returnCase.Id);

            return decision;
        }

        /// <summary>
        /// Post your Customer to an existing Case
        /// </summary>
        /// <param name="customer">Your Customer which you want to post</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns>The Customer along with the Id that TrustevClient have assigned it</returns>
        public static Customer PostCustomer(string caseId, Customer customer)
        {
            string uri = string.Format(Constants.URI_CUSTOMER_POST, BaseUrl, caseId);

            Customer response = PerformHttpCall<Customer>(uri, HttpMethod.Post, customer);

            return response;
        }

        /// <summary>
        /// Update the Customer on a Case which already contains a customer
        /// </summary>
        /// <param name="customer">Your Customer which you want to Put and update the exisiting Customer with</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static Customer UpdateCustomer(string caseId, Customer customer)
        {
            string uri = string.Format(Constants.URI_CUSTOMER_UDPATE, BaseUrl, caseId);

            Customer response = PerformHttpCall<Customer>(uri, HttpMethod.Put, customer);

            return response;
        }


        /// <summary>
        /// Get the Customer attached to the Case
        /// </summary>
        /// <param name="caseId">The case Id of the the Case with the Customer you want to get</param>
        /// <returns></returns>
        public static Customer GetCustomer(string caseId)
        {
            string uri = string.Format(Constants.URI_CUSTOMER_GET, BaseUrl, caseId);

            Customer response = PerformHttpCall<Customer>(uri, HttpMethod.Get, null);

            return response; ;
        }

        /// <summary>
        /// Post your Transaction to an existing Case
        /// </summary>
        /// <param name="transaction">Your Transaction which you want to post</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns>The Transaction along with the Id that TrustevClient have assigned it</returns>
        public static Transaction PostTransaction(string caseId, Transaction transaction)
        {
            string uri = string.Format(Constants.URI_TRANSACTION_POST, BaseUrl, caseId);

            Transaction response = PerformHttpCall<Transaction>(uri, HttpMethod.Post, transaction);

            return response;
        }

        /// <summary>
        /// Update the Transaction on a Case which already contains a transaction
        /// </summary>
        /// <param name="transaction">Your Transaction which you want to Put and update the exisiting Transaction with</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static Transaction UpdateTransaction(string caseId, Transaction transaction)
        {
            string uri = string.Format(Constants.URI_TRANSACTION_UDPATE, BaseUrl, caseId);

            Transaction response = PerformHttpCall<Transaction>(uri, HttpMethod.Put, transaction);

            return response;
        }

        /// <summary>
        /// Get the Transaction attached to the Case
        /// </summary>
        /// <param name="caseId">The case Id of the the Case with the Transaction you want to get</param>
        /// <returns></returns>
        public static Transaction GetTransaction(string caseId)
        {
            string uri = string.Format(Constants.URI_TRANSACTION_GET, BaseUrl, caseId);

            Transaction response = PerformHttpCall<Transaction>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Post your CaseStatus to an existing Case
        /// </summary>
        /// <param name="caseStatus">Your CaseStatus which you want to post</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static CaseStatus PostCaseStatus(string caseId, CaseStatus caseStatus)
        {
            string uri = string.Format(Constants.URI_CASESTATUS_POST, BaseUrl, caseId);

            CaseStatus response = PerformHttpCall<CaseStatus>(uri, HttpMethod.Post, caseStatus);

            return response;
        }

        /// <summary>
        /// Get a specific status from a Case
        /// </summary>
        /// <param name="caseStatusId">The Id of the CaseStatus you want to get</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static CaseStatus GetCaseStatus(string caseId, Guid caseStatusId)
        {
            string uri = string.Format(Constants.URI_CASESTATUS_GET, BaseUrl, caseId, caseStatusId);

            CaseStatus response = PerformHttpCall<CaseStatus>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the statuses from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static IList<CaseStatus> GetCaseStatuses(string caseId)
        {
            string uri = string.Format(Constants.URI_CASESTATUS_GET, BaseUrl, caseId, "");

            IList<CaseStatus> response = PerformHttpCall<IList<CaseStatus>>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Post your CustomerAddress to an existing Customer on an existing Case
        /// </summary>
        /// <param name="customerAddress">Your CustomerAddress which you want to post</param>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <returns></returns>
        public static CustomerAddress PostCustomerAddress(string caseId, CustomerAddress customerAddress)
        {
            string uri = string.Format(Constants.URI_CUSTOMERADDRESS_POST, BaseUrl, caseId);

            CustomerAddress response = PerformHttpCall<CustomerAddress>(uri, HttpMethod.Post, customerAddress);

            return response;
        }

        /// <summary>
        /// Update a specific CustomerAddress on a Case which already contains a CustomerAddresses
        /// </summary>
        /// <param name="customerAddressId">The id of the CustomerAddress you want to update</param>
        /// <param name="customerAddress">The CustomerAddress you want to update the exisiting CustomerAddress to</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static CustomerAddress UpdateCustomerAddress(string caseId, CustomerAddress customerAddress, Guid customerAddressId)
        {
            string uri = string.Format(Constants.URI_CUSTOMERADDRESS_UPDATE, BaseUrl, caseId, customerAddressId);

            CustomerAddress response = PerformHttpCall<CustomerAddress>(uri, HttpMethod.Put, customerAddress);

            return response;
        }

        /// <summary>
        /// Get a specific customerAddress from a Case
        /// </summary>
        /// <param name="customerAddressId">The Id of the CustomerAddress you want to get</param>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <returns></returns>
        public static CustomerAddress GetCustomerAddress(string caseId, Guid customerAddressId)
        {
            string uri = string.Format(Constants.URI_CUSTOMERADDRESS_GET, BaseUrl, caseId, customerAddressId);

            CustomerAddress response = PerformHttpCall<CustomerAddress>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the addresses from a Customer on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <returns></returns>
        public static IList<CustomerAddress> GetCustomerAddresses(string caseId)
        {
            string uri = string.Format(Constants.URI_CUSTOMERADDRESS_GET, BaseUrl, caseId, "");

            IList<CustomerAddress> response = PerformHttpCall<IList<CustomerAddress>>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Post your Email to an existing Customer on an existing Case
        /// </summary>
        /// <param name="email">Your Email which you want to post</param>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <returns></returns>
        public static Email PostEmail(string caseId, Email email)
        {
            string uri = string.Format(Constants.URI_EMAIL_POST, BaseUrl, caseId);

            Email response = PerformHttpCall<Email>(uri, HttpMethod.Post, email);

            return response;
        }

        /// <summary>
        /// Update a specific Email on a Case which already contains a Email
        /// </summary>
        /// <param name="emailId">The id of the Email you want to update</param>
        /// <param name="email">The Email you want to update the exisiting Email to</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static Email UpdateEmail(string caseId, Email email, Guid emailId)
        {
            string uri = string.Format(Constants.URI_EMAIL_UDPATE, BaseUrl, caseId, emailId);

            Email response = PerformHttpCall<Email>(uri, HttpMethod.Put, email);

            return response;
        }

        /// <summary>
        /// Get a specific Email from a Case
        /// </summary>
        /// <param name="emailId">The Id of the Email you want to get</param>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <returns></returns>
        public static Email GetEmail(string caseId, Guid emailId)
        {
            string uri = string.Format(Constants.URI_EMAIL_GET, BaseUrl, caseId, emailId);

            Email response = PerformHttpCall<Email>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the statuses from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <returns></returns>
        public static IList<Email> GetEmails(string caseId)
        {
            string uri = string.Format(Constants.URI_EMAIL_GET, BaseUrl, caseId, "");

            IList<Email> response = PerformHttpCall<IList<Email>>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Post your Payment to an existing Case
        /// </summary>
        /// <param name="payment">Your Payment which you want to post</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static Payment PostPayment(string caseId, Payment payment)
        {
            string uri = string.Format(Constants.URI_PAYMENT_POST, BaseUrl, caseId);

            Payment response = PerformHttpCall<Payment>(uri, HttpMethod.Post, payment);

            return response;
        }

        /// <summary>
        /// Update a specific Payment on a Case which already contains a Payments
        /// </summary>
        /// <param name="paymentId">The id of the Payment you want to update</param>
        /// <param name="payment">The Payment you want to update the exisiting Payment to</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static Payment UpdatePayment(string caseId, Payment payment, Guid paymentId)
        {
            string uri = string.Format(Constants.URI_PAYMENT_UPDATE, BaseUrl, caseId, paymentId);

            Payment response = PerformHttpCall<Payment>(uri, HttpMethod.Put, payment);

            return response;
        }

        /// <summary>
        /// Get a specific Payment from a Case
        /// </summary>
        /// <param name="paymentId">The Id of the Payment you want to get</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static Payment GetPayment(string caseId, Guid paymentId)
        {
            string uri = string.Format(Constants.URI_PAYMENT_GET, BaseUrl, caseId, paymentId);

            Payment response = PerformHttpCall<Payment>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the Payments from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static IList<Payment> GetPayments(string caseId)
        {
            string uri = string.Format(Constants.URI_PAYMENT_GET, BaseUrl, caseId, "");

            IList<Payment> response = PerformHttpCall<IList<Payment>>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Post your SocialAccount to an existing Customer on an existing Case
        /// </summary>
        /// <param name="socialAccount">Your SocialAccount which you want to post</param>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <returns></returns>
        public static SocialAccount PostSocialAccount(string caseId, SocialAccount socialAccount)
        {
            string uri = string.Format(Constants.URI_SOCIALACCOUNT_POST, BaseUrl, caseId);

            SocialAccount response = PerformHttpCall<SocialAccount>(uri, HttpMethod.Post, socialAccount);

            return response;
        }

        /// <summary>
        /// Update a specific SocialAccount on a Case which already contains a SocialAccount
        /// </summary>
        /// <param name="socialAccountId">The id of the SocialAccount you want to update</param>
        /// <param name="socialAccount">The SocialAccount you want to update the exisiting SocialAccount to</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static SocialAccount UpdateSocialAccount(string caseId, SocialAccount socialAccount, Guid socialAccountId)
        {
            string uri = string.Format(Constants.URI_SOCIALACCOUNT_UPDATE, BaseUrl, caseId, socialAccountId);

            SocialAccount response = PerformHttpCall<SocialAccount>(uri, HttpMethod.Put, socialAccount);

            return response;
        }

        /// <summary>
        /// Get a specific SocialAccount from a Case
        /// </summary>
        /// <param name="socialAccountId">The Id of the SocialAccount you want to get</param>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <returns></returns>
        public static SocialAccount GetSocialAccount(string caseId, Guid socialAccountId)
        {
            string uri = string.Format(Constants.URI_SOCIALACCOUNT_GET, BaseUrl, caseId, socialAccountId);

            SocialAccount response = PerformHttpCall<SocialAccount>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the socialAccounts from a Customer on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <returns></returns>
        public static IList<SocialAccount> GetSocialAccounts(string caseId)
        {
            string uri = string.Format(Constants.URI_SOCIALACCOUNT_GET, BaseUrl, caseId, "");

            IList<SocialAccount> response = PerformHttpCall<IList<SocialAccount>>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Post your TransactionAddress to an existing Transaction on an existing Case
        /// </summary>
        /// <param name="transactionAddress">Your TransactionAddress which you want to post</param>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <returns></returns>
        public static TransactionAddress PostTransactionAddress(string caseId, TransactionAddress transactionAddress)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONADDRESS_POST, BaseUrl, caseId);

            TransactionAddress response = PerformHttpCall<TransactionAddress>(uri, HttpMethod.Post, transactionAddress);

            return response;
        }

        /// <summary>
        /// Update a specific TransactionAddress on a Case which already contains a TransactionAddress
        /// </summary>
        /// <param name="transactionAddressId">The id of the TransactionAddress you want to update</param>
        /// <param name="transactionAddress">The TransactionAddress you want to update the exisiting TransactionAddress to</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static TransactionAddress UpdateTransactionAddress(string caseId, TransactionAddress transactionAddress, Guid transactionAddressId)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONADDRESS_UPDATE, BaseUrl, caseId, transactionAddressId);

            TransactionAddress response = PerformHttpCall<TransactionAddress>(uri, HttpMethod.Put, transactionAddress);

            return response;
        }

        /// <summary>
        /// Get a specific TransactionAddress from a Case
        /// </summary>
        /// <param name="transactionAddressId">The Id of the TransactionAddress you want to get</param>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <returns></returns>
        public static TransactionAddress GetTransactionAddress(string caseId, Guid transactionAddressId)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONADDRESS_GET, BaseUrl, caseId, transactionAddressId);

            TransactionAddress response = PerformHttpCall<TransactionAddress>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the addresses from a Transaction on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <returns></returns>
        public static IList<TransactionAddress> GetTransactionAddresses(string caseId)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONADDRESS_GET, BaseUrl, caseId, "");

            IList<TransactionAddress> response = PerformHttpCall<IList<TransactionAddress>>(uri, HttpMethod.Get, null);

            return response;
        }



        /// <summary>
        /// Post your TransactionItem to an existing Transaction on an existing Case
        /// </summary>
        /// <param name="transactionItem">Your TransactionItem which you want to post</param>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <returns></returns>
        public static TransactionItem PostTransactionItem(string caseId, TransactionItem transactionItem)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONITEM_POST, BaseUrl, caseId);

            TransactionItem response = PerformHttpCall<TransactionItem>(uri, HttpMethod.Post, transactionItem);

            return response;
        }

        /// <summary>
        /// Update a specific TransactionItem on a Case which already contains a TransactionItem
        /// </summary>
        /// <param name="transactionItemId">The id of the TransactionItem you want to update</param>
        /// <param name="transactionItem">The TransactionAddress you want to update the exisiting TransactionItem to</param>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <returns></returns>
        public static TransactionItem UpdateTransactionItem(string caseId, TransactionItem transactionItem, Guid transactionItemId)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONITEM_UPDATE, BaseUrl, caseId, transactionItemId);

            TransactionItem response = PerformHttpCall<TransactionItem>(uri, HttpMethod.Put, transactionItem);

            return response;
        }

        /// <summary>
        /// Get a specific TransactionItem from a Case
        /// </summary>
        /// <param name="transactionItemId">The Id of the TransactionItem you want to get</param>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <returns></returns>
        public static TransactionItem GetTransactionItem(string caseId, Guid transactionItemId)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONITEM_GET, BaseUrl, caseId, transactionItemId);

            TransactionItem response = PerformHttpCall<TransactionItem>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// Get a all the TransacitonItems from a Transaction on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <returns></returns>
        public static IList<TransactionItem> GetTransactionItems(string caseId)
        {
            string uri = string.Format(Constants.URI_TRANSACTIONITEM_GET, BaseUrl, caseId, "");

            IList<TransactionItem> response = PerformHttpCall<IList<TransactionItem>>(uri, HttpMethod.Get, null);

            return response;
        }

        /// <summary>
        /// This method synchronously performs the Http Request to the TrustevClient API
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="entity"></param>
        /// <param name="IsAuthenticationNeeded"></param>
        /// <returns></returns>
        private static T PerformHttpCall<T>(string uri, HttpMethod method, object entity, bool IsAuthenticationNeeded = true)
        {
            try
            {
                WebRequest request = WebRequest.Create(uri);

                request.Method = method.ToString();

                request.ContentType = "application/json";

                if (IsAuthenticationNeeded)
                {
                    request.Headers.Add("X-Authorization", string.Format("{0} {1}", UserName, GetToken()));
                }

                if (method != HttpMethod.Get)
                {
                    string json = "";

                    if (entity != null && entity.GetType() != typeof(string))
                    {
                        json = JsonConvert.SerializeObject(entity);
                    }
                    else
                    {
                        json = (string)entity;
                    }

                    byte[] byteArray = Encoding.UTF8.GetBytes(json);

                    request.ContentLength = byteArray.Length;

                    Stream requestDataStream = request.GetRequestStream();

                    requestDataStream.Write(byteArray, 0, byteArray.Length);

                    requestDataStream.Close();
                }

                WebResponse response = request.GetResponse();

                Console.WriteLine(((HttpWebResponse)response).StatusDescription);

                Stream responseDataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(responseDataStream);

                string resultstring = reader.ReadToEnd();

                reader.Close();
                responseDataStream.Close();
                response.Close();

                return JsonConvert.DeserializeObject<T>(resultstring);
                
            }
            catch (WebException ex)
            {
                Stream responseDataStream = ex.Response.GetResponseStream();

                StreamReader reader = new StreamReader(responseDataStream);

                string errorMessage = reader.ReadToEnd();

                reader.Close();
                responseDataStream.Close();
                ex.Response.Close();

                throw new TrustevHttpException(((HttpWebResponse)ex.Response).StatusCode, errorMessage);
            }
        }

        #region Private Methods

        private static string GetToken()
        {
            if (string.IsNullOrEmpty(APIToken) || ExpiryDate > DateTime.UtcNow)
            {
                SetToken();
            }

            return APIToken;
        }

        private static void SetToken()
        {
            CheckCredentials();

            DateTime currentTime = DateTime.UtcNow;

            TokenRequest requestObject = new TokenRequest()
            {
                UserName = UserName,
                PasswordHash = PasswordHashHelper(Password, Secret, currentTime),
                UserNameHash = UserNameHashHelper(UserName, Secret, currentTime),
                TimeStamp = currentTime.ToString("o")
            };

            string requestJson = JsonConvert.SerializeObject(requestObject);

            string uri = string.Format("{0}/token", BaseUrl);

            TokenResponse response = PerformHttpCall<TokenResponse>(uri, HttpMethod.Post, requestJson, false);

            APIToken = response.APIToken;
            ExpiryDate = response.ExpiryDate;
        }

        /// <summary>
        /// TrustevClient token response object
        /// </summary>
        private class TokenRequest
        {
            public string UserName { get; set; }
            public string PasswordHash { get; set; }
            public string UserNameHash { get; set; }
            public string TimeStamp { get; set; }
        }

        private class TokenResponse
        {
            public string APIToken { get; set; }
            public DateTime ExpiryDate { get; set; }
        }

        /// <summary>
        /// Check that the user has set the TrustevClient Credentials correctly
        /// </summary>
        private static void CheckCredentials()
        {
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Secret) || string.IsNullOrEmpty(Password))
            {
                throw new TrustevGeneralException("You have not set your TrustevClient credentials correctly. You need to set these by calling the SetUp method on the TrustevClient Class providing your UserName, Password and Secret as the paramters before you can access the TrustevClient API");
            }
        }

        /// <summary>
        /// Has password, secret and timestamp for GetToken request
        /// </summary>
        /// <param name="password"></param>
        /// <param name="sharedsecret"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        private static string PasswordHashHelper(string password, string sharedsecret, DateTime timestamp)
        {
            sharedsecret = sharedsecret.Replace("\"", "");
            password = password.Replace("\"", "");
            return Create256Hash(Create256Hash(timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "." + password) + "." + sharedsecret);
        }

        /// <summary>
        /// Has username, secret and timestamp for GetToken request
        /// </summary>
        /// <param name="username"></param>
        /// <param name="sharedsecret"></param>
        /// <param name="timestamp"></param>
        /// <returns></returns>

        private static string UserNameHashHelper(string username, string sharedsecret, DateTime timestamp)
        {
            sharedsecret = sharedsecret.Replace("\"", "");
            username = username.Replace("\"", "");
            return Create256Hash(Create256Hash(timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "." + username) + "." + sharedsecret);
        }


        private static string Create256Hash(string toHash)
        {
            HashAlgorithm sha = new SHA256Managed();

            byte[] tohashBytes = Encoding.UTF8.GetBytes(toHash);
            byte[] resultBytes = sha.ComputeHash(tohashBytes);

            return HexEncode(resultBytes);
        }

        private static string HexEncode(byte[] data)
        {
            string result = "";
            foreach (byte b in data)
            {
                result += b.ToString("X2");
            }
            result = result.ToLower();

            return (result);

        }

        #endregion
    }
}