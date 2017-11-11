using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.Authorization.Users;
using Abp.Extensions;
using Digital_Content.DigitalContent.Authorization.Users;
using Digital_Content.DigitalContent.Validation;
using Abp.Runtime.Validation;

namespace Digital_Content.DigitalContent.Authorization.Accounts.Dto
{
    public class RegisterInput
    {
        public RegisterInput(){}
        [Required]
        [StringLength(User.MaxNameLength)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(User.MaxNameLength)]
        public string LastName { get; set; }

        [StringLength(User.MaxNameLength)]
        public string UserName { get; set; }


        [StringLength(User.MaxNameLength)]
        public string YourName { get; set; }

        public string ContentType { get; set; }

        
        public string ContentVolume { get; set; }

       
        public string Industry { get; set; }

   
        public bool IsContentShedule { get; set; }

        
        public string CMS { get; set; }

        public string Website { get; set; }

        
        public string Company { get; set; }

        //[Required]
        public string UserType { get; set; }

     
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(AbpUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }


        [Required]
        [StringLength(User.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }
    }
}