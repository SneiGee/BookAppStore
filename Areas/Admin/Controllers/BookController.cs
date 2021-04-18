using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using BookAppStore.Areas.Admin.Repository;
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Data;
using BookAppStore.Models;

namespace BookAppStore.Areas.Admin.Controllers
{
    [Area("admin"), Authorize(Roles = "Admin")]
    public class BookController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AdminBookRepository _adminBookRepository = null;

        public BookController(UserManager<ApplicationUser> userManager,
            AdminBookRepository adminBookRepository)
        {
            _userManager = userManager;
            _adminBookRepository = adminBookRepository;
        }

        [Route("admin/manage-books")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("admin/manage-books/edit")]
        public async Task<IActionResult> UpdateBook(int id)
        {
            // display user book data
            var book = await _adminBookRepository.GetBookById(id);
            if (book == null)
            {
                return NotFound($"Unable to load user book data");
            }

            var allBooksModel = new AllBooksModel()
            {
                Id = book.Id,
                Title = book.Title,
                Description = book.Description,
                Category = book.Category,
                TotalPage = book.TotalPage,
            };

            return View(allBooksModel);
        }

        [HttpPost("admin/manage-books/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBook(AllBooksModel allBooksModel)
        {
            // check validate and update book data
            if (ModelState.IsValid)
            {
                var result = await _adminBookRepository.UpdateBookAsync(allBooksModel);

                if (result > 0)
                {
                    ViewBag.IsSuccess = true;
                    return View(allBooksModel);
                }

            }

            ModelState.AddModelError("", "Something is wrong! Please try again");

            return View(allBooksModel);
        }

        #region API Call
            
            [HttpGet]
            public async Task<IActionResult> BooksList()
            {
                // load data without refreshing the page
                return Json(new { data = await _adminBookRepository.AllBooksAsync() });
            }

            [HttpDelete]
            [Route("admin/manage-books/delete")]
            public async Task<IActionResult> DeleteBook(AllBooksModel allBooksModel)
            {
                // delete book method
                var book = await _adminBookRepository.DeleteBookAsync(allBooksModel);
                if (book == null)
                {
                    return Json(new { success = false, message = "Error while deleting book" });
                }

                return Json(new { success = true, message = "book deleted successful!" });
            }

        #endregion
    }
}