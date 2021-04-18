using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using BookAppStore.Areas.Admin.Models;


namespace BookAppStore.Models
{
    public class BookModel
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Please enter the title of your book"), Display(Name = "Book Name")]
        public string Title { get; set; }
        

        [Required(ErrorMessage = "Please enter the description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter the book category")]
        public string Category { get; set; }

        public string ApplicationUserId { get; set; }
        public string ApplicationUser { get; set; }
        
        [Required(ErrorMessage = "Please enter the total pages"), Display(Name = "Total pages of book")]
        public int? TotalPage { get; set; }
        
        [Required(ErrorMessage = "Please choose cover photo")]
        [Display(Name = "Cover Photo")]
        public IFormFile CoverPhoto { get; set; }
        public string CoverImageUrl { get; set; }

        [Required(ErrorMessage = "Please choose book in pdf format")]
        [Display(Name = "Book PDF")]
        public IFormFile BookPdf { get; set; }
        public string BookPdfUrl { get; set; }
        
    }
}