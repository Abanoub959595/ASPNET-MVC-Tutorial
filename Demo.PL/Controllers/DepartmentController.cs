using AutoMapper;
using Demo.BLL.Interfaces;
using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Mvc;

namespace Demo.PL.Controllers
{
    public class DepartmentController : Controller
    {
		private readonly IUnitOfWork unitOfWork;

		//private readonly IDepartmentRepository departmentRepository;
		private readonly ILogger<DepartmentController> logger;
        private readonly IMapper mapper;

        public DepartmentController(
            //IDepartmentRepository departmentRepository,
            IUnitOfWork unitOfWork,
            ILogger<DepartmentController> logger,
            IMapper mapper)
        {
			this.unitOfWork = unitOfWork;
			//this.departmentRepository = departmentRepository;
			this.logger = logger;
            this.mapper = mapper;
        }

        public IActionResult Index()
        {
            IEnumerable<Department> departments;
            IEnumerable<DepartmentViewModel> departmentViewModels;

			departments = unitOfWork.DepartmentRepository.GetAll();
			//var departmentviewmodel = mapper.Map<DepartmentViewModel>(departments);
			//ViewBag.Message = "Hello From View Bag";
			//ViewData["MessageData"] = "Hello From View Data";
			// TempData.Keep("MessageSuccess");
			departmentViewModels = mapper.Map<IEnumerable<Department>, IEnumerable<DepartmentViewModel>>(departments);

			return View(departmentViewModels);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new DepartmentViewModel());
        }
        [HttpPost]
        public IActionResult Create(DepartmentViewModel departmentViewModel)
        {
            var department = mapper.Map<Department>(departmentViewModel);
            if (ModelState.IsValid)
            {
				unitOfWork.DepartmentRepository.Add(department);
                unitOfWork.Complete();

                TempData["MessageSuccess"] = "Department Added Successfully";


                return RedirectToAction("Index");

            }
            return View(departmentViewModel);    
            // you didn't need to save changes => beacause you call method in departmentRepository
            // and i implement in this mehtod save changes 

            // return RedirectToAction(nameof(Index));
        }

        public IActionResult Details (int? id)
        {
            try
            {
                if (id == null)
                    return BadRequest(); // 400 (bad request)

				var  department = unitOfWork.DepartmentRepository.GetById(id);
                if (department is null)
                    return NotFound();

                var departmentviewmodel = mapper.Map<DepartmentViewModel>(department);
				return View(departmentviewmodel);


			}
			catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
                // already exist in controller => home
            }
            
            
        }
		public IActionResult Delete(int? id)
        {
            try
            {
                if (id is null)
                    return BadRequest();

                var department = unitOfWork.DepartmentRepository.GetById(id);
                if (department is null)
                    return NotFound();

                var departmentviewmodel = mapper.Map<DepartmentViewModel> (department);
                return View(departmentviewmodel);

            }catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }

        public IActionResult DeleteDepartment(DepartmentViewModel departmentViewModel)
        {
            var department = mapper.Map<Department>(departmentViewModel);
			unitOfWork.DepartmentRepository.Delete(department);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }

		public IActionResult Update(int id)
        {
            try
            {
                if (id == null)
                    return BadRequest();
                var department= unitOfWork.DepartmentRepository.GetById(id);
                if (department is null)
                    return NotFound();

                var departmentviewmodel = mapper.Map<DepartmentViewModel>(department);
                return View(departmentviewmodel);

            }catch (Exception ex)
            {
                logger.LogError(ex.Message);
                return RedirectToAction("Error", "Home");
            }
        }
        [HttpPost]
        public IActionResult Update(DepartmentViewModel departmentViewModel)
        {
            var department = mapper.Map<Department>(departmentViewModel);
			unitOfWork.DepartmentRepository.Update(department);
            unitOfWork.Complete();
            return RedirectToAction("Index");
        }



        

	}
}
