using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trustev.Domain
{
    public class Enums
    {
        public enum DecisionResult
        {
            /// <summary>
            /// This result should not be returned to you. It means that an error has occurred and a Trustev Decision has not been made on your Trustev Case.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// This result indicates that the Trustev Case shows no signs for suspicion and the 'transaction' should be accepted.
            /// </summary>
            Pass,

            /// <summary>
            /// This result indicates that the Trustev Case contains elements for suspicion which should be reviewed before a final decision is made.
            /// </summary>
            Flag,

            /// <summary>
            /// This result indicates that the Trustev Case contains a number of fraudulent features and the 'transaction' should be rejected.
            /// </summary>
            Fail
        }

        public enum MarketType
        {
            /// <summary>
            /// A Default MarketType
            /// </summary>
            Default = 0,

            /// <summary>
            /// Unsecured Personal Loans Market Type
            /// </summary>
            UnsecuredPersonalLoans = 1,

            /// <summary>
            /// Credit Cards Market Type
            /// </summary>
            CreditCards = 2,

            /// <summary>
            /// Multi-Family Rental Screening Market Type
            /// </summary>
            MultiFamilyRentalScreening = 3,

            /// <summary>
            /// Auto Lending Market Type
            /// </summary>
            AutoLending = 4,

            /// <summary>
            /// Short Term Alternative Lending Market Type
            /// </summary>
            ShortTermAlternativeLending = 5,

            /// <summary>
            /// Telecom and Communications Market Type
            /// </summary>
            TelecomAndCommunications = 6,

            /// <summary>
            /// Insurance Market Type
            /// </summary>
            Insurance = 7,

            /// <summary>
            /// Mortgage Market Type
            /// </summary>
            Mortgage = 8
        }

        public enum CaseStatusType
        {
            /// <summary>
            /// Order Completed
            /// </summary>
            Completed,

            /// <summary>
            /// Order Rejected due to Fraud
            /// </summary>
            RejectedFraud,

            /// <summary>
            /// Order Rejected due to Card Authentication Failure
            /// </summary>
            RejectedAuthFailure,

            /// <summary>
            /// Order Rejected due to suspect Fraud
            /// </summary>
            RejectedSuspicious,

            /// <summary>
            /// Order Cancelled
            /// </summary>
            Cancelled,

            /// <summary>
            /// Return of funds to customer due to Fraud
            /// </summary>
            ChargebackFraud,

            /// <summary>
            /// Return of funds to customer for other reasons
            /// </summary>
            ChargebackOther,

            /// <summary>
            /// Customer refunded
            /// </summary>
            Refunded,

            /// <summary>
            /// Order has been placed on system
            /// </summary>
            Placed,

            /// <summary>
            /// Order is under review, no decision made yet
            /// </summary>
            OnHoldReview,

            /// <summary>
            /// Order deemed fraudulent after it was fulfilled
            /// </summary>
            ReportedFraud = 12,

            /// <summary>
            /// Order deemed fraudulent after it was fulfilled
            /// </summary>
            Ato,

            /// <summary>
            /// Order deemed fraudulent after it was fulfilled
            /// </summary>
            AtoChargeback = 14
        }

        public enum PaymentType
        {
            /// <summary>
            /// None Payment Type
            /// </summary>
            None,

            /// <summary>
            /// Credit Card Payment
            /// </summary>
            CreditCard,

            /// <summary>
            /// Debit Card Payment
            /// </summary>
            DebitCard,

            /// <summary>
            /// Direct Debit Payment
            /// </summary>
            DirectDebit,

            /// <summary>
            /// PayPal Payment
            /// </summary>
            PayPal,

            /// <summary>
            /// BitCoin Payment
            /// </summary>
            Bitcoin,

            /// <summary>
            /// Due Payment
            /// </summary>
            Due,

            /// <summary>
            /// Stripe Payment
            /// </summary>
            Stripe,

            /// <summary>
            /// Dwolla Payment
            /// </summary>
            Dwolla,

            /// <summary>
            /// ApplePay Payment
            /// </summary>
            ApplePay,

            /// <summary>
            /// Payoneer Payment
            /// </summary>
            Payoneer,

            /// <summary>
            /// TwoCheckout Payment
            /// </summary>
            TwoCheckout,

            /// <summary>
            /// AmazonPay Payment
            /// </summary>
            AmazonPay,

            /// <summary>
            /// Square Payment
            /// </summary>
            Square,

            /// <summary>
            /// AndroidPay Payment
            /// </summary>
            AndroidPay,

            /// <summary>
            /// LeoPay Payment
            /// </summary>
            LeoPay,

            /// <summary>
            /// GooglePay Payment
            /// </summary>
            GooglePay,

            /// <summary>
            /// SamsungPay Payment
            /// </summary>
            SamsungPay,

            /// <summary>
            /// MasterPass Payment
            /// </summary>
            MasterPass,

            /// <summary>
            /// MicrosoftPay Payment
            /// </summary>
            MicrosoftPay,

            /// <summary>
            /// Other (Non Defined) Payment
            /// </summary>
            Other
        }
        /// <summary>
        /// This enum represents the ADR Status
        /// </summary>
        [DefaultValue(Started)]
        public enum ADRStatus
        {
            /// <summary>
            /// The flow has started
            /// </summary>
            Started = 0,

            /// <summary>
            /// Success
            /// </summary>
            Success = 1,

            /// <summary>
            /// NO details available
            /// </summary>
            NoDetailsAvailable = 2,

            /// <summary>
            /// The case is invalid
            /// </summary>
            InvalidCase = 3,

            /// <summary>
            /// The case is not eligible for ADR
            /// </summary>
            Ineligible = 4,

            /// <summary>
            /// Error
            /// </summary>
            Error = 5,

            /// <summary>
            /// The feature is misconfigured
            /// </summary>
            Misconfigured = 6,

            /// <summary>
            /// No consent was given
            /// </summary>
            NoConsent = 7
        }

        public enum CaseType
        {
            Default,
            AccountCreation = 2,
            Application,
            ADR,
            Login
        }

        public enum SessionType
        {
            JavaScript = 0,
            Mobile = 1,
            Broker = 2
        }

        public enum AddressType
        {
            /// <summary>
            /// Standard Address Type
            /// </summary>
            Standard,

            /// <summary>
            /// Billing Address Type
            /// </summary>
            Billing,

            /// <summary>
            /// Delivery Address Type
            /// </summary>
            Delivery
        }
         /// <summary>
        /// TimeToFulfilment
        /// </summary>
        public enum TimeToFulfilment
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined,
            /// <summary>
            /// Immediate
            /// </summary>
            Immediate,
            /// <summary>
            /// Same Day
            /// </summary>
            SameDay,
            /// <summary>
            ///  Next Day
            /// </summary>
            NextDay,
            /// <summary>
            /// Up To 3 Days,
            /// </summary>
            UpTo3Days,
            /// <summary>
            /// Up To 5 Days
            /// </summary>
            UpTo5Days
        }

        /// <summary>
        /// Fulfilment Method
        /// </summary>
        public enum FulfilmentMethod
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined,
            /// <summary>
            /// Virtual
            /// </summary>
            Virtual,
            /// <summary>
            /// In Person
            /// </summary>
            InPerson,
            /// <summary>
            /// Post
            /// </summary>
            Post,
            /// <summary>
            /// Courier
            /// </summary>
            Courier
        }

        /// <summary>
        /// Fulfilment GeoLocation
        /// </summary>
        public enum FulfilmentGeoLocation
        {
            /// <summary>
            /// Undefined
            /// </summary>
            Undefined,
            /// <summary>
            /// National Location
            /// </summary>
            National,
            /// <summary>
            /// International Location
            /// </summary>
            International
        }

        public enum OTPStatus
        {
            /// <summary>
            /// Case is Eligible and OTP Offered 
            /// </summary>
            Offered = 0,

            /// <summary>
            /// OTP Offered And Passed
            /// </summary>
            Pass = 1,

            /// <summary>
            /// OTP Offered And Failed
            /// </summary>
            Fail = 2,

            /// <summary>
            /// Case is Ineligible and OTP was not Offered 
            /// </summary>
            Ineligible = 3,

            /// <summary>
            /// OTP Offered and code sent 
            /// </summary>
            InProgress = 4,

            /// <summary>
            /// Hit max retries 
            /// </summary>
            MaxRetryHit = 5,

            /// <summary>
            /// Final state of Abandoned
            /// </summary>
            Abandoned = 6,

            /// <summary>
            /// OTP is not Configured
            /// </summary>
            NotConfigured = 7
        }
        public enum PhoneDeliveryType
        {
            Sms,
            Voice
        }
        public enum OTPLanguageEnum
        {
            /// <summary>
            /// English
            /// </summary>
            EN = 0,

            /// <summary>
            /// Spanish
            /// </summary>
            ES = 1
        }

        public enum BaseUrl
        {
            EU,
            US,
            Local
        }

        /// <summary>
        /// Enum for the KBAData Status Codes
        /// </summary>
        public enum KBAStatus
        {
            /// <summary>
            /// KBA Status is NotConfigured
            /// </summary>
            NotConfigured = -1,

            /// <summary>
            /// KBA Status is Offered
            /// </summary>
            Offered = 0,

            /// <summary>
            /// KBA Status is MultiPassOffered
            /// </summary>
            MultiPassOffered = 1,
            /// <summary>
            /// KBA Status is Ineligible
            /// </summary>
            Ineligible = 2,

            /// <summary>
            /// KBA Status is NoData
            /// </summary>
            NoData = 3,

            /// <summary>
            /// KBA Status is Passed
            /// </summary>
            Passed = 4,

            /// <summary>
            /// KBA Status is Failed
            /// </summary>
            Failed = 5,

            /// <summary>
            /// KBA Status is Abbandoned
            /// </summary>
            Abbandoned = 6,
        }

        /// <summary>
        /// Enum for the KBAData MultiPass Status Codes
        /// </summary>
        public enum MultiPassStatus
        {
            /// <summary>
            /// MultiPass is not Enabled or configured
            /// </summary>
            NotEnabled = -1,

            /// <summary>
            /// MultiPass is not Used
            /// </summary>
            NotUsed = 0,

            /// <summary>
            /// MultiPass is Used
            /// </summary>
            Used = 1,

            /// <summary>
            /// MultiPass is in use
            /// </summary>
            Using = 2,
        }
    }
}
