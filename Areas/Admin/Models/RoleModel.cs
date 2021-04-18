using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;

namespace BookAppStore.Areas.Admin.Models
{
    public class RoleModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Role name is required"), Display(Name = "Role Name")]
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public bool IsSuccess { get; set; }
    }
}