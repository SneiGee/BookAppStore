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
using BookAppStore.Models;

namespace BookAppStore.Areas.Admin.Controllers
{
    [Area("admin"), Authorize(Roles = "Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleRepository _roleRepository = null;
        public RoleController(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            RoleRepository roleRepository)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _roleRepository = roleRepository;
        }

        [Route("admin")]
        public IActionResult Index()
        {
            return View();
        }

        [Route("admin/add-role")]
        public IActionResult AddRole()
        {
            // add new role form/page
            return View();
        }

        [HttpPost("admin/add-role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(RoleModel roleModel)
        {
            // validate the form and add role
            if (ModelState.IsValid)
            {
                var result = await _roleRepository.AddRoleAsync(roleModel);
                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    ModelState.Clear();
                    return View();
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(roleModel);
        }

        [HttpGet("admin/update-role")]
        public async Task<IActionResult> UpdateRole(string id)
        {
            // retrieve role data
            var role = await _roleRepository.GetRoleById(id);

            var model = new RoleModel()
            {
                Id = role.Id,
                Name = role.Name
            };
            return View(model);
        }

        [HttpPost("admin/update-role")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(RoleModel roleModel)
        {
            // update the fole
            if (ModelState.IsValid)
            {
                var result = await _roleRepository.UpdateRoleAsync(roleModel);

                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    return View(roleModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(roleModel);
        }

        [Route("admin/manage-users")]
        public IActionResult AllUsersList()
        {
            return View();
        }

        [HttpGet("admin/manage-users/edit")]
        public async Task<IActionResult> EditUsers(string id)
        {
            // edit / update users
            var user = await _roleRepository.GetUserById(id);
            if (user == null)
            {
                return NotFound($"Unable to load user data");
            }

            var userRoles = await _userManager.GetRolesAsync(user);

            var model = new EditUserModel()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.Email,
                Email = user.Email,
                Roles = userRoles
            };

            return View(model);
        }

        [HttpPost("admin/manage-users/edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUsers(EditUserModel editUserModel)
        {
            // validate and update user data
            if (ModelState.IsValid)
            {
                var result = await _roleRepository.EditUserAsync(editUserModel);

                if (result.Succeeded)
                {
                    ViewBag.IsSuccess = true;
                    return View(editUserModel);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(editUserModel);
        }

        [HttpGet("admin/manage-users/edit/role")]
        public async Task<IActionResult> ManageUserRole(string userId)
        {
            // retrieve all roles with user selected role
            ViewBag.userId = userId;

            var user = await _roleRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound("Unable to load user data!");
            }

            var model = new List<ManageUsersRoleModel>();

            foreach (var role in _roleManager.Roles)
            {
                var manageUserRole = new ManageUsersRoleModel()
                {
                    RoleId = role.Id,
                    RoleName = role.Name
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    manageUserRole.IsSelected = true;
                }
                else
                {
                    manageUserRole.IsSelected = false;
                }

                model.Add(manageUserRole);
            }

            return View(model);
        }

        [HttpPost("admin/manage-users/edit/role")]
        public async Task<IActionResult> ManageUserRole(List<ManageUsersRoleModel> model, string userId)
        {
            // add / update user role
            var user = await _roleRepository.GetUserById(userId);
            if (user == null)
            {
                return NotFound("Unable to load user data!");
            }

            var role = await _userManager.GetRolesAsync(user);
            var result = await _userManager.RemoveFromRolesAsync(user, role);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing role!");
                return View(model);
            }

            var result1 = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleName));
            if (!result1.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected role to user");
                return View(model);
            }

            return RedirectToAction("EditUsers", new { Id = userId });
        }

        #region API Call
            [HttpGet]
            public async Task<IActionResult> RoleList()
            {
                // retrieve all roles
                return Json(new  { data = await _roleRepository.AllRolesAsync() });
            }

            [HttpGet]
            public async Task<IActionResult> UsersList()
            {
                // retrieve all users with their role
                return Json(new { data = await _roleRepository.AllUserAsync() });
            }

            [HttpDelete]
            [Route("admin/delete")]
            public async Task<IActionResult> DeleteRole(string id)
            {
                // delete role
                var deleteRole = await _roleRepository.DeleteRoleAsync(id);
                if (deleteRole == null)
                {
                    return Json(new { success = false, message = "Error while deleting role" });
                }

                return Json(new { success = true, message = "Delete successful" });
            }

            [HttpDelete]
            [Route("admin/manage-users/delete")]
            public async Task<IActionResult> DeleteAdminUser(string id)
            {
                // delete user
                var deleteUser = await _roleRepository.DeleteUserAsync(id);
                if (deleteUser == null)
                {
                    return Json(new { success = false, message = "Error while deleting user" });
                }

                return Json(new { success = true, message = "Delete user successful" });
            }
        #endregion
    }
}