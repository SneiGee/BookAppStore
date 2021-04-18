using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Data;
using BookAppStore.Models;
using BookAppStore.Repository;
using BookAppStore.Service;

namespace BookAppStore.Controllers
{
    public class BookController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookRepository _bookRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly UserService _userService;
        private readonly BookStoreContext _context;

        public BookController(
            BookRepository bookRepository, 
            IWebHostEnvironment webHostEnvironment, UserService userService,
            UserManager<ApplicationUser> userManager, BookStoreContext context)
        {
            _bookRepository = bookRepository;
            _webHostEnvironment = webHostEnvironment; _userService = userService;
            _userManager = userManager; _context = context;
        }

        [Route("add-book")]
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost("add-book")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBook(BookModel bookModel)
        {
            // create new book
            if (ModelState.IsValid)
            {
                if (bookModel.CoverPhoto != null)
                {
                    string folder = "books/coverImg/";
                    bookModel.CoverImageUrl = await UploadImage(folder, bookModel.CoverPhoto);
                }

                if (bookModel.BookPdf != null)
                {
                    string folder = "books/pdfFile/";
                    bookModel.BookPdfUrl = await UploadImage(folder, bookModel.BookPdf);
                }

                int id = await _bookRepository.AddBookAsync(bookModel);
                if (id > 0)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }

            }

            ModelState.AddModelError("", "Something is wrong!");

            return View(bookModel);
        }

        [Route("book-detail")]
        public async Task<IActionResult> BookDetail(BookModel bookModel)
        {
            // display book detail with id

            var data = await  _bookRepository.GetBookByIdAsync(bookModel.Id);
            return View(data);
        }

        private async Task<string> UploadImage(string folderPath, IFormFile file)
        {

            folderPath += Guid.NewGuid().ToString() + "_" + file.FileName;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }
    }
}