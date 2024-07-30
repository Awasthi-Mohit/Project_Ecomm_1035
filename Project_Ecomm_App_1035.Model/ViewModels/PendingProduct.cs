using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model.ViewModels
{
    public class PendingProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public IEnumerable<OrderHeader> OrderStatuse { get; set; }
        public IEnumerable<OrderDetails> OrderDetails { get; set; }
        public OrderDetails OrderDetail { get; set; }
        public OrderHeader OrderHeader { get; set; }
    }
}
