using System.Collections.Generic;
using Digital_Content.DigitalContent.Auditing.Dto;
using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.Auditing.Exporting
{
    public interface IAuditLogListExcelExporter
    {
        FileDto ExportToFile(List<AuditLogListDto> auditLogListDtos);
    }
}
