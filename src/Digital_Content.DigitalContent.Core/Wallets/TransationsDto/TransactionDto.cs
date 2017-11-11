using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Currencies;
using Digital_Content.DigitalContent.Payments;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Wallets.TransationsDto
{
    [AutoMapFrom(typeof(Transaction))]
    public class TransactionDto: EntityDto<long>, IPassivable, IHasCreationTime
    { 
        //private string _currnecyType { get; set; }

        public long? UserId { get; set; }

        public string WalletId { get; set; }

        public string AmountDue { get; set; }

        //TODO: old
        //public string CurrencyType {
        //    get
        //    {
        //        return this._currnecyType;
        //    }
        //    set
        //    {
        //        this._currnecyType = ((CurrentTypeEnum)Convert.ToInt32(value)).ToString();
        //    }
        //}
        //(BTC | ETH)

        public Currency Currency { get; set; }

        public string Address { get; set; }

        public DateTime CreationTime { get; set; }

        public string TransactionStatus { get; set; }

        public bool IsDeleted { get; set; }

        public bool IsActive { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime TransactionDate { get; set; }

        public long TokensIssued { get; set; }

        public long PaymentAmount { get; set; }

        public string EmailAddress { get; set; }
    }
}
