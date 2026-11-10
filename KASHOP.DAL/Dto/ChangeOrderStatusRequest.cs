using KASHOP.DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class ChangeOrderStatusRequest
    {
        public OrderStatusEnum Status { get; set; }
    }
}
