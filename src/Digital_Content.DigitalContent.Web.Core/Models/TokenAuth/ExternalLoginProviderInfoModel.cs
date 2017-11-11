using Abp.AutoMapper;
using Digital_Content.DigitalContent.Web.Authentication.External;

namespace Digital_Content.DigitalContent.Web.Models.TokenAuth
{
    [AutoMapFrom(typeof(ExternalLoginProviderInfo))]
    public class ExternalLoginProviderInfoModel
    {
        public string Name { get; set; }

        public string ClientId { get; set; }
    }
}
