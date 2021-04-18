using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using BookAppStore.Areas.Admin.Models;
using BookAppStore.Models;
// using BookAppStore.Service;

namespace BookAppStore.Areas.Admin.Repository
{
    public class RoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IdentityRole> GetRoleById(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserById(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<List<IdentityRole>> AllRolesAsync()
        {
            // implement all roles-
            return await _roleManager.Roles.ToListAsync();
        }

        public async Task<List<UserInRoleModel>> AllUserAsync()
        {
            // implement and display all users with their role..
            var userList = new List<UserInRoleModel>();
            var users = await _userManager.Users.ToListAsync();

            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var roleName = userRoles.Count() == 0 ? "No role" : userRoles[0];

                userList.Add(new UserInRoleModel() 
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    UserRole = roleName
                });
            }

            return userList;
        }

        public async Task<IdentityResult> AddRoleAsync(RoleModel roleModel)
        {
            // implement the adding new role
            var role = new IdentityRole()
            {
                Name = roleModel.Name
            };
            var result = await _roleManager.CreateAsync(role);

            return result;
        }

        public async Task<IdentityResult> UpdateRoleAsync(RoleModel roleModel)
        {
            // implement the update role
            var role = await _roleManager.FindByIdAsync(roleModel.Id);
            if (role != null)
            {
                role.Name = roleModel.Name;
            }
            var result = await _roleManager.UpdateAsync(role);

            return result;
        }

        public async Task<IdentityResult> DeleteRoleAsync(string id)
        {
            // implement the deleting role
            var delete = await _roleManager.FindByIdAsync(id);
            return await _roleManager.DeleteAsync(delete);
        }

        public async Task<IdentityResult> EditUserAsync(EditUserModel editUserModel)
        {
            // edit user data
            var user = await _userManager.FindByIdAsync(editUserModel.Id);
            if (user != null)
            {
                user.FirstName = editUserModel.FirstName;
                user.LastName = editUserModel.LastName;
                user.UserName = editUserModel.Email;
                user.Email = editUserModel.Email;
            }

            var result = await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            // implement the deleting user
            var delete = await _userManager.FindByIdAsync(id);
            return await _userManager.DeleteAsync(delete);
        }
        
    }
}