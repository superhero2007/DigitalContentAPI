using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Wallets
{
    [Table("Transactions")]
    public class Transaction : Entity<long>, IHasCreationTime, ISoftDelete, IMayHaveTenant
    {
        public long? UserId { get; set; }

        public string WalletId { get; set; }

        public decimal AmountDue { get; set; }

        //TODO old public int CurrencyType { get; set; }
        //(BTC | ETH)

        public Guid CurrencyId { get; set; }

        public string Address { get; set; }

        public DateTime CreationTime { get; set; }

        public TransactionStatusEnum TransactionStatus { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime TransactionDate { get; set; }

        public long TokensIssued { get; set; }

        public long PaymentAmount { get; set; }

        public string EmailAddress { get; set; }
        
        public virtual ICollection<PaymentTransaction.PaymentTransaction> PaymentTransaction { get; set; }
        public int? TenantId { get; set; }
    }

    //public enum CurrentTypeEnum : int
    //{
    //    btc = 0,
    //    eth,
    //    dcnt,
    //    usd,
    //    ltc
    //}

    public enum TransactionStatusEnum : int
    {
        Paid = 0,
        PartiallyPaid,
        Commited,
        Rescind
    }
}