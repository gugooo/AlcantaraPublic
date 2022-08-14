using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Alcantara.ViewModels.MyAccount
{
    public class PersonalInformationViewModel
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

            [Display(Name = "DateOfBirth")]
            [DataType(DataType.Date,ErrorMessage = "DateOfBirthIncorrect")]
            public DateTime DateOfBirth { get; set; }

            [Display(Name = "Email")]
            [Required(ErrorMessage = "SharedRequired")]
            [EmailAddress(ErrorMessage = "InvalidEmail")]
            [Remote(action: "MyAccountChakEmail", controller: "Account", ErrorMessage = "EmailRemote")]
            public string Email { get; set; }

            [Display(Name = "Phone")]
            [DataType(DataType.PhoneNumber)]
            public string Phone { get; set; }

            [MaxLength(50,ErrorMessage = "SharedMaxLenght")]
            [DataType(DataType.Text)]
            [Display(Name = "Country")]
            [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")] 
            public string Country { get; set; }

            [MaxLength(50,ErrorMessage = "SharedMaxLenght")]
            [DataType(DataType.Text)]
            [Display(Name = "City")]
            [RegularExpression(@"^[А-Яа-яԱ-Ֆա-ֆA-Za-z-\s]{3,50}$", ErrorMessage = "ValidName")]         
            public string City { get; set; }

            [MaxLength(50,ErrorMessage = "SharedMaxLenght")]
            [DataType(DataType.Text)]
            [Display(Name = "Adress")]
            public string Adress { get; set; }

            [Display(Name = "Postcode")]
            [DataType(DataType.PostalCode)]
            public string Postcode { get; set; }

            [Display(Name = "Password")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password",ErrorMessage = "ComparePasswordErrMassage")]
            [Display(Name = "ComparePassword")]
            [RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
            public string ComparePassword { get; set; }

            [Required(ErrorMessage = "SharedRequired")]
            [Display(Name = "CurrentPassword")]
            [DataType(DataType.Password)]
            [RegularExpression(@"^(?=.*[a-zа-яա-ֆ])(?=.*[A-ZА-ЯԱ-Ֆ])(?=.*\d)\S{6,20}$", ErrorMessage = "PasswordRegEx")]
            public string CurrentPassword { get; set; }
    }
}
