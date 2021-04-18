using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BookAppStore.Areas.Admin.Models
{
    public class AllBooksModel
    {
        public int Id { get; set; }
        public string Title { get; set; }        
        public string Description { get; set; }
        public string Category { get; set; }
        public string ApplicationUserId { get; set; }
        public string ApplicationUser { get; set; }
        public int? TotalPage { get; set; }
        public DateTime? CreatedOn { get; set; }
        // public IFormFile CoverPhoto { get; set; }
        public string CoverImageUrl { get; set; }
        // public IFormFile BookPdf { get; set; }
        public string BookPdfUrl { get; set; }
    }
}