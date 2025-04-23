using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.PL.Controllers
{
    //[Authorize]
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<UserController> logger;

        public UserController(
            UserManager<ApplicationUser> userManager,
            ILogger<UserController> logger)
        {
            this.userManager = userManager;
            this.logger = logger;
        }
        
        public async Task<IActionResult> Index (string SearchValue = "")
        {
            List<ApplicationUser> users;
            if (string.IsNullOrEmpty(SearchValue)) 
                users = await userManager.Users.ToListAsync();
            else 
                users = await userManager.Users.Where(
                    user => user.NormalizedEmail.Trim().Contains(SearchValue.Trim().ToUpper())).ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Details (string id, string viewModel= "Details")
        {
            if (id is null)
                return NotFound();

            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            return View(viewModel, user);  
        }
            
        public async Task<IActionResult> Update (string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
                return NotFound();
            if(ModelState.IsValid)
            {
                try
                {
                    var user = await userManager.FindByIdAsync (id);
                    if(user is null)
                        return NotFound();
                    user.UserName = applicationUser.UserName;
                    user.NormalizedUserName = applicationUser.UserName.ToUpper();
                    var result = await userManager.UpdateAsync (user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        logger.LogError(error.Description);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
            return View(applicationUser);
        }
        public async Task<IActionResult> Delete (string id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id, ApplicationUser applicationUser)
        {
            if (id != applicationUser.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var user = await userManager.FindByIdAsync(id);
                    if (user is null)
                        return NotFound();
                    var result = await userManager.DeleteAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    foreach(var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                        logger.LogError(error.Description);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
            return View(applicationUser);
        }
    }
}
