using System.ComponentModel.DataAnnotations;

namespace JT.UserService
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}