using Demo.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.DAL.Context
{
    public class AppDBContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer("server=. ; database=CompanyAppDb; integrated security=true");
        }


        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }

    }
}


// optionsBuilder.UseSqlServer("server=. ; database=CompanyAppDb; integrated security=true");
// C# files => convert to dll file so you can't change connection string 
// so we didn't use this way 
// we add configuration in appsetting.json file => in PL project
// appsetting.json => this file still json and didn't convert to dll file 
// so you can change connection string
