using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        ICoverTypeRepository CoverType { get; }
        ISPCALL SPCALL { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IShopingRepository Shopping { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IOrderDetailsRepository OrderDetails { get; }
        IApplicationUserRepository ApplicationUser { get; }
        void Save();
    }
}
