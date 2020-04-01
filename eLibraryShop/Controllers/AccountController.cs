using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using eLibraryShop.Models.UserRelated;
using eLibraryShop.Models.UserVariants;
using eLibraryShop.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace eLibraryShop.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private readonly eLibraryShopContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;
        private readonly ILogger<AccountController> logger;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher,
                                eLibraryShopContext context,
                                ILogger<AccountController> logger)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.context = context;
            this.logger = logger;
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
                    var token = await userManager.GenerateEmailConfirmationTokenAsync(appUser);

                    var confirmationLink = Url.Action("ConfirmEmail", "Account", new { userId = appUser.Id, token = token }, Request.Scheme);

                    EmailService.SendEmailConfirmation(appUser.UserName, appUser.Email, confirmationLink);

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


        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
            {
                return RedirectToAction("Page", "Pages");
            }

            AppUser appUser = await userManager.FindByIdAsync(userId);

            if (appUser == null)
            {
                TempData["Error"] = "Id użytkownika jest nieprawidłowe"; //Id is incorrect
                return RedirectToAction("Page", "Pages");
            }

            var result = await userManager.ConfirmEmailAsync(appUser, token);

            if (result.Succeeded)
            {
                TempData["Success"] = "Adres email został potwierdzony"; //Email has been confirmed
                return RedirectToAction("Page", "Pages");
            }

            TempData["Error"] = "Adres email nie może zostać potwierdzony"; //Email can't be confirmed
            return RedirectToAction("Page", "Pages");
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
                    TempData["Error"] = "Nie ma takiego użytkownika w bazie"; //Wrong username
                    return View(login);
                }

                if (!appUser.EmailConfirmed && (await userManager.CheckPasswordAsync(appUser, login.Password)))
                {
                    ModelState.AddModelError("", "Potwierdź adres email, poprzez link w wysłanej wiadomości"); //Email not confirmed
                    return View(login);
                }

                SignInResult result = await signInManager.PasswordSignInAsync(appUser, login.Password, false, false);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Logowanie udane"; //Logging successful
                    return Redirect(login.ReturnUrl ?? "/");
                }
                else
                {
                    TempData["Error"] = "Wprowadzono nieprawidłowe hasło"; //Wrong password
                }
            }

            return View(login);
        }


        // GET /account/forgotpassword
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }


        // POST /account/forgotpassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPasswordVM)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(forgotPasswordVM.Email);

                if (appUser != null && await userManager.IsEmailConfirmedAsync(appUser))
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(appUser);

                    var passwordResetLink = Url.Action("ResetPassword", "Account",
                        new { email = forgotPasswordVM.Email, token = token }, Request.Scheme);

                    EmailService.SendPasswordResetRequest(appUser.UserName, appUser.Email, passwordResetLink);

                    forgotPasswordVM.IsExisting = true;

                    return View("ForgotPasswordConfirmation", forgotPasswordVM);
                }

                forgotPasswordVM.IsExisting = false;

                return View("ForgotPasswordConfirmation", forgotPasswordVM);
            }

            return View(forgotPasswordVM);
        }


        [AllowAnonymous]
        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null)
            {
                ModelState.AddModelError("", "Nieprawidłowy link do resetowania hasła"); //Invalid password reset token
            }

            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel resetPasswordVM)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = await userManager.FindByEmailAsync(resetPasswordVM.Email);

                if (appUser != null)
                {
                    var result =
                        await userManager.ResetPasswordAsync(appUser, resetPasswordVM.Token, resetPasswordVM.Password);

                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfirmation", appUser.UserName);
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

                    return View(resetPasswordVM);
                }

                return View("ResetPasswordConfirmation", appUser.UserName);
            }

            return View(resetPasswordVM);
        }



        // GET /account/logout
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            TempData["Success"] = "Wylogowano pomyślnie"; //Logout successful

            return Redirect("/");
        }


        // GET /account/editcredentials
        public async Task<IActionResult> EditCredentials()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserEdit user = new UserEdit(appUser);

            return View(user);
        }


        //POST /account/editcredentials
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCredentials(UserEdit user)
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

                }

                IdentityResult result = await userManager.ChangePasswordAsync(appUser, user.OldPassword, user.Password);

                if (!result.Succeeded)
                {
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

                        if (error.Code == "PasswordMismatch")
                        {
                            error.Description = "Wprowadzone aktualne hasło jest nieprawidłowe"; //Current password is incorrect
                        }
                        
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(user);

                }

                TempData["Success"] = "Dane zostały zmienione"; //Credentials changed
            }

            return RedirectToAction("Details");
        }

        // GET /account/details
        public async Task<IActionResult> Details()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);
            UserViewModel user = new UserViewModel(appUser);

            user.Address = await context.DeliveryAddresses.FirstOrDefaultAsync(x => x.UserId == appUser.Id);

            return View(user);
        }

        // GET /account/recentorders
        public async Task<IActionResult> RecentOrders()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            List<Order> orders = new List<Order>();

            foreach (Order order in context.Orders.Where(x => x.UserId == appUser.Id)
                                                .Include(o => o.Books).
                                                ThenInclude(o => o.Book).
                                                ThenInclude(o => o.Genre))
            {
                orders.Add(order);
            }

            return View(orders);
        }

        //GET /account/recentorders/id
        public async Task<IActionResult> OrderDetails(int id)
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            Order order = context.Orders.Where(x => x.UserId == appUser.Id)
                                        .Include(o => o.Books)
                                        .ThenInclude(o => o.Book)
                                        .FirstOrDefault(x => x.Id == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // GET /account/editaddress
        public async Task<IActionResult> EditAddress()
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            DeliveryAddress address = await context.DeliveryAddresses.FirstOrDefaultAsync(x => x.UserId == appUser.Id) ?? new DeliveryAddress();

            return View(address);
        }


        //POST /account/editaddress
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(DeliveryAddress address)
        {
            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            if (ModelState.IsValid)
            {
                DeliveryAddress newAddress =
                    await context.DeliveryAddresses.FirstOrDefaultAsync(x => x.UserId == appUser.Id);

                if (newAddress == null)
                {
                    address.UserId = appUser.Id;
                    context.Add(address);
                    TempData["Success"] = "Adres został dodany"; //Address added
                }
                else
                {
                    newAddress.City = address.City;
                    newAddress.Street = address.Street;
                    newAddress.ZIPCode = address.ZIPCode;

                    context.Update(newAddress);
                    TempData["Success"] = "Adres został zmieniony"; //Address changed
                }

                await context.SaveChangesAsync();
            }

            return RedirectToAction("Details");
        }



    }
}