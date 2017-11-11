using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Authorization.Companies.Dto
{
    [AutoMapFrom(typeof(Company))]
    public class CompanyInput : EntityDto
    {
        public CompanyInput()
        {

        }
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
