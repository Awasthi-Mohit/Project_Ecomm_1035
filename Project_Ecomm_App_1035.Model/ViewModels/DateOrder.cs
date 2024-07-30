using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.Model.ViewModels
{
	public class DateOrder
	{
		public IEnumerable<OrderHeader> OrderHeaders { get; set; }
		public IEnumerable<OrderHeader>orders { get; set; }	
		public Dictionary<string,int>WeeklySummary { get; set; }
		public Dictionary<string, int> MonthlySummary { get; set; }
		public string SelectedInterval { get; set; }	
		public DateTime? StartDate { get; set;}
		public DateTime? EndDate { get; set;}	
		public string SelectedStatus { get; set; }
        public IEnumerable<OrderHeader> Status { get; set; }


    }
}
