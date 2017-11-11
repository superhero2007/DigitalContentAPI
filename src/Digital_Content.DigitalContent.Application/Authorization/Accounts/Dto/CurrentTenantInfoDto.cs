using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Digital_Content.DigitalContent.MultiTenancy;

namespace Digital_Content.DigitalContent.Authorization.Accounts.Dto
{
    [AutoMapFrom(typeof(Tenant))]
    public class CurrentTenantInfoDto : EntityDto
    {
        public string TenancyName { get; set; }

        public string Name { get; set; }
    }
}