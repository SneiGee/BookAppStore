using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace BookAppStore.Models
{
    public class EditUserProfile
    {
         public string Id { get; set; }  
        
        [Required(ErrorMessage = "First Name is required"), Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required"), Display(Name = "Last Name")]
        public string LastName { get; set; }
        
        public string UserName { get; set; } 
        
        [Required(ErrorMessage = "Email Address is required"), Display(Name = "Email address")]
        [EmailAddress(ErrorMessage = "Email Address is not valid")]
        public string Email { get; set; }  
    }
}