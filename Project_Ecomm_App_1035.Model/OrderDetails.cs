using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model
{
    public class OrderDetails
    {
        public int id { get; set; }
        public int OrderHeaderId { get; set; }
        [ForeignKey("OrderHeaderId")] 
        public OrderHeader orderHeader { get; set; }    
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }        
        public int Count { get; set; }
        public double Price { get; set; }
    }
}
