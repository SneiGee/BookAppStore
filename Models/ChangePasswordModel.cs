using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookAppStore.Models
{
    public class ChangePasswordModel
    {
        [Required(ErrorMessage = "Current Password is required"), DataType(DataType.Password), Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }
        
        [Required(ErrorMessage = "New Password is required"), DataType(DataType.Password), Display(Name = "New Password")]
        public string NewPassword { get; set; }
        
        [Required(ErrorMessage = "Confirm New Password is required"), DataType(DataType.Password), Display(Name = "Confirm New Password")]
        [Compare("NewPassword", ErrorMessage = "Oop! Password do not match")]
        public string ConfirmNewPassword { get; set; }
    }
}