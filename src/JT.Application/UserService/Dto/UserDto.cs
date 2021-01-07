using System;
using System.ComponentModel.DataAnnotations;
using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using JT.Abp.Authorization.Users;
using JT.Authorization.Users;

namespace JT.UserService
{
    [AutoMapFrom(typeof(User))]
    public class UserDto : EntityDto<long>
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

        public string FullName { get; set; }

        public DateTime? LastLoginTime { get; set; }

        public DateTime CreationTime { get; set; }

        public string[] RoleNames { get; set; }
    }
}
