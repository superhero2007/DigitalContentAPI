using System;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Currencies.Dto
{
    public class UpdateSupportCurrencyInput
    {
        [Required]
        public Guid Id { get; set; }

        public string WalletId { get; set; }

        public string WiredInstruction { get; set; }

        public int? SortOrder { get; set; }
    }
}