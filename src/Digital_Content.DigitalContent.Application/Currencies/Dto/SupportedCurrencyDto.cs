using System;
using Abp.AutoMapper;

namespace Digital_Content.DigitalContent.Currencies.Dto
{
    public class SupportedCurrencyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ImageUrl { get; set; }

        public string WalletId { get; set; }

        public string WiredInstruction { get; set; }

        public int? SortOrder { get; set; }

        public SupportedCurrencyDto(Guid id, string name, string code, string imageUrl, string walletId, string wiredInstruction, int? sortOrder = null)
        {
            Id = id;
            Name = name;
            Code = code;
            ImageUrl = imageUrl;
            WalletId = walletId;
            WiredInstruction = wiredInstruction;
            SortOrder = sortOrder;
        }
    }
}