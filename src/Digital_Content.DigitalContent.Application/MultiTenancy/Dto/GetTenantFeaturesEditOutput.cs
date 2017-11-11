using System.Collections.Generic;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Editions.Dto;

namespace Digital_Content.DigitalContent.MultiTenancy.Dto
{
    public class GetTenantFeaturesEditOutput
    {
        public List<NameValueDto> FeatureValues { get; set; }

        public List<FlatFeatureDto> Features { get; set; }
    }
}