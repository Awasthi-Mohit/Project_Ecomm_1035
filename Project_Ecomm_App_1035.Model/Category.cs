using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model
{
    public class Category
    {
        public int Id { get; set; }
        [Required ]
        public String Name  { get; set; }
    }
}
