using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Models
{
    public class Category: ISoftDelete
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<SubCategory> SubCategories { get; set; }
        public bool IsDeleted { get; set; }
    }
}
