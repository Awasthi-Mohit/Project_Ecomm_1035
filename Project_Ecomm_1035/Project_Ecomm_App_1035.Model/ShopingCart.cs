﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model
{
    public class ShopingCart
    {

        public ShopingCart()
        {
            Count = 1;
        }
        public int Id { get; set; } 

        public string ApplicationUserId { get;set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }    
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }    
        public int Count { get; set; }
        [NotMapped]
        public double price { get; set; }
    }
}
