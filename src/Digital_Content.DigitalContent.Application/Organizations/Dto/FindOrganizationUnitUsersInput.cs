using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.Organizations.Dto
{
    public class FindOrganizationUnitUsersInput : PagedAndFilteredInputDto
    {
        public long OrganizationUnitId { get; set; }
    }
}
