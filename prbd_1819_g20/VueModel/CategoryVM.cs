using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace prbd_1819_g20.VueModel
{
    public class CategoryVM
    {
        public Category Category { get; set; }

        public bool HasBookSaved { get; set; }

        public CategoryVM(Category cat)
        {
            Category = cat;
        }
    }
}
