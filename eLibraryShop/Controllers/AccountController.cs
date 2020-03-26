using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Models;
using eLibraryShop.Models.UserVariants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace eLibraryShop.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
        }


        // GET /account/register
        [AllowAnonymous]
        public IActionResult Register() => View();


        // Post /account/register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    TempData["RegisterSuccess"] = "Konto zostało utworzone pomyślnie"; //Account has been created

                    return RedirectToAction("Register");
                }

                foreach (IdentityError error in result.Errors)
                {
                    if (error.Code == "PasswordRequiresDigit")
                    {
                        error.Description = "Hasło musi zawierać minimum jedną cyfrę"; //Password require at least 1 digit number
                    }

                    if (error.Code == "PasswordRequiresLower")
                    {
                        error.Description = "Hasło musi zawierać minimum jedną małą literę"; //Password require at least 1 lower letter
                    }

                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(user);
        }


        // GET /account/login
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            Login login = new Login
            {
                ReturnUrl = returnUrl
            };

            return View(login);
        }


        // POST /account/login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByNameAsync(login.UserName);


                if (appUser == null)
                {
                    ModelState.AddModelError("", "Wprowadzono nieprawidłowe dane"); //Wrong credentials
                    return View(login);
                }

                SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);

                if (result.Succeeded)
                {
                    TempData["LoginSuccess"] = "Logowanie udane"; //Logging successful
                    return Redirect(login.ReturnUrl ?? "/");
                }
            }

            return View(login);
        }

        // GET /account/logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            TempData["LogoutSuccess"] = "Wylogowano pomyślnie"; //Logout successful

            return Redirect("/");
        }

        // GET /account/edit
        public async Task<IActionResult> Edit()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }


        //POST /account/edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserEdit user)
        {
            AppUser appUser = await userManager.FindByNameAsync(user.UserName);

            if (ModelState.IsValid)
            {
                appUser.Email = user.Email;

                if (user.Password != null)
                {
                    if (user.OldPassword == null)
                    {
                        ModelState.AddModelError("", "Wprowadź aktualne hasło");  //Enter current password
                        return View(user);
                    }

                    var passwordValidator = new PasswordValidator<AppUser>();
                    var validatePassword = await passwordValidator.ValidateAsync(userManager, appUser, user.OldPassword);

                    if (validatePassword.Succeeded)
                    {
                        appUser.PasswordHash = passwordHasher.HashPassword(appUser, user.Password);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Aktualne hasło jest nieprawidłowe"); //Password is incorrect
                        return View(user);
                    }
                }

                IdentityResult result = await userManager.UpdateAsync(appUser);

                if (result.Succeeded)
                {
                    TempData["EditSuccess"] = "Dane zostały zmienione"; //Credentials changed
                }
            }

            return RedirectToAction("Details");
        }

        public async Task<IActionResult> Details()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserViewModel user = new UserViewModel(appUser);

            return View(user);
        }
    }
}