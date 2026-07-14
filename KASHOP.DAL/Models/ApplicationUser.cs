using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public String FullName { get; set; }
        public String? City { get; set; }
        public String? Street { get; set; }
    }
}
