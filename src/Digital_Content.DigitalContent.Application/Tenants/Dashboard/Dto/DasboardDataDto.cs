using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Tenants.Dashboard.Dto
{
    public class DasboardDataDto
    {
        public decimal UserDcntTokensPurchased { get; set; }

        public decimal UserDownline { get; set; }

        public decimal UserDownlineTokenBough { get; set; }

        public decimal UserAffricateTokensEarned { get; set; }

        public decimal UserDcntTokensBalance { get; set; }

        public decimal TotalDcntInvestors { get; set; }

        public decimal CurrentBonus { get; set; }

        public decimal TotalTokensPurchased { get; set; }
    }
}
