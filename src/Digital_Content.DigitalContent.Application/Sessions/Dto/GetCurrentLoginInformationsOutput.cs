using Digital_Content.DigitalContent.TermsOfServices.Dto;

namespace Digital_Content.DigitalContent.Sessions.Dto
{
    public class GetCurrentLoginInformationsOutput
    {
        public UserLoginInfoDto User { get; set; }

        public TenantLoginInfoDto Tenant { get; set; }

        public ApplicationInfoDto Application { get; set; }

        public TermsOfServiceOutput Tos { get; set; }
    }
}