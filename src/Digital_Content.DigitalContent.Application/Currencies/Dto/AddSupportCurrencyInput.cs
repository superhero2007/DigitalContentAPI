using System;
using Abp.AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Currencies.Dto
{
    public class AddSupportCurrencyInput
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string WalletId { get; set; }

        public string WiredInstruction { get; set; }
    }
}