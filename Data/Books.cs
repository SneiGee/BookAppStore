using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookAppStore.Areas.Admin.Models;

namespace BookAppStore.Data
{
    public class Books
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ApplicationUserId { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public int TotalPage { get; set; }
        public string CoverImageUrl { get; set; }
        public string BookPdfUrl { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

        // public IEnumerable<Books> Books { get; set; }
    }
}