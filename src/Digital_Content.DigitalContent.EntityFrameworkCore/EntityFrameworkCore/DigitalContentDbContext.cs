using Abp.IdentityServer4;
using Abp.Zero.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Digital_Content.DigitalContent.Authorization.Roles;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Chat;
using Digital_Content.DigitalContent.Editions;
using Digital_Content.DigitalContent.Friendships;
using Digital_Content.DigitalContent.MultiTenancy;
using Digital_Content.DigitalContent.MultiTenancy.Payments;
using Digital_Content.DigitalContent.Storage;
using Digital_Content.DigitalContent.Authorization.Companies;
using Digital_Content.DigitalContent.Authorization.Companies.Dto;
using Digital_Content.DigitalContent.Wallets;
using Digital_Content.DigitalContent.ExchangeRates;
using Digital_Content.DigitalContent.CreditCardPayments;
using Digital_Content.DigitalContent.TermsOfServices;
using Digital_Content.DigitalContent.Payments;
using Digital_Content.DigitalContent.PaymentTransaction;
using Digital_Content.DigitalContent.Currencies;
using Digital_Content.DigitalContent.Roadmap.Entity;

namespace Digital_Content.DigitalContent.EntityFrameworkCore
{
    public class DigitalContentDbContext : AbpZeroDbContext<Tenant, Role, User, DigitalContentDbContext>, IAbpPersistedGrantDbContext
    {
        /* Define an IDbSet for each entity of the application */

        public virtual DbSet<BinaryObject> BinaryObjects { get; set; }

        public virtual DbSet<Friendship> Friendships { get; set; }

        public virtual DbSet<ChatMessage> ChatMessages { get; set; }

        public virtual DbSet<SubscribableEdition> SubscribableEditions { get; set; }

        public virtual DbSet<SubscriptionPayment> SubscriptionPayments { get; set; }

        public DbSet<PersistedGrantEntity> PersistedGrants { get; set; }

        public virtual DbSet<Company> Companies { get; set; }

        public virtual DbSet<Transaction> WalletLogs { get; set; }

        public virtual DbSet<ExchangeRate> ExchangeRates { get; set; }

        public virtual DbSet<CreditCardPayment> CreditCardPayments { get; set; }

        public virtual DbSet<PaymentOutcome> Outcomes { get; set; }

        public virtual DbSet<PaymentSource> Sources{ get; set; }

        public virtual DbSet<TermsOfService> TermsOfServices { get; set; }

        public virtual DbSet<Payment> Payments { get; set; }

        public virtual DbSet<Currency> Currencies { get; set; }

        public virtual DbSet<SupportedCurrency> SupportedCurrencies { get; set; }

        public virtual DbSet<RoadmapRecord> RoadmapRecords { get; set; }

        public virtual DbSet<CompanyToken> CompanyTokens { get; set; }
        public DigitalContentDbContext(DbContextOptions<DigitalContentDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BinaryObject>(b =>
            {
                b.HasIndex(e => new { e.TenantId });
            });

            modelBuilder.Entity<ChatMessage>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId, e.ReadState });
                b.HasIndex(e => new { e.TenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.TargetUserId, e.ReadState });
                b.HasIndex(e => new { e.TargetTenantId, e.UserId, e.ReadState });
            });

            modelBuilder.Entity<Friendship>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.FriendUserId });
                b.HasIndex(e => new { e.FriendTenantId, e.UserId });
                b.HasIndex(e => new { e.FriendTenantId, e.FriendUserId });
            });

            modelBuilder.Entity<Tenant>(b =>
            {
                b.HasIndex(e => new { e.SubscriptionEndDateUtc });
                b.HasIndex(e => new { e.CreationTime });
            });

            modelBuilder.Entity<SubscriptionPayment>(b =>
            {
                b.HasIndex(e => new { e.Status, e.CreationTime });
                b.HasIndex(e => new { e.PaymentId, e.Gateway });
            });


            modelBuilder.Entity<PaymentTransaction.PaymentTransaction>()
           .HasKey(t => new { t.PaymentId, t.TransactionId });

            modelBuilder.Entity<PaymentTransaction.PaymentTransaction>()
                .HasOne(pt => pt.Payment)
                .WithMany(p => p.PaymentTransaction)
                .HasForeignKey(pt => pt.PaymentId);

            modelBuilder.Entity<PaymentTransaction.PaymentTransaction>()
                .HasOne(pt => pt.Transaction)
                .WithMany(t => t.PaymentTransaction)
                .HasForeignKey(pt => pt.TransactionId);

            modelBuilder.ConfigurePersistedGrantEntity();
        }
    }
}
