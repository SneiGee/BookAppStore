using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Data;
using BookAppStore.Repository;
using BookAppStore.Models;
using BookAppStore.Service;


namespace BookAppStore.Repository
{
    public class BookRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly BookStoreContext _context = null;
        private readonly UserService _userService;

        public BookRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            BookStoreContext context, UserService userService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _userService = userService;
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<List<BookModel>> GetAllBookAsync()
        {
            var books = new List<BookModel>();
            var allbooks = await _context.Books.ToListAsync();
            if (allbooks?.Any() == true)
            {
                
                foreach (var book in allbooks)
                {
                    var author = (from u in _userManager.Users
                                    where u.Id.Equals(book.ApplicationUserId)
                                    select u.FirstName + " " + u.LastName).SingleOrDefault();

                    books.Add(new BookModel() 
                    {
                        Category = book.Category,
                        Description = book.Description,
                        Id = book.Id,
                        Title = book.Title,
                        ApplicationUserId = author,
                        TotalPage = book.TotalPage,
                        CoverImageUrl = book.CoverImageUrl
                    });
                }
            }

            return books;
        }

        public async Task<int> AddBookAsync(BookModel model)
        {
            // implement the add new book
            var userId = _userService.GetUserId();
            var newBook = new Books()
            {
                ApplicationUserId = userId,
                CreatedOn = DateTime.UtcNow,
                Description = model.Description,
                Title = model.Title,
                Category = model.Category,
                TotalPage = model.TotalPage.HasValue ? model.TotalPage.Value : 0,
                UpdatedOn = DateTime.UtcNow,
                CoverImageUrl = model.CoverImageUrl,
                BookPdfUrl = model.BookPdfUrl
            };

            await _context.Books.AddAsync(newBook);
            await _context.SaveChangesAsync();

            return newBook.Id;
        }

        public async Task<BookModel> GetBookByIdAsync(int id)
        {
            // retrieve book detail
            // var userID = _userManager.FindByIdAsync(userId);
            var users =   _context.Users;
            var model = new BookModel();

            foreach (var use in users)
            {
                var userAuthor = await _userManager.FindByIdAsync(model.ApplicationUserId);
            }

            return await _context.Books.Where(x => x.Id == id)
                .Select(book => new BookModel()
                {
                    ApplicationUser = book.ApplicationUser.FirstName + " " + book.ApplicationUser.LastName,
                    Category = book.Category,
                    Id = book.Id,
                    Description = book.Description,
                    Title = book.Title,
                    TotalPage = book.TotalPage,
                    CoverImageUrl = book.CoverImageUrl,
                    BookPdfUrl = book.BookPdfUrl,
                    ApplicationUserId = book.ApplicationUserId.ToString()
                    // ApplicationUserId = _context.Users.Where(x => x.Id == user.Id).Equals(book.ApplicationUser.Id).ToString()
                }).FirstOrDefaultAsync();
        }
    }
}