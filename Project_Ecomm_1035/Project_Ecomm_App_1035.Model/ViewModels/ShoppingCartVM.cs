using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShopingCart>ListCart{ get; set; }
       public OrderHeader orderHeader { get; set; }
       public OrderDetails OrderDetails { get; set; }
        //public ShoppingCartVM[] ItemChecked { get; set; }
    }
}
