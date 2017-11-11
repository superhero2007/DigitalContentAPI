using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Chat;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Digital_Content.DigitalContent.Authorization.Companies.Dto
{   
    [Table("Company")]
    public class Company : Entity<long>, IHasCreationTime
    {
        public Company()
        {
            this.Users = new List<User>();
        }
        [Required]
        public string Name { get; set; }

        public DateTime CreationTime { get; set; }

        [ForeignKey("CompanyUserId")]
        public virtual ICollection<User> Users { get; set; }
    }   
}

