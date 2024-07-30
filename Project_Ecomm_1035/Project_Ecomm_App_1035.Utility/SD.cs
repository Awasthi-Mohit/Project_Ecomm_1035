using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Utility
{
    public static class SD
    {
        //cover type stored procedure
        public const string Proc_GetCoverTypes = "GetCoverTypes";
        public const string Proc_GetCoverType = "GetCoverType";
        public const string Proc_CreateCoverType = "CreateCoverType";
        public const string Proc_UpdateCoverType = "UpdateCoverType";
        public const string Proc_DeleteCoverType = "DeleteCoverType";
        // ROLES
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee User";
        public const string Role_Company = "Company User";
        public const string Role_Individual = "Individual User";
        //Sessions
        public const string Ss_CartSessionCount = "Cart Count Session";

        //satus of the order parameter
        public const string orderstatuspemding = "Pending";
        public const string orderstatusApproved = "Approved";
        public const string oderstatusinprogress = "Processing";
        public const string orderstausshipped = "Shipped";
        public const string orderstatusrefund = "Refund";
        public const string OrderStatusCancelled = "Cancelled";

        //for Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayPayment = "PaymentStatusDelay";
        public const string PaymentstatusRejected = "Rejected";
        //method for the price
        public static double GetPriceBasedOnQuality(double Quantity,double price,double price50,double price100)
        {
            if (Quantity < 50)
                return price;
            else if (Quantity > 100)
                return price50;
            else return price100;   

        }
        //Code for the SMS

        
    }
}
