using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository.IRepository;
using BulkySh.Models.Models;

namespace BulkySh.DataAccess.Repository
{
	public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
	{

		private readonly ApplicationDbContext _dbContext;

        public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;

        }

		public void Update(ApplicationUser applicationUser)
		{
			_dbContext.ApplicationUsers.Update(applicationUser);
		}
	}
}
