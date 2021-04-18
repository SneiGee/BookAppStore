using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppStore.Models
{
    public class UserForgotPasswordModel
    {
        [Required(ErrorMessage = "Email is required"), Display(Name = "Your Email"), 
            EmailAddress(ErrorMessage = "Enter a valid email address")]
        public string Email { get; set; }
        public bool EmailSent { get; set; }
    }
}