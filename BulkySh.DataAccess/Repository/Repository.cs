using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using BulkySh.DataAccess.Data;
using BulkySh.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace BulkySh.DataAccess.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		private readonly ApplicationDbContext _dbContext;
		internal DbSet<T> dbSet;


		public Repository(ApplicationDbContext dbContext)
		{
			_dbContext = dbContext;

			this.dbSet = _dbContext.Set<T>();

			//_dbContext.Products.Include(u => u.Category).Include(u => u.CategoryId);

		}

		public void Add(T entity)
		{
			dbSet.Add(entity);
		}

		public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
		{

			IQueryable<T> query = dbSet;

			if (tracked)
			{
				query = query.Where(filter);
			}
			else
			{
				query = query.AsNoTracking().Where(filter);
			}

			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}

			return query.FirstOrDefault();

		}

		public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
		{
			IQueryable<T> query = dbSet;
			if (filter != null)
			{
				query = query.Where(filter);
			}
			if (!string.IsNullOrEmpty(includeProperties))
			{
				foreach (var includeProp in includeProperties
					.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
				{
					query = query.Include(includeProp);
				}
			}
			return query.ToList();
		}
		
		public void Remove(T entity)
		{
			dbSet.Remove(entity);
		}

		public void RemoveRange(IEnumerable<T> entity)
		{
			dbSet.RemoveRange(entity);
		}

		public void Update(T entity)
		{
			dbSet.Update(entity);
		}

	}
}
