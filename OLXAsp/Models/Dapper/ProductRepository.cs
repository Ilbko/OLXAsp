using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OLXAsp.Models.Dapper
{
    public class Product
    {
        public int Product_Id { get; set; }
        public string Product_ImageURL { get; set; }
        public string Product_Description { get; set; }
        public decimal Product_Price { get; set; }
        public int Product_CategoryId { get; set; }
    }
    public class ProductRepository
    {

    }
}
