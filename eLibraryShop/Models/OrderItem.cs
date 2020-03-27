using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace eLibraryShop.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        public int BookId { get; set; }

        [Display(Name = "Ilość")]
        public int Quantity { get; set; }

        [ForeignKey("BookId")]
        public virtual Book Book { get; set; }

        public OrderItem()
        {
            
        }

        public OrderItem(CartItem cartItem)
        {
            BookId = cartItem.BookId;
            Quantity = cartItem.Quantity;
        }
    }
}
