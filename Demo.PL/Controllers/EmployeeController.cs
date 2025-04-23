using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Helper;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Reflection.Metadata;

namespace Demo.PL.Controllers
{
	public class EmployeeController : Controller
	{
		private readonly IUnitOfWork unitOfWork;
		private readonly ILogger<EmployeeController> logger;
        private readonly IMapper mapper;

        public EmployeeController(
			IUnitOfWork unitOfWork,
			ILogger<EmployeeController> logger,
			IMapper mapper)
        {
			this.unitOfWork = unitOfWork;
			this.logger = logger;
            this.mapper = mapper;
        }
        public IActionResult Index(string SearchValue)
		{
			IEnumerable<Employee> employees;
			IEnumerable<EmployeeViewModel> employeesViewModel;
			if (string.IsNullOrEmpty(SearchValue))
				employees = unitOfWork.EmployeeRepository.GetAll();
			else 
				employees = unitOfWork.EmployeeRepository.SearchByName(SearchValue);

			employeesViewModel = mapper.Map<IEnumerable<EmployeeViewModel>>(employees);
			return View(employeesViewModel);
			
		}
		public IActionResult Create ()
		{
			ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
			return View (new EmployeeViewModel());
		}
		[HttpPost]
		public IActionResult Create (EmployeeViewModel employeeViewModel)
		{
			//ModelState["Department"].ValidationState = ModelValidationState.Valid;
			if (ModelState.IsValid)
			{
				// Manual Mapping 
				//Employee employee = new Employee
				//{
				//	Name = employeeViewModel.Name,
				//	Email = employeeViewModel.Email,
				//	Address = employeeViewModel.Address,
				//	DepartmentId = employeeViewModel.DepartmentId,
				//	HireDate = employeeViewModel.HireDate,
				//	Salary = employeeViewModel.Salary,
				//	IsActive = employeeViewModel.IsActive,
				//};

				// another way 

				//employee.Name = employeeViewModel.Name;
				//employee.Email = employeeViewModel.Email;
				//employee.Address = employeeViewModel.Address;
				//employee.DepartmentId = employeeViewModel.DepartmentId;
				//employee.Salary = employeeViewModel.Salary;
				//employee.IsActive = employeeViewModel.IsActive;


				// maping with AutoMapper

				// Employee => TDestination, employeeViewModel => object from TSource 
				//var employee = mapper.Map<Employee>(employeeViewModel);
				// or
				// EmployeeViewModel => TSource, Employee => TDestination 
				var employee = mapper.Map<EmployeeViewModel, Employee>(employeeViewModel);

				employee.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");

				unitOfWork.EmployeeRepository.Add(employee); 

				unitOfWork.Complete();

				return RedirectToAction(nameof(Index));
			}
			ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
			return View(employeeViewModel);
		}
		public IActionResult Details (int? id)
		{
			try
			{
				if (id == null)
					return BadRequest();

				var Employee = unitOfWork.EmployeeRepository.GetById(id);
				if (Employee is null)
					return NotFound();

				ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
				var employeeviewmodel = mapper.Map<EmployeeViewModel>(Employee);	
				return View(employeeviewmodel);
			}catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return RedirectToAction("Error", "Home");
			}
		}
		public IActionResult Update(int? id)
		{
			try
			{
				if (id == null)
					return BadRequest();

				var employee = unitOfWork.EmployeeRepository.GetById(id);
				if(employee is null)
					return NotFound();

				ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
				var employeeviewmodel = mapper.Map<EmployeeViewModel>(employee);
				DocumentSettings.DeleteFile(employeeviewmodel.ImageUrl, "Images");
				return View(employeeviewmodel);
			}catch (Exception ex)
			{
				logger.LogError(ex.Message);
				return RedirectToAction("Error", "Home");

			}
		}
		[HttpPost]
		public IActionResult Update(EmployeeViewModel employeeViewModel)
		{
			employeeViewModel.ImageUrl = DocumentSettings.UploadFile(employeeViewModel.Image, "Images");
			var employee = mapper.Map<Employee>(employeeViewModel);
			unitOfWork.EmployeeRepository.Update(employee);
			unitOfWork.Complete();
			return RedirectToAction("Index");
		}

		public IActionResult Delete(int? id)
		{
			try
			{
				if (id == null)
					return BadRequest();

				var employee = unitOfWork.EmployeeRepository.GetById(id);
				if (employee is null)
					return NotFound();

				ViewBag.Departments = unitOfWork.DepartmentRepository.GetAll();
				var employeeviewmodel = mapper.Map<EmployeeViewModel>(employee);
				
				return View(employeeviewmodel);
			}catch(Exception ex)
			{
				logger.LogError(ex.Message);
				return RedirectToAction("Error", "Home");
			}
		}
		[HttpPost]
		public IActionResult Delete (string employeeId)
		{
			int id = int.Parse(employeeId);
			var employee = unitOfWork.EmployeeRepository.GetById(id);
		 	DocumentSettings.DeleteFile(employee.ImageUrl, "Images");
			unitOfWork.EmployeeRepository.Delete(employee);
			unitOfWork.Complete();
			return RedirectToAction("Index");
		}

	}
}
