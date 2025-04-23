using Demo.DAL.Entities;
using Demo.PL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Demo.PL.Controllers
{
    [Authorize(Roles ="Admin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ILogger<RoleController> logger;

        public RoleController(RoleManager<ApplicationRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger<RoleController> logger)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
            this.logger = logger;
        }
        public async Task<IActionResult> Index(string searchValue = "")
        {
            IEnumerable<ApplicationRole> roles;
            if (string.IsNullOrEmpty(searchValue))
                roles = await roleManager.Roles.ToListAsync();
            else
                roles = await roleManager.Roles.Where(
                    role => role.Name.Trim().ToLower().Contains(searchValue.Trim().ToLower())).ToListAsync();
            return View(roles);
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationRole applicationRole)
        {
            if (applicationRole == null)
                return NotFound();
            if (ModelState.IsValid)
            {
                var result = await roleManager.CreateAsync(applicationRole);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                    logger.LogError(error.Description);
                }
            }
            return View(applicationRole);
        }
        public async Task<IActionResult> Details(string id, string viewName = "Details")
        {
            if (id is null)
                return NotFound();
            var role = await roleManager.FindByIdAsync(id);
            if (role is null)
                return NotFound();
            return View(viewName, role);
        }
        public async Task<IActionResult> Update(string id)
        {
            return await Details(id, "Update");
        }
        [HttpPost]
        public async Task<IActionResult> Update(string id, ApplicationRole applicationRole)
        {
            if (id != applicationRole.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await roleManager.FindByIdAsync(id);
                    if (role is null)
                        return NotFound();
                    role.Name = applicationRole.Name;
                    role.NormalizedName = applicationRole.Name.ToUpper();
                    var result = await roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    foreach (var error in result.Errors)
                    {
                        logger.LogError(error.Description);
                        ModelState.AddModelError("", error.Description);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);

                }
            }
            return View(applicationRole);
        }
        public async Task<IActionResult> Delete(string id)
        {
            return await Details(id, "Delete");
        }
        [HttpPost]
        public async Task<IActionResult> Delete(string id, ApplicationRole applicationRole)
        {
            if (id != applicationRole.Id)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await roleManager.FindByIdAsync(id);
                    if (role is null)
                        return NotFound();
                    var result = await roleManager.DeleteAsync(role);
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
            return View(applicationRole);
        }
        public async Task<IActionResult> AddOrRemoveUsers(string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                return NotFound();

            ViewBag.RoleId = roleId;

            var UsersInRole = new List<UserInRoleViewModel>();
            var users = await userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                var userInRole = new UserInRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                    userInRole.IsSelected = true;
                else
                    userInRole.IsSelected = false;
                UsersInRole.Add(userInRole);
            }
            return View(UsersInRole);

        }
        [HttpPost]
        public async Task<IActionResult> AddOrRemoveUsers(string roleId, List<UserInRoleViewModel> users)
        {
            var role = await roleManager.FindByIdAsync(roleId);
            if (role is null)
                return NotFound();
            if (ModelState.IsValid)
            {
                foreach (var user in users)
                {
                    var appUser = await userManager.FindByIdAsync(user.UserId);
                    if (appUser is null)
                        return NotFound();
                    if (appUser is not null)
                    {
                        if (user.IsSelected == true && !await userManager.IsInRoleAsync(appUser, role.Name))
                            await userManager.AddToRoleAsync(appUser, role.Name);
                        else if (user.IsSelected == false && await userManager.IsInRoleAsync(appUser, role.Name))
                            await userManager.RemoveFromRoleAsync(appUser, role.Name);
                    }
                }
                return RedirectToAction("Update", new { Id = roleId });
            }
            return View(users);

        }

    }
}
