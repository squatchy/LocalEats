using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalEats.Model
{
    public class RestaurantCategoryInfo : BaseModel
    {
        public string Name { get; set; }
        public int? RestaurantCount { get; set; }
    }
}
