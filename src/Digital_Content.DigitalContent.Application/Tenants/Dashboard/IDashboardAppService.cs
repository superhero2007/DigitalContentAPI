using Digital_Content.DigitalContent.Tenants.Dashboard.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Tenants.Dashboard
{
    public interface IDashboardAppService
    {
        Task<DasboardDataDto> GetDashboardDataForUser();
    }
}
