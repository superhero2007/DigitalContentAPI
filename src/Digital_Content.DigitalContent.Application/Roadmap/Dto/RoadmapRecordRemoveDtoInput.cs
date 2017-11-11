using System;
using Abp.AutoMapper;
using Digital_Content.DigitalContent.Roadmap.Entity;
using System.ComponentModel.DataAnnotations;

namespace Digital_Content.DigitalContent.Roadmap.Dto
{
    public class RoadmapRecordRemoveDtoInput
    {
        [Required]
        public long Id { get; set; }
    }
}