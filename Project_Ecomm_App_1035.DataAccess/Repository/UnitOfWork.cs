using Project_Ecomm_App_1035.DataAccess.Data;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Category = new CategoryRepository(_context);
            CoverType= new CoverTypeRepository(_context);
            SPCALL = new SPCALL(_context);
            Product= new ProductRepository(_context);
            Company= new CompanyRepository(_context);
            Shopping = new ShopingCartRepository(_context);
            OrderHeader=new OrderHeaderRepository(_context);
            OrderDetails=new OrderDetailsRepository(_context);
            ApplicationUser=new ApplicationUserRepository(_context);
        }
        public ICategoryRepository Category { private set; get; }

        public ICoverTypeRepository CoverType { private set; get; }
        public ISPCALL SPCALL { private set; get; }
        public IProductRepository Product { private set; get; }
        public ICompanyRepository Company { private set; get; }
        

        public IShopingRepository Shopping { private set; get; }
        public IOrderHeaderRepository OrderHeader { private set; get; }
        public IOrderDetailsRepository OrderDetails { private set; get; }

        public IApplicationUserRepository ApplicationUser { private set; get; }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
