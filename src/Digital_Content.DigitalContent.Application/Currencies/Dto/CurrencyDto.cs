using System;
using Abp.AutoMapper;

namespace Digital_Content.DigitalContent.Currencies.Dto
{
    [AutoMap(typeof(Currency))]
    public class CurrencyDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Code { get; set; }

        public string ImageUrl { get; set; }

        public int? SortOrder { get; set; }
    }
}