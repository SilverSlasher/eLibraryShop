using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Controllers
{
    public class CartController : Controller
    {
        private readonly eLibraryShopContext context;

        public CartController(eLibraryShopContext context)
        {
            this.context = context;
        }

        // GET  /cart
        public IActionResult Index()
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart") ?? new List<CartItem>();

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

           CartItem cartItem = cart.Where(x => x.BookId == id).FirstOrDefault();

           if (cartItem == null)
           {
                cart.Add(new CartItem(book));
           }
           else
           {
               cartItem.Quantity += 1;
           }

           HttpContext.Session.SetJson("Cart",cart);

           return RedirectToAction("Index");
        }

        // GET  /cart/decrease/id
        public async Task<IActionResult> Decrease(int id)
        {
            List<CartItem> cart = HttpContext.Session.GetJson<List<CartItem>>("Cart");

            CartItem cartItem = cart.Where(x => x.BookId == id).FirstOrDefault();

            if (cartItem == null)
            {
                cart.Add(new CartItem(book));
            }
            else
            {
                cartItem.Quantity += 1;
            }

            HttpContext.Session.SetJson("Cart", cart);

            return RedirectToAction("Index");
        }
    }
}