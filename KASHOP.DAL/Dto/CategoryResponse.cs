using KASHOP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class CategoryResponse
    {
        public int Id { get; set; }
        public String User { get; set; }
        public string Name { get; set; }
        //public List<CategoryTranslationResponse> Translations { get; set; }
    }
}
