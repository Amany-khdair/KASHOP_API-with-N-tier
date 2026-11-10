using KASHOP.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class OrderSummaryResponse
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatusEnum Status { get; set; }
        public PaymentStatusEnum PaymentStatus { get; set; }
        public PaymentMethodEnum PaymentMethod { get; set; }
        public decimal? TotalAmount { get; set; }
        public int ItemsCount { get; set; }
    }
}
