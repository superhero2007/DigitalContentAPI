using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Authorization.Users;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.TermsOfServices
{
    [Table("TermsOfService")]
    public class TermsOfService : Entity<long>,IMayHaveTenant
    {
        public int? TenantId { get; set; }

        public string TosContent { get; set; }

        public DateTime DateRevised { get; set; }

        public User RevicedBy { get; set; }

        public bool IsDefault { get; set; }

        public int Version { get; set; }


    }
}
