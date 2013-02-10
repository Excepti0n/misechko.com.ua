using System.ComponentModel.DataAnnotations;

namespace misechko.com.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Ім'я користувача")]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Pazz { get; set; }
    }
}