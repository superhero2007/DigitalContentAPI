using Digital_Content.DigitalContent.Authorization.Companies.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Authorization.Companies
{
    public interface ICompanyManager
    {
        Task<long?> CreateCompany(CompanyInput input);
    }
}
