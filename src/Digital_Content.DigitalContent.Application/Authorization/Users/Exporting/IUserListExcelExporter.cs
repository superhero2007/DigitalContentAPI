using System.Collections.Generic;
using Digital_Content.DigitalContent.Authorization.Users.Dto;
using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.Authorization.Users.Exporting
{
    public interface IUserListExcelExporter
    {
        FileDto ExportToFile(List<UserListDto> userListDtos);
    }
}