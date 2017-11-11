using System.Collections.Generic;

namespace Digital_Content.DigitalContent.MultiTenancy.HostDashboard.Dto
{
    public class GetRecentTenantsOutput
    {
        public List<RecentTenant> RecentTenants;

        public GetRecentTenantsOutput(List<RecentTenant> recentTenants)
        {
            RecentTenants = recentTenants;
        }
    }
}