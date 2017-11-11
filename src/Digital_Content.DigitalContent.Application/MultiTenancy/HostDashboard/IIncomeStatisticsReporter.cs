using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Digital_Content.DigitalContent.MultiTenancy.HostDashboard.Dto;

namespace Digital_Content.DigitalContent.MultiTenancy.HostDashboard
{
    public interface IIncomeStatisticsService
    {
        Task<List<IncomeStastistic>> GetIncomeStatisticsData(DateTime startDate, DateTime endDate,
            ChartDateInterval dateInterval);
    }
}