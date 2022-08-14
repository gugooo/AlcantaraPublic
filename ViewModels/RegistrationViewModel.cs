using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Alcantara.ViewModels
{
    public class RegistrationViewModel
    {
            [Required(ErrorMessage ="SharedRequired")]
            [Display(Name = "FName")]
            [MaxLength(50,ErrorMessage = "SharedMaxLenght")]
            [DataType(DataType.Text)]
            [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$",ErrorMessage ="ValidName")]
            public string FName { get; set; }

            [Required(ErrorMessage = "SharedRequired")]
            [Display(Name = "LName")]
            [MaxLength(50,ErrorMessage = "SharedMaxLenght")]
            [DataType(DataType.Text)]
            [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")] 
            public string LName { get; set; }

            [Display(Name = "Email")]
            [Required(ErrorMessage = "SharedRequired")]
            [EmailAddress(ErrorMessage = "InvalidEmail")]
            [Remote(action: "ChakEmail", controller: "Account", ErrorMessage = "EmailRemote")]
            public string Email { get; set; }

            [Required(ErrorMessage = "SharedRequired")]
            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
            public string Password { get; set; }

            [Required(ErrorMessage = "SharedRequired")]
            [DataType(DataType.Password)]
            [Compare("Password",ErrorMessage = "ComparePasswordErrMassage")]
            [Display(Name = "ComparePassword")]
            [RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
            public string ComparePassword { get; set; }
    }
}
