using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model
{
    public class OrderHeader
    {
        public int id { get;set ; } 
        public string? ApplicationUserId { get;set; }
        [Display(Name ="ApplicationUserId")]
        public  ApplicationUser ApplicationUser{ get;set; }
        [Required]
        public DateTime OrderDate  { get;set; }
        [Required]
        public DateTime ShippingDate { get; set; }
        [Required]
        public Double OrderTotal { get; set;}
        [Required]
        public string? TrackingNumber{ get; set; }
        public string? Carrier { get; set; }
        public string? OrderStatus { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime PaymentDate { get; set; }
        public DateTime PaymentDueDate { get; set; }
        public string? TransationId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name ="StreetAddress")]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        public string State { get; set; }
        [Required]
        [Display(Name ="PostalCode")]
        public string PostCode { get; set; }
        [Required]
        [Display(Name ="PhoneNumber")]
        public string PhoneNumber { get; set; }
       
    }
}
