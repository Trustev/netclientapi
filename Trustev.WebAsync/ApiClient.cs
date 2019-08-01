using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Trustev.Domain;
using Trustev.Domain.Entities;
using Trustev.Domain.Exceptions;

namespace Trustev.WebAsync
{
    /// <summary>
    /// The ApiClient has all the methods required to communicate with the Trustev Platform
    /// </summary>
    public static class ApiClient
    {
        /// <summary>
        /// List of credentials - The key will be the user name
        /// </summary>
        private static IDictionary<string, SiteCredential> Credentials = new Dictionary<string, SiteCredential>();

        /// <summary>
        /// Base url used to connect to Trustev Api
        /// </summary>
        private static string BaseUrl { get; set; }

        /// <summary>
        /// Request Timeout
        /// </summary>
        internal static int HttpRequestTimeout { get; set; }

        /// <summary>
        /// lock object for enforcing thread safety
        /// </summary>
        private static readonly object TokenLock = new object();

        /// <summary>
        /// Credentials Lock
        /// </summary>
        private static readonly object CredentialsLock = new object();

        /// <summary>
        /// Token
        /// </summary>
        private static IDictionary<string, TokenResponse> Tokens = new Dictionary<string, TokenResponse>();

        /// <summary>
        /// Gets an API AUth token
        /// </summary>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <remarks>
        /// When the username is left empty or null it will retrieve the first token in the list
        /// </remarks>
        private static TokenResponse GetApiToken(string userName = "")
        {
            TokenResponse token = null;
            lock (TokenLock)
            {
                if (Tokens.ContainsKey(userName))
                {
                    token =  Tokens[userName];
                }
                else
                {
                    // Backwards compatability - when the userName is not supplied, then we use the first.
                    if (Tokens.Count > 0)
                    {
                        var key = Tokens.Keys.ToList().FirstOrDefault();
                        token = Tokens[key];
                    }
                }
            }
            return token;
        }

        /// <summary>
        /// Gets the credentials
        /// <param name="userName">Required: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// </summary>
        private static SiteCredential GetCredential(string userName)
        {
            SiteCredential siteCredential = null;
            lock (CredentialsLock)
            {
                if (Credentials.ContainsKey(userName))
                {
                    siteCredential = Credentials[userName];
                }
                else
                {
                    // Backwards compatability - when the userName is not supplied, then we use the first.
                    if (Credentials.Count > 0)
                    {
                        var key = Credentials.Keys.ToList().FirstOrDefault();
                        siteCredential = Credentials[key];
                    }
                }
                if (siteCredential != null)
                {
                    // Clone th eobject for concurrency reasons
                    return DeepClone(siteCredential);
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the credentials
        /// </summary>
        private static void AddOrUpdate(string username, string password, string secret, string publicKey = "")
        {
            lock (CredentialsLock)
            {
                var credential = new SiteCredential()
                {
                    UserName = username,
                    Password = password,
                    PublicKey = publicKey,
                    Secret = secret
                };
                if (Credentials.ContainsKey(username))
                {
                    Credentials[username] = credential;
                }
                else
                {
                    Credentials.Add(username, credential);
                }
            }
        }

        /// <summary>
        /// Adds or updates a token
        /// </summary>
        /// <param name="username">The user name associated with this token</param>
        /// <param name="tokenResponse">the token response, that was retrieved from the API</param>
        private static void AddOrUpdateToken(string username, TokenResponse tokenResponse)
        {
            lock (CredentialsLock)
            {
                if (Tokens.ContainsKey(username))
                {
                    Tokens[username] = tokenResponse;
                }
                else
                {
                    Tokens.Add(username, tokenResponse);
                }
            }
        }

        /// <summary>
        /// Determines whether or not a new token will be generated on each request. Defaults to false unless otherwise specified.
        /// </summary>
        private static Boolean RegenerateTokenOnEachRequest { get; set; } = false;


        static ApiClient()
        {

        }

        /// <summary>
        /// Initialize the Trustev class by passing in your UserName, Password, Secret, and BaseUrl - This could be EU or US, depending on your location.
        /// </summary>
        /// <param name="userName">Your ApiClient UserName</param>
        /// <param name="password">Your ApiClient Password</param>
        /// <param name="secret">Your ApiClient Secret</param>
        /// <param name="baseUrl">Your BaseURL - US/EU</param>
        /// <param name="regenerateToken">Flag to generateToken on each request</param>
        /// <param name="httpRequestTimeout">The timeout value of this http request in milliseconds</param>
        public static void SetUp(string userName, string password, string secret, Enums.BaseUrl baseUrl, Boolean regenerateToken = false, int httpRequestTimeout = 15000)
        {
            AddOrUpdate(userName, password, secret);
            if (baseUrl.Equals(Enums.BaseUrl.EU))
            {
                BaseUrl = "https://app-eu.trustev.com/api/v2.0";
            }
            else if (baseUrl.Equals(Enums.BaseUrl.US))
            {
                BaseUrl = "https://app.trustev.com/api/v2.0";
            }
            RegenerateTokenOnEachRequest = regenerateToken;
            HttpRequestTimeout = httpRequestTimeout;
        }

        /// <summary>
        /// Initialize the Trustev class by passing in your UserName, Password, Secret, and BaseUrl - This could be EU or US, depending on your location.
        /// </summary>
        /// <param name="userName">Your ApiClient UserName</param>
        /// <param name="password">Your ApiClient Password</param>
        /// <param name="secret">Your ApiClient Secret</param>
        /// <param name="publicKey">Your ApiClient Public Key</param>
        /// <param name="baseUrl">Your BaseURL - US/EU</param>
        /// <param name="regenerateToken">Flag to generateToken on each request</param>
        /// <param name="httpRequestTimeout">The timeout value of this http request in milliseconds</param>
        public static void SetUp(string userName, string password, string secret, string publicKey, Enums.BaseUrl baseUrl, Boolean regenerateToken = false, int httpRequestTimeout = 15000)
        {
            AddOrUpdate(userName, password, secret, publicKey);

            if (baseUrl.Equals(Enums.BaseUrl.EU))
            {
                BaseUrl = "https://app-eu.trustev.com/api/v2.0";
            }
            else if (baseUrl.Equals(Enums.BaseUrl.US))
            {
                BaseUrl = "https://app.trustev.com/api/v2.0";
            }
            RegenerateTokenOnEachRequest = regenerateToken;
            HttpRequestTimeout = httpRequestTimeout;
        }

        /// <summary>
        /// Initialize the Trustev class by passing in your UserName, Password, Secret, and BaseUrl
        /// </summary>
        /// <param name="userName">Your ApiClient UserName</param>
        /// <param name="password">Your ApiClient Password</param>
        /// <param name="secret">Your ApiClient Secret</param>
        /// <param name="baseUrl">Your BaseURL - specified through a url string</param>
        /// <param name="regenerateToken">Flag to generateToken on each request</param>
        /// <param name="httpRequestTimeout">Your default httpRequestTimeout</param>
        public static void SetUp(string userName, string password, string secret, string baseUrl, Boolean regenerateToken = false, int httpRequestTimeout = 15000)
        {
            AddOrUpdate(userName, password, secret);

            BaseUrl = baseUrl;
            RegenerateTokenOnEachRequest = regenerateToken;
            HttpRequestTimeout = httpRequestTimeout;
        }

        /// <summary>
        /// Initialize the Trustev class by passing in your UserName, Password, Secret, PublicKey, and BaseUrl
        /// </summary>
        /// <param name="userName">Your ApiClient UserName</param>
        /// <param name="password">Your ApiClient Password</param>
        /// <param name="secret">Your ApiClient Secret</param>
        /// <param name="publicKey">Your ApiClient Public Key</param>
        /// <param name="baseUrl">Your BaseURL - specified through a url string</param>
        /// <param name="regenerateToken">Flag to generateToken on each request</param>
        /// <param name="httpRequestTimeout">Your default httpRequestTimeout</param>
        public static void SetUp(string userName, string password, string secret, string publicKey, string baseUrl, Boolean regenerateToken = false, int httpRequestTimeout = 15000)
        {
            AddOrUpdate(userName, password, secret, publicKey);
            BaseUrl = baseUrl;
            RegenerateTokenOnEachRequest = regenerateToken;
            HttpRequestTimeout = httpRequestTimeout;
        }

        /// <summary>
        /// Post your Case to the TrustevClient Api
        /// </summary>
        /// <param name="session">Your Session which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns>The Session along with the Id that TrustevClient have assigned it</returns>
        public static async Task<Session> PostSessionAsync(Session session, string userName = "")
        {
            string uri = string.Format(Constants.UriSessionPost, BaseUrl);

            Session response = await PerformHttpCallAsync<Session>(uri, HttpMethod.Post, session, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your Detail to an existing Session
        /// </summary>
        /// <param name="sessionId">The Id of the session you want to add the detail to. It comes back as part of the Session Post Response</param>
        /// <param name="detail">Your Detail which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns>The Session along with the Id that TrustevClient have assigned it</returns>
        public static async Task<Detail> PostDetailAsync(Guid sessionId, Detail detail, string userName = "")
        {
            string uri = string.Format(Constants.UriDetailPost, BaseUrl, sessionId);

            Detail response = await PerformHttpCallAsync<Detail>(uri, HttpMethod.Post, detail, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your Case to the TrustevClient Api asynchronously
        /// </summary>
        /// <param name="kase">Your Case which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns>The Case along with the Id that TrustevClient have assigned it</returns>
        public static async Task<Case> PostCaseAsync(Case kase, string userName = "")
        {
            string uri = string.Format(Constants.UriCasePost, BaseUrl);

            Case responseCase = await PerformHttpCallAsync<Case>(uri, HttpMethod.Post, kase, true, HttpRequestTimeout, userName);

            return responseCase;
        }

        /// <summary>
        /// Update your Case with caseId provided with the new Case object asynchronously
        /// </summary>
        /// <param name="kase">Your Case which you want to Put and update the existing case with</param>
        /// <param name="caseId">The Case Id of the case you want to update. TrustevClient will have assigned this Id and returned it in the response Case from the Case post Method</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Case> UpdateCaseAsync(Case kase, string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriCaseUpdate, BaseUrl, caseId);

            Case response = await PerformHttpCallAsync<Case>(uri, HttpMethod.Put, kase, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get the Case with the Id caseId
        /// </summary>
        /// <param name="caseId">The case Id of the Case you want to get. TrustevClient will have assigned this Id and returned it in the response Case from the Case post Method</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Case> GetCaseAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriCaseGet, BaseUrl, caseId);

            Case response = await PerformHttpCallAsync<Case>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a Decision on a Case with Id caseId.
        /// </summary>
        /// <param name="caseId">The Id of a Case which you have already posted to the TrustevClient API.</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Decision> GetDecisionAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriDecisionGet, BaseUrl, caseId);

            Decision decision = await PerformHttpCallAsync<Decision>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            decision.CaseId = caseId;

            return decision;
        }

        /// <summary>
        /// Get a Detailed Decision on a Case with Id caseId.
        /// </summary>
        /// <param name="caseId">The Id of a Case which you have already posted to the TrustevClient API.</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<DetailedDecision> GetDetailedDecisionAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriDetailedDecisionGet, BaseUrl, caseId);

            DetailedDecision detailedDecision = await PerformHttpCallAsync<DetailedDecision>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            detailedDecision.CaseId = caseId;

            return detailedDecision;
        }

        /// <summary>
        /// Use this endpoint and HTTP method to Request OR Regenerate a OTP to a previously created Trustev Case.
        /// </summary>
        /// <param name="caseId">
        /// CaseId - This is returned in the Response Header when a Trustev Case is created. 
        /// </param>
        /// <param name="request">
        /// Status Request Object 
        /// </param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns>
        /// </returns>
        public static async Task<OTPResult> PostOtpAsync(string caseId, OTPResult request, string userName = "")
        {
            var uri = string.Format(Constants.UriOtp, BaseUrl, caseId);

            var digitalAuthenticationResult = await PerformHttpCallAsync<OTPResult>(uri, HttpMethod.Post, request, true, HttpRequestTimeout, userName);
            return digitalAuthenticationResult;
        }

        /// <summary>
        /// Use this endpoint and HTTP method to Request a OTP Verification to a previously created OTP.
        /// </summary>
        /// <param name="caseId">
        /// CaseId - This is returned in the Response Header when a Trustev Case is created. 
        /// </param>
        /// <param name="request">
        /// Status Request Object 
        /// </param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns>
        /// </returns>
        public static async Task<OTPResult> PutOtpAsync(string caseId, OTPResult request, string userName = "")
        {
            var uri = string.Format(Constants.UriOtp, BaseUrl, caseId);

            var digitalAuthenticationResult = await PerformHttpCallAsync<OTPResult>(uri, HttpMethod.Put, request, true, HttpRequestTimeout, userName);
            return digitalAuthenticationResult;
        }

        /// <summary>
        /// Post your Customer to an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="customer">Your Customer which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns>The Customer along with the Id that TrustevClient have assigned it</returns>
        public static async Task<Customer> PostCustomerAsync(string caseId, Customer customer, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerPost, BaseUrl, caseId);

            Customer response = await PerformHttpCallAsync<Customer>(uri, HttpMethod.Post, customer, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update the Customer on a Case which already contains a customer
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="customer">Your Customer which you want to Put and update the existing Customer with</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Customer> UpdateCustomerAsync(string caseId, Customer customer, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerUpdate, BaseUrl, caseId);

            Customer response = await PerformHttpCallAsync<Customer>(uri, HttpMethod.Put, customer, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get the Customer attached to the Case
        /// </summary>
        /// <param name="caseId">The case Id of the the Case with the Customer you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Customer> GetCustomerAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerGet, BaseUrl, caseId);

            Customer response = await PerformHttpCallAsync<Customer>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your Transaction to an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="transaction">Your Transaction which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Transaction> PostTransactionAsync(string caseId, Transaction transaction, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionPost, BaseUrl, caseId);

            Transaction response = await PerformHttpCallAsync<Transaction>(uri, HttpMethod.Post, transaction, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update the Transaction on a Case which already contains a transaction
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="transaction">Your Transaction which you want to Put and update the existing Transaction with</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Transaction> UpdateTransactionAsync(string caseId, Transaction transaction, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionUpdate, BaseUrl, caseId);

            Transaction response = await PerformHttpCallAsync<Transaction>(uri, HttpMethod.Put, transaction, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get the Transaction attached to the Case
        /// </summary>
        /// <param name="caseId">The case Id of the the Case with the Transaction you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Transaction> GetTransactionAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionGet, BaseUrl, caseId);

            Transaction response = await PerformHttpCallAsync<Transaction>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your CaseStatus to an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="caseStatus">Your CaseStatus which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<CaseStatus> PostCaseStatusAsync(string caseId, CaseStatus caseStatus, string userName = "")
        {
            string uri = string.Format(Constants.UriCaseStatusPost, BaseUrl, caseId);

            CaseStatus response = await PerformHttpCallAsync<CaseStatus>(uri, HttpMethod.Post, caseStatus, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a specific status from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="caseStatusId">The Id of the CaseStatus you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<CaseStatus> GetCaseStatusAsync(string caseId, Guid caseStatusId, string userName = "")
        {
            string uri = string.Format(Constants.UriCaseStatusGet, BaseUrl, caseId, caseStatusId);

            CaseStatus response = await PerformHttpCallAsync<CaseStatus>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a all the statuses from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<IList<CaseStatus>> GetCaseStatusesAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriCaseStatusesGet, BaseUrl, caseId);

            IList<CaseStatus> response = await PerformHttpCallAsync<IList<CaseStatus>>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your CustomerAddress to an existing Customer on an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <param name="customerAddress">Your CustomerAddress which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<CustomerAddress> PostCustomerAddressAsync(string caseId, CustomerAddress customerAddress, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerAddressPost, BaseUrl, caseId);

            CustomerAddress response = await PerformHttpCallAsync<CustomerAddress>(uri, HttpMethod.Post, customerAddress, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update a specific CustomerAddress on a Case which already contains a CustomerAddresses
        /// </summary><param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="customerAddress">The CustomerAddress you want to update the existing CustomerAddress to</param>
        /// <param name="customerAddressId">The id of the CustomerAddress you want to update</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<CustomerAddress> UpdateCustomerAddressAsync(string caseId, CustomerAddress customerAddress, Guid customerAddressId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerAddressUpdate, BaseUrl, caseId, customerAddressId);

            CustomerAddress response = await PerformHttpCallAsync<CustomerAddress>(uri, HttpMethod.Put, customerAddress, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a specific customerAddress from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <param name="customerAddressId">The Id of the CustomerAddress you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<CustomerAddress> GetCustomerAddressAsync(string caseId, Guid customerAddressId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerAddressGet, BaseUrl, caseId, customerAddressId);

            CustomerAddress response = await PerformHttpCallAsync<CustomerAddress>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a all the addresses from a Customer on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer  which you have already posted</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<IList<CustomerAddress>> GetCustomerAddressesAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerAddressesGet, BaseUrl, caseId);

            IList<CustomerAddress> response = await PerformHttpCallAsync<IList<CustomerAddress>>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your Customer Email to an existing Customer on an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <param name="email">Your Customer Email which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Email> PostEmailAsync(string caseId, Email email, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerEmailPost, BaseUrl, caseId);

            Email response = await PerformHttpCallAsync<Email>(uri, HttpMethod.Post, email, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update a specific Customer Email on a Case which already contains a Email
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="email">The Customer Email you want to update the existing Email to</param>
        /// <param name="emailId">The Id of the Email you want to update</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Email> UpdateEmailAsync(string caseId, Email email, Guid emailId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerEmailUpdate, BaseUrl, caseId, emailId);

            Email response = await PerformHttpCallAsync<Email>(uri, HttpMethod.Put, email, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a specific Customer Email from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <param name="emailId">The Id of the Email you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Email> GetEmailAsync(string caseId, Guid emailId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerEmailGet, BaseUrl, caseId, emailId);

            Email response = await PerformHttpCallAsync<Email>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get all the Customer Emails from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<IList<Email>> GetEmailsAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriCustomerEmailsGet, BaseUrl, caseId);

            IList<Email> response = await PerformHttpCallAsync<IList<Email>>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your Payment to an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="payment">Your Payment which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Payment> PostPaymentAsync(string caseId, Payment payment, string userName = "")
        {
            string uri = string.Format(Constants.UriPaymentPost, BaseUrl, caseId);

            Payment response = await PerformHttpCallAsync<Payment>(uri, HttpMethod.Post, payment, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update a specific Payment on a Case which already contains a Payments
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="payment">The Payment you want to update the existing Payment to</param>
        /// <param name="paymentId">The id of the Payment you want to update</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Payment> UpdatePaymentAsync(string caseId, Payment payment, Guid paymentId, string userName = "")
        {
            string uri = string.Format(Constants.UriPaymentUpdate, BaseUrl, caseId, paymentId);

            Payment response = await PerformHttpCallAsync<Payment>(uri, HttpMethod.Put, payment, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a specific Payment from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="paymentId">The Id of the Payment you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<Payment> GetPaymentAsync(string caseId, Guid paymentId, string userName = "")
        {
            string uri = string.Format(Constants.UriPaymentGet, BaseUrl, caseId, paymentId);

            Payment response = await PerformHttpCallAsync<Payment>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get all the Payments from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<IList<Payment>> GetPaymentAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriPaymentsGet, BaseUrl, caseId);

            IList<Payment> response = await PerformHttpCallAsync<IList<Payment>>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your TransactionAddress to an existing Transaction on an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <param name="transactionAddress">Your TransactionAddress which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<TransactionAddress> PostTransactionAddressAsync(string caseId, TransactionAddress transactionAddress, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionAddressPost, BaseUrl, caseId);

            TransactionAddress response = await PerformHttpCallAsync<TransactionAddress>(uri, HttpMethod.Post, transactionAddress, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update a specific TransactionAddress on a Case which already contains a TransactionAddress
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="transactionAddress">The TransactionAddress you want to update the existing TransactionAddress to</param>
        /// <param name="transactionAddressId">The id of the TransactionAddress you want to update</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<TransactionAddress> UpdateTransactionAddressAsync(string caseId, TransactionAddress transactionAddress, Guid transactionAddressId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionAddressUpdate, BaseUrl, caseId, transactionAddressId);

            TransactionAddress response = await PerformHttpCallAsync<TransactionAddress>(uri, HttpMethod.Put, transactionAddress, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a specific TransactionAddress from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <param name="transactionAddressId">The Id of the TransactionAddress you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<TransactionAddress> GetTransactionAddressAsync(string caseId, Guid transactionAddressId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionAddressGet, BaseUrl, caseId, transactionAddressId);

            TransactionAddress response = await PerformHttpCallAsync<TransactionAddress>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a all the addresses from a Transaction on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<IList<TransactionAddress>> GetTransactionAddresssesAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionAddressesGet, BaseUrl, caseId);

            IList<TransactionAddress> response = await PerformHttpCallAsync<IList<TransactionAddress>>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your TransactionItem to an existing Transaction on an existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <param name="transactionItem">Your TransactionItem which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<TransactionItem> PostTransactionItemAsync(string caseId, TransactionItem transactionItem, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionItemPost, BaseUrl, caseId);

            TransactionItem response = await PerformHttpCallAsync<TransactionItem>(uri, HttpMethod.Post, transactionItem, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Update a specific TransactionItem on a Case which already contains a TransactionItem
        /// </summary>
        /// <param name="caseId">The Case Id of a Case which you have already posted</param>
        /// <param name="transactionItem">The TransactionAddress you want to update the existing TransactionItem to</param>
        /// <param name="transactionItemId">The id of the TransactionItem you want to update</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<TransactionItem> UpdateTransactionItemAsync(string caseId, TransactionItem transactionItem, Guid transactionItemId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionItemUpdate, BaseUrl, caseId, transactionItemId);

            TransactionItem response = await PerformHttpCallAsync<TransactionItem>(uri, HttpMethod.Put, transactionItem, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a specific TransactionItem from a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Customer which you have already posted</param>
        /// <param name="transactionItemId">The Id of the TransactionItem you want to get</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<TransactionItem> GetTransactionItemAsync(string caseId, Guid transactionItemId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionItemGet, BaseUrl, caseId, transactionItemId);

            TransactionItem response = await PerformHttpCallAsync<TransactionItem>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Get a all the TransactionItems from a Transaction on a Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case with the Transaction which you have already posted</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<IList<TransactionItem>> GetTransactionItemsAsync(string caseId, string userName = "")
        {
            string uri = string.Format(Constants.UriTransactionItemsGet, BaseUrl, caseId);

            IList<TransactionItem> response = await PerformHttpCallAsync<IList<TransactionItem>>(uri, HttpMethod.Get, null, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// Post your KBAResult existing Case
        /// </summary>
        /// <param name="caseId">The Case Id of a Case</param>
        /// <param name="kbaResult">Your KBA Answers which you want to post</param>
        /// <param name="userName">Optional: The user name, needed to support multiple creds, if only one set of creds is used this parameter is not needed</param>
        /// <returns></returns>
        public static async Task<KBAResult> PostKBAResultAsync(string caseId, KBAResult kbaResult, string userName = "")
        {
            string uri = string.Format(Constants.UriKBAResultPost, BaseUrl, caseId);

            KBAResult response = await PerformHttpCallAsync<KBAResult>(uri, HttpMethod.Post, kbaResult, true, HttpRequestTimeout, userName);

            return response;
        }

        /// <summary>
        /// This method asynchronously performs the Http Request to the Trustev API
        /// </summary>
        /// <typeparam name="T">The Type of the return object</typeparam>
        /// <param name="uri">The HttpMethod Uri</param>
        /// <param name="method">The Http Method</param>
        /// <param name="entity">The relevant entity</param>
        /// <param name="isAuthenticationNeeded">Does this api call require the X-Authorization header</param>
        /// <param name="requestTimeout">The timeout value of this http request in milliseconds</param>
        /// <returns></returns>
        private static async Task<T> PerformHttpCallAsync<T>(string uri, HttpMethod method, object entity, bool isAuthenticationNeeded = true, int requestTimeout = 15000, string userName = "")
        {
            JsonSerializerSettings jss = new JsonSerializerSettings();
            DefaultContractResolver dcr = new PrivateSetterContractResolver();
            jss.ContractResolver = dcr;

            HttpClient client = new HttpClient();

            if (isAuthenticationNeeded)
            {
                var credential = GetCredential(userName);
                if (uri.Contains("/session"))
                {
                    if (string.IsNullOrEmpty(credential.PublicKey))
                        throw new TrustevGeneralException("You need to set your public key if you want to post a Session. This can be done via the SetUp method");

                    client.DefaultRequestHeaders.Add("X-PublicKey", credential.PublicKey);
                }
                else
                {
                    var token = await GetTokenAsync(credential.UserName);
                    client.DefaultRequestHeaders.Add("X-Authorization", credential.UserName + " " + token);
                }
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            client.Timeout = new TimeSpan(requestTimeout * TimeSpan.TicksPerMillisecond);

            HttpResponseMessage response = new HttpResponseMessage();

            string json = string.Empty;

            if (entity != null && entity.GetType() != typeof(string))
            {
                json = JsonConvert.SerializeObject(entity, jss);
            }
            else
            {
                json = (string)entity;
            }

            if (method == HttpMethod.Post)
            {
                response = await client.PostAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
            }
            else if (method == HttpMethod.Put)
            {
                response = await client.PutAsync(uri, new StringContent(json, Encoding.UTF8, "application/json"));
            }
            else if (method == HttpMethod.Get)
            {
                response = await client.GetAsync(uri);
            }

            string resultstring = string.Empty;

            if (response.IsSuccessStatusCode)
            {
                resultstring = await response.Content.ReadAsStringAsync();
            }
            else
            {
                resultstring = await response.Content.ReadAsStringAsync();

                throw new TrustevHttpException(response.StatusCode, resultstring);
            }

            return JsonConvert.DeserializeObject<T>(resultstring, jss);
        }


        /// <summary>
        /// Gets value of APIToken.
        /// Determins whether or not to generate a token on each request or reuse the current one if
        /// its valid.
        /// </summary>
        /// <param name="regenerateToken"></param>
        /// <returns></returns>
        private static async Task<string> GetTokenAsync(string userName)
        {
            var authenticationToken = GetApiToken(userName);

            if (!RegenerateTokenOnEachRequest)
            {
                if (authenticationToken == null || string.IsNullOrEmpty(authenticationToken.APIToken) || authenticationToken.ExpireAt == null || authenticationToken.ExpireAt.Value >= DateTime.UtcNow)
                {
                    await SetTokenAsync(userName);
                }
            }
            else
            {
                await SetTokenAsync(userName);
            }

            authenticationToken = GetApiToken(userName);

            return authenticationToken.APIToken;
        }

        /// <summary>
        /// Sets new APIToken and ExpiryDate on each call
        /// </summary>
        /// <returns></returns>
        private static async Task SetTokenAsync(string userName)
        {
            var credential = GetCredential(userName);

            CheckCredentials(credential);

            DateTime currentTime = DateTime.UtcNow;

            TokenRequest requestObject = new TokenRequest()
            {
                UserName = credential.UserName,
                PasswordHash = PasswordHashHelper(credential.Password, credential.Secret, currentTime),
                UserNameHash = UserNameHashHelper(credential.UserName, credential.Secret, currentTime),
                TimeStamp = currentTime.ToString("o")
            };

            string requestJson = JsonConvert.SerializeObject(requestObject);

            string uri = string.Format("{0}/token", BaseUrl);

            var response = await PerformHttpCallAsync<TokenResponse>(uri, HttpMethod.Post, requestJson, false, HttpRequestTimeout, userName);

            AddOrUpdateToken(userName, response);
        }

        #region Private Methods
        /// <summary>
        /// Check that the user has set the TrustevClient Credentials correctly
        /// </summary>
        private static void CheckCredentials(SiteCredential credential)
        {
            if (credential == null || string.IsNullOrEmpty(credential.UserName) || string.IsNullOrEmpty(credential.Secret) || string.IsNullOrEmpty(credential.Password))
            {
                throw new TrustevGeneralException("You have not set your TrustevClient credentials correctly. You need to set these by calling the SetUp method on the TrustevClient Class providing your UserName, Password and Secret as the paramters before you can access the TrustevClient API");
            }
        }

        /// <summary>
        /// Has password, secret and timestamp for GetToken request
        /// </summary>
        /// <param name="password">Your Trustev Password</param>
        /// <param name="sharedsecret">Your Trustev Secret</param>
        /// <param name="timestamp">The current UTC timestamp</param>
        /// <returns></returns>
        private static string PasswordHashHelper(string password, string sharedsecret, DateTime timestamp)
        {
            sharedsecret = sharedsecret.Replace("\"", string.Empty);
            password = password.Replace("\"", string.Empty);
            return Create256Hash(Create256Hash(timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ") + "." + password) + "." + sharedsecret);
        }

        /// <summary>
        /// Has username, secret and timestamp for GetToken request
        /// </summary>
        /// <param name="username">Your Trustev UserName</param>
        /// <param name="sharedsecret">Your Trustev Secret</param>
        /// <param name="timestamp">The current UTC timestamp</param>
        /// <returns></returns>
        private static string UserNameHashHelper(string username, string sharedsecret, DateTime timestamp)
        {
            sharedsecret = sharedsecret.Replace("\"", string.Empty);
            username = username.Replace("\"", string.Empty);
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
            string result = string.Empty;
            foreach (byte b in data)
            {
                result += b.ToString("X2");
            }

            result = result.ToLower();

            return result;
        }

        private static T DeepClone<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (T)formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// TrustevClient Token Response Object
        /// </summary>
        public class TokenResponse
        {
            public string APIToken { get; set; }

            public int CredentialType { get; set; }

            public DateTime ExpireAt { get; set; }
        }

        /// <summary>
        /// TrustevClient Token request Object
        /// </summary>
        private class TokenRequest
        {
            public string UserName { get; set; }

            public string PasswordHash { get; set; }

            public string UserNameHash { get; set; }

            public string TimeStamp { get; set; }
        }

        private class PrivateSetterContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                var prop = base.CreateProperty(member, memberSerialization);

                if (!prop.Writable)
                {
                    var property = member as PropertyInfo;
                    if (property != null)
                    {
                        var hasPrivateSetter = property.GetSetMethod(true) != null;
                        prop.Writable = hasPrivateSetter;
                    }
                }

                return prop;
            }
        }
        #endregion
    }
}