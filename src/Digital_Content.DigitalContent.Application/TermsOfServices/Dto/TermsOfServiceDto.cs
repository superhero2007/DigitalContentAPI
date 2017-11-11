using Digital_Content.DigitalContent.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.TermsOfServices.Dto
{
    public class TermsOfServiceDto
    {
        public int? TenantId { get; set; }

        public string TosContent { get; set; }

        public DateTime DateRevised { get; set; }

        public User RevicedBy { get; set; }

        public bool IsDefault { get; set; }
    }
}
