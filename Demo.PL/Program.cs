using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Demo.DAL.Context;
using Demo.DAL.Entities;
using Demo.PL.Mapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Demo.PL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            //builder.Services.AddDbContext<AppDBContext>(options =>
            //{
            //    options.UseSqlServer("server=. ; database=CompanyAppDb; integrated security=true");
            //});


            builder.Services.AddDbContext<AppDBContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();  
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            //builder.Services.AddSingleton<IDepartmentRepository , DepartmentRepository>();
            //builder.Services.AddTransient<IDepartmentRepository , DepartmentRepository>();

            ////builder.Services.AddAutoMapper(typeof(EmployeeProfile));
            //// or
            builder.Services.AddAutoMapper(map => map.AddProfile(new EmployeeProfile()));


            builder.Services.AddAutoMapper(map => map.AddProfile(new DepartmentProfile()));

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
                {
                    options.LoginPath = new PathString("/Account/Login");
                    options.AccessDeniedPath = new PathString("/Home/Error");
                });

            //optional configuration
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {

                options.Password.RequireDigit = true; // must conatin digit
                options.Password.RequireNonAlphanumeric = true; // must contain sympols
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredLength = 6;
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(15);
            })
            .AddEntityFrameworkStores<AppDBContext>()
            .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}


// we should add this service instead of use using(){} to make operation on database 
//builder.Services.AddDbContext<AppDBContext>(options =>
//{
//    options.UseSqlServer("server=. ; database=CompanyAppDb; integrated security=true");
//});
// but note if you add connection string by this way 
// this file convert to dll file so we can't make changes in connection string 

// so we write the connection string in appsettings.json and call by this way 

// in appsetting.json
//,
//  "ConnectionStrings": {
//    "DefaultConnection": "server=. ; database=CompanyAppDb; integrated security=true"
//  }

// note DefaultConnection => connection string key 
// "server=. ; database=CompanyAppDb; integrated security=true" => connection string value 

// and after that you read the connection in start up using this way 

//builder.Services.AddDbContext<AppDBContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});





// note program may have more than one database 



// note AppDBContext => take options as parameter and it didn't have parameterized constructor 
// so we need to add this constructor in AppDBContext => class 
// ctrl + . => select Generate AppDBContext(options)
// this constructor Generated 
//public AppDBContext(DbContextOptions options) : base(options)
//{
//}


// add you override to by Generic 
//public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
//{
//}




// make migration 
// 1. R.C on PL => Make as start up project 
// 2. Download Microsoft.Entityframework.Tools(migration) in PL project 
// 3. on package manager console => default project => DAL 






// note when you work with N Tier => there is only one project run 