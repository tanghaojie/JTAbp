using System.ComponentModel.DataAnnotations;
using Abp.Auditing;
using Abp.AutoMapper;
using Abp.Runtime.Validation;
using JT.Abp.Authorization.Users;
using JT.Authorization.Users;

namespace JT.UserService
{
    [AutoMapTo(typeof(User))]
    public class CreateUserDto : IShouldNormalize
    {
        [Required]
        [StringLength(JTUserBase.MaxUserNameLength)]
        public string UserName { get; set; }

        [Required]
        [StringLength(JTUserBase.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(JTUserBase.MaxSurnameLength)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(JTUserBase.MaxEmailAddressLength)]
        public string EmailAddress { get; set; }

        public bool IsActive { get; set; }

        public string[] RoleNames { get; set; }

        [Required]
        [StringLength(JTUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public void Normalize()
        {
            if (RoleNames == null)
            {
                RoleNames = new string[0];
            }
        }
    }
}
