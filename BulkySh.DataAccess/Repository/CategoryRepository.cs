using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;

namespace BulkySh.DataAccess.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

        public void Save()
        {
            _dbContext.SaveChanges();

        }

        //public void Update(Category category)
        //{
        //    _dbContext.Categories.Update(category);
        //}
    }
}
