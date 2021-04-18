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
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Data;
using BookAppStore.Models;

namespace BookAppStore.Areas.Admin.Repository
{
    public class AdminBookRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly BookStoreContext _context = null;

        public AdminBookRepository(UserManager<ApplicationUser> userManager,
            BookStoreContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<Books> GetBookById(int id)
        {
            return await _context.Books.FindAsync(id);
        }

        public async Task<List<AllBooksModel>> AllBooksAsync()
        {
            // retrieve all book for admin
            var books = new List<AllBooksModel>();
            var allBooks = await _context.Books.ToListAsync();

            if (allBooks?.Any() == true)
            {
                foreach (var book in allBooks)
                {
                    // loop all users and get the author or book creator
                    var author = (from u in _userManager.Users where u.Id.Equals(book.ApplicationUserId)
                                        select(u.FirstName + " " + u.LastName)).SingleOrDefault();
                    
                    books.Add(new AllBooksModel()
                    {
                        Category = book.Category,
                        Description = book.Description,
                        Id = book.Id,
                        Title = book.Title,
                        ApplicationUserId = author,
                        TotalPage = book.TotalPage,
                        CreatedOn = book.CreatedOn,
                        CoverImageUrl = book.CoverImageUrl
                    });
                }
            }

            return books;
        }

        public async Task<int> UpdateBookAsync(AllBooksModel allBookModel)
        {
            // update the yser book data
            var book = await _context.Books.FindAsync(allBookModel.Id);
            if (book != null)
            {
                book.Title = allBookModel.Title;
                book.Category = allBookModel.Category;
                book.Description = allBookModel.Description;
                book.TotalPage = allBookModel.TotalPage.HasValue ? allBookModel.TotalPage.Value : 0;
            }
            
            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            return allBookModel.Id;
        }

        public async Task<int?> DeleteBookAsync(AllBooksModel allBooksModel)
        {
            // implement the delete function
            var deleteBook = await _context.Books.FirstOrDefaultAsync(u => u.Id == allBooksModel.Id);
            
            _context.Books.Remove(deleteBook);
            return await _context.SaveChangesAsync();
        }
    }
}