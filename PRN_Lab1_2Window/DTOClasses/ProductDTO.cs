using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN_Lab1_2Window.DTOClasses
{
    internal class ProductDTO
    {
        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        = null!;
        public string SupplierName { get; set; } = null!;
        public int SoldNumber {  get; set; }
        public string CategoryName { get; set; } = null!;
        public decimal? UnitPrice { get; set; }

    }
}
