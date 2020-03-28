using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
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
        private readonly eLibraryShopContext context;
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private IPasswordHasher<AppUser> passwordHasher;

        public AccountController(UserManager<AppUser> userManager,
                                SignInManager<AppUser> signInManager,
                                IPasswordHasher<AppUser> passwordHasher,
                                eLibraryShopContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.passwordHasher = passwordHasher;
            this.context = context;
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
                    TempData["Success"] = "Logowanie udane"; //Logging successful
                    return Redirect(login.ReturnUrl ?? "/");
                }
            }

            return View(login);
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
                    TempData["Success"] = "Dane zostały zmienione"; //Credentials changed
                }
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

            foreach(Order order in context.Orders.Where(x => x.UserId == appUser.Id)
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

            DeliveryAddress address = await context.DeliveryAddresses.FirstOrDefaultAsync(x => x.UserId == appUser.Id);

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
                if (await context.DeliveryAddresses.FirstOrDefaultAsync(x => x.UserId == appUser.Id) == null)
                {
                    address.UserId = appUser.Id;
                    context.Add(address);
                    TempData["Success"] = "Adres został dodany"; //Address added
                }
                else
                {
                    context.Update(address);
                    TempData["Success"] = "Adres został zmieniony"; //Address changed
                }

                await context.SaveChangesAsync();
            }

            return RedirectToAction("Details");
        }
    }
}