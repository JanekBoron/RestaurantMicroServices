using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restaurant.Services.OrderAPI.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }
        public int CartHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")]
        public virtual OrderHeader CartHeader { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }
}
