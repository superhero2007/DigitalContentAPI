using System;
using Abp.Authorization.Users;
using Abp.Extensions;
using Abp.Timing;
using System.ComponentModel.DataAnnotations.Schema;
using Digital_Content.DigitalContent.Authorization.Companies;
using Digital_Content.DigitalContent.Authorization.Companies.Dto;
using Digital_Content.DigitalContent.CreditCardPayments;
using System.Collections.Generic;
using Digital_Content.DigitalContent.TermsOfServices;

namespace Digital_Content.DigitalContent.Authorization.Users
{
    /// <summary>
    /// Represents a user in the system.
    /// </summary>
    public class User : AbpUser<User>
    {
        public const int MaxPhoneNumberLength = 24;

        public virtual Guid? ProfilePictureId { get; set; }

        public virtual bool ShouldChangePasswordOnNextLogin { get; set; }

        public DateTime? SignInTokenExpireTimeUtc { get; set; }

        public string SignInToken { get; set; }

        public string GoogleAuthenticatorKey { get; set; }

        public int? UserType { get; set; }

        [ForeignKey("CompanyUserId")]
        public virtual Company Company { get; set; }

        public long? CompanyUserId { get; set; }

        public string YourName { get; set; }

        public string Website { get; set; }

        public string ContentType { get; set; }

        public string ContentVolume { get; set; }

        public string Industry { get; set; }

        public bool IsContentShedule { get; set; }

        public string CMS { get; set; }

        public string Eth { get; set; }
        
        public string Waves { get; set; }

        public decimal  AmountTokens { get; set; }

        [ForeignKey("CreditCardUserId")]
        public virtual ICollection<CreditCardPayment> CreditCardPayments { get; set; }

        public decimal DcntTokensPurchased { get; set; }

        public decimal Downline { get; set; }

        public decimal DownlineTokenBough { get; set; }

        public decimal AffricateTokensEarned { get; set; }

        public decimal DcntTokensBalance { get; set; }

        public long? TosVersion { get; set; }

        public string CompanyTenant { get; set; } // Company => Tenant Name.

        public string TokenName { get; set; }

        public string TokenDescription { get; set; }

        public string WiredInstruction { get; set; }

        //Can add application specific user properties here

        public User()
        {
            IsLockoutEnabled = true;
            IsTwoFactorEnabled = true;
            this.CreditCardPayments = new List<CreditCardPayment>();
        }

        /// <summary>
        /// Creates admin <see cref="User"/> for a tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="emailAddress">Email address</param>
        /// <returns>Created <see cref="User"/> object</returns>
        public static User CreateTenantAdminUser(int tenantId, string emailAddress)
        {
            var user = new User
            {
                TenantId = tenantId,
                UserName = AdminUserName,
                Name = AdminUserName,
                Surname = AdminUserName,
                EmailAddress = emailAddress
            };

            user.SetNormalizedNames();

            return user;
        }

        public static string CreateRandomPassword()
        {
            return Guid.NewGuid().ToString("N").Truncate(16);
        }

        public override void SetNewPasswordResetCode()
        {
            /* This reset code is intentionally kept short.
             * It should be short and easy to enter in a mobile application, where user can not click a link.
             */
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(10).ToUpperInvariant();
        }

        public void Unlock()
        {
            AccessFailedCount = 0;
            LockoutEndDateUtc = null;
        }

        public void SetSignInToken()
        {
            SignInToken = Guid.NewGuid().ToString();
            SignInTokenExpireTimeUtc = Clock.Now.AddMinutes(1).ToUniversalTime();
        }
    }
    public enum UserType:Int32
    {
        publisher = 0,
        creator,
        tokenBuyer
    }
}