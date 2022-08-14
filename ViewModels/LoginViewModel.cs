using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Alcantara.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "SharedRequired")]
        [MaxLength(50,ErrorMessage = "MaxLenght")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "InvalidEmail")]
        public string LoginEmail { get; set; }

        [Required(ErrorMessage = "SharedRequired")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
        public string LoginPassword { get; set; }

        [Display(Name = "Remember")]
        public bool Remember { get; set; }
    }
}
