using Demo.BLL.Interfaces;
using Demo.DAL.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.BLL.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly AppDBContext context;

		public IEmployeeRepository EmployeeRepository { get; set; }
		public IDepartmentRepository DepartmentRepository { get; set; }
        //public UnitOfWork(
        //    IEmployeeRepository employeeRepository,
        //    IDepartmentRepository departmentRepository)
        //{
        //    EmployeeRepository = employeeRepository;    
        //    DepartmentRepository = departmentRepository;
        //}
        public UnitOfWork(AppDBContext context)
        {
			this.context = context;
            EmployeeRepository = new EmployeeRepository(context);   
            DepartmentRepository = new DepartmentRepository(context);
		}

		public int Complete()
		{
			return context.SaveChanges();
		}
	}
}
