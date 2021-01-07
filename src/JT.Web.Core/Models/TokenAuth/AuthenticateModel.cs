using Abp.Auditing;
using JT.Abp.Authorization.Users;
using System.ComponentModel.DataAnnotations;

namespace JT.Web.Core.Models.TokenAuth
{
    public class AuthenticateModel
    {
        [Required]
        [StringLength(JTUserBase.MaxEmailAddressLength)]
        public string UserNameOrEmailAddress { get; set; }

        [Required]
        [StringLength(JTUserBase.MaxPlainPasswordLength)]
        [DisableAuditing]
        public string Password { get; set; }

        public bool RememberClient { get; set; }
    }
}
