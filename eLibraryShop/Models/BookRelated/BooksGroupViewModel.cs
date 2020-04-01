using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    public class BooksGroupViewModel
    {
        public List<Book> Books { get; set; }

        public int PageNumber { get; set; }
        public int PageRange { get; set; }
        public int TotalPages { get; set; }
        public string Genre { get; set; }
        public string CategorySlug { get; set; }

        public BooksGroupViewModel(List<Book> books, int pageNumber, int pageRange, int totalPages)
        {
            Books = books;
            PageNumber = pageNumber;
            PageRange = pageRange;
            TotalPages = totalPages;
        }

        public BooksGroupViewModel(List<Book> books, int pageNumber, int pageRange, int totalPages, string genre, string categorySlug)
        {
            Books = books;
            PageNumber = pageNumber;
            PageRange = pageRange;
            TotalPages = totalPages;
            Genre = genre;
            CategorySlug = categorySlug;
        }
    }
}
