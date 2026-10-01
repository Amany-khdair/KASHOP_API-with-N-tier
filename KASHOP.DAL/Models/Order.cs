using KASHOP.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatusEnum Status { get; set; } = OrderStatusEnum.Pending;
        public PaymentStatusEnum PaymentStatus { get; set; } = PaymentStatusEnum.Pending;
        public PaymentMethodEnum PaymentMethod { get; set; }
        public DateTime? ShippedDate { get; set; }
        public decimal? AmountPaid { get; set; }
        public decimal? TotalAmount { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PhoneNumber { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
