using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    public class Order
    {
        public int Id { get; set; }

        public List<OrderItem> Books { get; set; }

        [Column(TypeName = "decimal(6,2)")]
        [Display(Name = "Wartość zamówienia")]
        public decimal GrandTotal { get; set; }

        [Display(Name = "Data")]
        public DateTime Date { get; set; }

        public string UserId { get; set; }

        public Order()
        {
            
        }

        public Order(List<CartItem> boughtBooks, decimal grandTotal, DateTime date,string userId)
        {
            Books = new List<OrderItem>();

            foreach (CartItem book in boughtBooks)
            {
                OrderItem item = new OrderItem(book);
                Books.Add(item);
            }

            GrandTotal = grandTotal;
            Date = date;
            UserId = userId;
        }
    }
}