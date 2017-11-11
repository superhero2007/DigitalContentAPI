using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Currencies.Dto
{
    public class ICOInput
    {
        public string CompanyTenant { get; set; } // Company => Tenant Name.

        public string TokenName { get; set; }

        public string TokenDescription { get; set; }

        public string WiredInstruction { get; set; }
    }
}
