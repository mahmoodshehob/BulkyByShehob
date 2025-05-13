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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public OrderDetailRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public void Save()
        {
            _dbContext.SaveChanges();

        }

        public void Update(OrderDetail orderDetail)
        {
            _dbContext.OrderDetails.Update(orderDetail);
        }
    }
}
