using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppStore.Models
{
    public class UserSignUpModel
    {
        public string Id { get; set; }
        
        [Required(ErrorMessage = "First Name is required"), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required"), Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        [Required(ErrorMessage = "Email Address is required"), Display(Name = "Email Address"),
            EmailAddress(ErrorMessage = "Enter correct email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required"), Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required"), Display(Name = "Confirm Password"),
        Compare("Password", ErrorMessage = "Password does not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}