using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Controllers
{
    public class CartController : Controller
    {
        private readonly eLibraryShopContext context;
        private readonly UserManager<AppUser> userManager;


        public CartController(eLibraryShopContext context, UserManager<AppUser> userManager)
        {
            this.userManager = userManager;
            this.context = context;
        }

        // GET  /cart
        public async Task<IActionResult> Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            CartViewModel cartVM = new CartViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Total)
            };

            return View(cartVM);
        }

        // GET  /cart/add/id
        public async Task<IActionResult> Add(int id)
        {
            Book book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);

            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

            CartItem cartItem = cart.FirstOrDefault(x => x.BookId == id);

            if (cartItem == null)
            {
                cart.Add(new CartItem(book));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                return RedirectToAction("Index");
            }

            return ViewComponent("SmallCart");
        }

        // GET  /cart/decrease/id
        public async Task<IActionResult> Decrease(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.FirstOrDefault(x => x.BookId == id);

            if (cartItem.Quantity > 1)
            {
                --cartItem.Quantity;
            }
            else
            {
                cart.RemoveAll(x => x.BookId == id);
            }


            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }


        // GET  /cart/remove/id
        public IActionResult Remove(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            cart.RemoveAll(x => x.BookId == id);


            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart");
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            return RedirectToAction("Index");
        }


        // GET  /cart/clear
        public IActionResult Clear(int id)
        {
            HttpContext.Session.Remove("Cart");

            if (HttpContext.Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            return Ok();
        }


        public async Task<IActionResult> SaveOrder()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            AppUser appUser = await userManager.FindByNameAsync(User.Identity.Name);

            if (context.DeliveryAddresses.FirstOrDefault(x => x.UserId == appUser.Id) == null)
            {
                TempData["Error"] = "Wprowadź adres dostawy";
                return RedirectToAction("Details", "Account");
            }

            Order recentOrder = new Order(cart, cart.Sum(x => x.Total), DateTime.Now, appUser.Id);

            context.Add(recentOrder);
            await context.SaveChangesAsync();

            CartViewModel cartVM = new CartViewModel
            {
                CartItems = cart,
                GrandTotal = cart.Sum(x => x.Total),
                Function = "FinalizeOrder()"
            };

            return View("Index",cartVM);
        }

    }
}