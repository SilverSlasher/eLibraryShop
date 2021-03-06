﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using eLibraryShop.Infrastructure;
using eLibraryShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace eLibraryShop.Areas.Admin.Controllers
{
    [Authorize(Roles = "admin,moderator")]
    [Area("Admin")]
    public class BooksController : Controller
    {
        private readonly eLibraryShopContext context;
        private readonly IWebHostEnvironment webHostEnvironment;

        public BooksController(eLibraryShopContext context, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            this.webHostEnvironment = webHostEnvironment;
        }

        //GET /admin/books
        public async Task<IActionResult> Index(int p = 1)
        {
            int pageSize = 10;
            var books = context.Books.OrderByDescending(x => x.Id).Include(x => x.Genre).Skip((p - 1) * pageSize).Take(pageSize);

            ViewBag.PageNumber = p;
            ViewBag.PageRange = pageSize;
            ViewBag.TotalPages = (int)Math.Ceiling((decimal)context.Books.Count() / pageSize);

            return View(await books.ToListAsync());
        }


        //GET /admin/books/create
        public IActionResult Create()
        {
            ViewBag.GenreId = new SelectList(context.Genres.OrderBy(x => x.Sorting), "Id", "Name");

            return View();
        }


        //Post /admin/books/create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Book book)
        {
            ViewBag.GenreId = new SelectList(context.Genres.OrderBy(x => x.Sorting), "Id", "Name");

            if (ModelState.IsValid)
            {
                book.Slug = book.Title.ToLower().Replace(" ", "-");
                var slug = await context.Books.FirstOrDefaultAsync(x => x.Slug == book.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Książka o podanej nazwie już istnieje"); //Book already exists
                    return View(book);
                }

                string imageName = "noimage.png";

                //Create a new,  unique name for photo and save it
                if (book.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "media/books");
                    imageName = Guid.NewGuid().ToString() + "_" + book.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await book.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                }

                book.Image = imageName;
                context.Add(book);
                await context.SaveChangesAsync();

                TempData["Success"] = "Książka została dodana"; //Book has been added

                return RedirectToAction("Index");
            }

            return View(book);
        }


        //GET /admin/books/details/id
        public async Task<IActionResult> Details(int id)
        {
            Book book = await context.Books.Include(x => x.Genre).FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }


        //GET /admin/pages/edit/id
        public async Task<IActionResult> Edit(int id)
        {
            Book book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                return NotFound();
            }

            ViewBag.GenreId = new SelectList(context.Genres.OrderBy(x => x.Sorting), "Id", "Name", book.GenreId);

            return View(book);
        }

        //Post /admin/books/edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Book book)
        {

            ViewBag.GenreId = new SelectList(context.Genres.OrderBy(x => x.Sorting), "Id", "Name", book.GenreId);

            if (ModelState.IsValid)
            {
                book.Slug = book.Title.ToLower().Replace(" ", "-");
                var slug = await context.Books.Where(x => x.Id != id).FirstOrDefaultAsync(x => x.Slug == book.Slug);

                if (slug != null)
                {
                    ModelState.AddModelError("", "Książka o podanej nazwie już istnieje"); //Book already exists
                    return View(book);
                }

                if (book.ImageUpload != null)
                {
                    string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "media/books");

                    //If there is "no image" photo, don't delete it, because its universal photo for many items
                    if (!string.Equals(book.Image, "noimage.png"))
                    {
                        string oldImagePath = Path.Combine(uploadDir, book.Image);

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }

                    }

                    //Create a new,  unique name for photo and save it
                    string imageName = Guid.NewGuid().ToString() + "_" + book.ImageUpload.FileName;
                    string filePath = Path.Combine(uploadDir, imageName);
                    FileStream fs = new FileStream(filePath, FileMode.Create);
                    await book.ImageUpload.CopyToAsync(fs);
                    fs.Close();
                    book.Image = imageName;
                }

                context.Update(book);
                await context.SaveChangesAsync();

                TempData["Success"] = "Książka została zedytowana"; //Book has been edited

                return RedirectToAction("Index");
            }

            return View(book);
        }

        //GET /admin/books/delete/id
        public async Task<IActionResult> Delete(int id)
        {
            Book book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);

            if (book == null)
            {
                TempData["Error"] = "Książka nie istnieje"; //Book does not exist
                return RedirectToAction("Index");
            }

            //If there is "no image" photo, don't delete it, because its universal photo for many items
            if (!string.Equals(book.Image, "noimage.png"))
            {
                string uploadDir = Path.Combine(webHostEnvironment.WebRootPath, "media/books");
                string oldImagePath = Path.Combine(uploadDir, book.Image);

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            context.Books.Remove(book);
            await context.SaveChangesAsync();
            TempData["Success"] = "Książka została usunięta"; //Book has been deleted

            return RedirectToAction("Index");
        }
    }
}