using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.DAL.Dto
{
    public class CategoryRequest
    {
        public List <CategoryTranslationRequest> Translations { get; set; }
    }
}
