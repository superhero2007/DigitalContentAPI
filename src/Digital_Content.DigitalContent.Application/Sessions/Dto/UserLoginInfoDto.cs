using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using Digital_Content.DigitalContent.Authorization.Users;

namespace Digital_Content.DigitalContent.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }

        public string ProfilePictureId { get; set; }
    }
}
