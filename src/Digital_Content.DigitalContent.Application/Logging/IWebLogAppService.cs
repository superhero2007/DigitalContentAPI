using Abp.Application.Services;
using Digital_Content.DigitalContent.Dto;
using Digital_Content.DigitalContent.Logging.Dto;

namespace Digital_Content.DigitalContent.Logging
{
    public interface IWebLogAppService : IApplicationService
    {
        GetLatestWebLogsOutput GetLatestWebLogs();

        FileDto DownloadWebLogs();
    }
}
