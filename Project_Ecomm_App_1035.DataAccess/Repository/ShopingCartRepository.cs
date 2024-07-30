﻿using Project_Ecomm_App_1035.DataAccess.Data;
using Project_Ecomm_App_1035.DataAccess.Repository.IRepository;
using Project_Ecomm_App_1035.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_Ecomm_App_1035.DataAccess.Repository
{
    public class ShopingCartRepository : Repository<ShopingCart>, IShopingRepository
    {       private readonly ApplicationDbContext _context;
        public ShopingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
