using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly AppDBContext context;

		public GenericRepository(AppDBContext context)
        {
			this.context = context;
		}
        public void Add(T entity)
		{
			context.Set<T>().Add(entity);
			//return context.SaveChanges();
		}

		public void Delete(T entity)
		{
			context.Set<T>().Remove(entity);
			//return context.SaveChanges();
		}

		public IEnumerable<T> GetAll()
			=> context.Set<T>().ToList();

		public T GetById(int? id)
			=> context.Set<T>().Find(id);

		public void Update(T entity)
		{
			context.Set<T>().Update(entity);
			//return context.SaveChanges();
		}
	}
}
