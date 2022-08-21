using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace prjGroceryStore4.Models
{
    public class PurchaseViewModel
    {
        public long PurchaseId { get; set; }
        public System.DateTime PurchaseDate { get; set; }

        public int SupplierID { get; set; }

        public int ProductID { get; set; }

        public int ProductQuantity { get; set; }

        public decimal ProductBuyingPrice { get; set; }
        

    }
}