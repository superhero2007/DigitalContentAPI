using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Services;
using Digital_Content.DigitalContent.Authorization.Companies.Dto;
using Digital_Content.DigitalContent.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Authorization.Companies
{
    public class CompanyManager : DigitalContentDomainServiceBase, ICompanyManager, IDomainService
    {
       
        private readonly IRepository<Company, long> _companyRepository;


        public CompanyManager(
            IRepository<Company, long> companyRepository)
        {
            _companyRepository = companyRepository;
        }
        ///// <summary>
        ///// Create new Company
        ///// </summary>
        ///// <param name="input"></param>
        ///// <returns></returns>
        public async Task<long?> CreateCompany(CompanyInput input)
        {
            if (input != null)
            {
                var company = new Company
                {
                    Name = input.Name == null ? "Default": input.Name,
                    CreationTime = DateTime.Now
                };
                var existingCompany = await _companyRepository.FirstOrDefaultAsync(exCompany => 
                exCompany.Name.ToUpper().Trim().Equals(company.Name.ToUpper().Trim()));

                var companyId = (existingCompany != null) ? 
                    existingCompany.Id : 
                    await _companyRepository.InsertAndGetIdAsync(company);
                return companyId;
            }
            return null;
        }

    }
}
