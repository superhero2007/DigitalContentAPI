using System;
using Abp.AutoMapper;
using Digital_Content.DigitalContent.Roadmap.Entity;
using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Roadmap.Dto
{
    public class RoadmapRecordUpdateDtoInput
    {
        [Required]
        public long Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Description { get; set; }

        public decimal ExchangeRate { get; set; }

        public long MinTokenToBuy { get; set; }

        public decimal Bonus { get; set; }

        public bool IncludeFeatureYears { get; set; }
    }
}