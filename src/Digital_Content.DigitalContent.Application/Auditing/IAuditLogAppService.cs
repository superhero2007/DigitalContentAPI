using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Digital_Content.DigitalContent.Auditing.Dto;
using Digital_Content.DigitalContent.Dto;

namespace Digital_Content.DigitalContent.Auditing
{
    public interface IAuditLogAppService : IApplicationService
    {
        Task<PagedResultDto<AuditLogListDto>> GetAuditLogs(GetAuditLogsInput input);

        Task<FileDto> GetAuditLogsToExcel(GetAuditLogsInput input);
    }
}