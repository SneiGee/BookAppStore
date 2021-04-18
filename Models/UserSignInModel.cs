using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace BookAppStore.Models
{
    public class UserSignInModel
    {
        [Required(ErrorMessage = "Email address is required"), Display(Name = "Email Address"),
            EmailAddress(ErrorMessage = "Enter correct Emaill Address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is requires"), DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}