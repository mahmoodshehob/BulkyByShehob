using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkySh.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public ProductRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public void Save()
        {
            _dbContext.SaveChanges();
        }

        public void Update(Product obj)
        {
            _dbContext.Products.Update(obj);
        }
    }
}
