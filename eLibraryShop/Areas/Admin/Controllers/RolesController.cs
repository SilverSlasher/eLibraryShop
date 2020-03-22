using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eLibraryShop.Areas.Admin.Controllers
{
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
                    TempData["Success"] = "Rola została utworzona";
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (IdentityError error in result.Errors) 
                    {
                        if (error.Code == "DuplicateRoleName")
                        {
                            error.Description = "Rola o podanej nazwie już istnieje";
                        }

                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }

    }
}