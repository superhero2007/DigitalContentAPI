using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.Common.Dto
{
    public class FindUsersInput : PagedAndFilteredInputDto
    {
        public int? TenantId { get; set; }
    }
}