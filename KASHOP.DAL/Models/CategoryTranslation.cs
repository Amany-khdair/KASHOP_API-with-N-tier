using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Models
{
    public class CategoryTranslation
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public String Language { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
