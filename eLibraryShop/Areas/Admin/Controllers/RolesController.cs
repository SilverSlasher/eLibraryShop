using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eLibraryShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin")]
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<AppUser> userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<AppUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }

        //GET /admin/roles
        public IActionResult Index() => View(roleManager.Roles);

        //GET /admin/roles/create
        public IActionResult Create() => View();

        //POST /admin/roles/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Role role)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(role.Name));

                if (result.Succeeded)
                {
                    TempData["Success"] = "Rola została utworzona"; //Role has been created
                    return RedirectToAction("Index");
                }

                ModelState.AddModelError("", "Rola o podanej nazwie już istnieje"); //Role already exists
            }

            return View();
        }

        //GET /admin/roles/edit/id
        public async Task<IActionResult> Edit(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            List<AppUser> members = new List<AppUser>();
            List<AppUser> nonMembers = new List<AppUser>();

            foreach (AppUser user in userManager.Users)
            {
                var list = await userManager.IsInRoleAsync(user, role.Name) ? members : nonMembers;
                list.Add(user);
            }

            return View(new RoleEdit
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        //POST /admin/roles/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleEdit roleEdit)
        {
            IdentityResult result;

            foreach (string userId in roleEdit.AddIds ?? new string[] { })
            {
                AppUser user = await userManager.FindByIdAsync(userId);
                result = await userManager.AddToRoleAsync(user, roleEdit.RoleName);
            }

            foreach (string userId in roleEdit.DeleteIds ?? new string[] { })
            {
                AppUser user = await userManager.FindByIdAsync(userId);
                result = await userManager.RemoveFromRoleAsync(user, roleEdit.RoleName);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        //GET /admin/roles/delete/id
        public async Task<IActionResult> Delete(string id)
        {
            IdentityRole role = await roleManager.FindByIdAsync(id);

            if (role != null)
            {
                await roleManager.DeleteAsync(role);
                TempData["Success"] = "Rola została usunięty"; //Role has been deleted
            }
            else
            {
                TempData["Error"] = "Rola nie istnieje"; //Role does not exists
            }
            return RedirectToAction("Index");
        }
    }
}