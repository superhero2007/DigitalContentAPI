using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.MultiTenancy
{
    public class CompanyToken :Entity<int>, IMayHaveTenant
    {
        public int? TenantId { get; set; }
        public string TokenName { get; set; }
        public decimal TokenPrice { get; set; }
    }
}
