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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CompanyRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public void Save()
        {
            _dbContext.SaveChanges();

        }

        //public void Update(Company company)
        //{
        //    _dbContext.Companies.Update(company);
        //}
    }
}
