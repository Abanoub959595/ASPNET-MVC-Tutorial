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
    public class EmployeeRepository : GenericRepository<Employee> , IEmployeeRepository
    {

        private readonly AppDBContext context;

        // constructor injection (inject AppDBContext in constructor)
        public EmployeeRepository(AppDBContext context) : base (context)
        {
            this.context = context;
            // we didn't create context here => dependance injection help us in this point
            // by register EmployeeRepository as service 
            // context added (because we already added connection string) 

            // you must create private field 
            // place on context => Ctrl + . => create and assign private
        }
        //public int Add(Employee employee)
        //{
        //    context.Employees.Add(employee);
        //    return context.SaveChanges();
        //}

        //public int Delete(Employee employee)
        //{
        //    context.Employees.Remove(employee);
        //    return context.SaveChanges();
        //}

        //public IEnumerable<Employee> GetAll()
        //    => context.Employees.ToList();

        //public Employee GetById(int id)
        //    => context.Employees.Find(id);

		public IEnumerable<Employee> GetEmployeesByDepartmentName(string departmentName)
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Employee> SearchByName(string name)
		{
            var employees = context.Employees.Where(emp => 
                            emp.Name.Trim().ToLower().Contains(name.Trim().ToLower())||
                            emp.Email.Trim().ToLower().Contains(name.Trim().ToLower()));
            return employees;
		}

		// => context.Employees.FirstOrDefault(x => x.Id == id);

		//public int Update(Employee employee)
  //      {
  //          context.Employees.Update(employee);
  //          return context.SaveChanges();
  //      }

    }
}
