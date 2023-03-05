using GondolinWeb.Areas.Application.Models;
using GondolinWeb.Areas.Identity.Data;
using GondolinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Task = GondolinWeb.Areas.Application.Models.Task;

namespace GondolinWeb.Areas.Application.Controllers
{
    [Authorize]
    [Area("Application")]
    public class HomeController : Controller
    {
        /// <summary>
        /// Database Context for application.
        /// </summary>
        private readonly GondolinWebIdentityDBContext _context;

        /// <summary>
        /// Instantiates the homecontroller class.
        /// </summary>
        /// <param name="context">DB context for the application.</param>
        public HomeController(GondolinWebIdentityDBContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// Is called when user navigates to the Dashboard page (application/index)
        /// </summary>
        /// <returns>Returns the index view.</returns>
        public async Task<IActionResult> Index(string sortOrder)
        {
            ViewData["PNameSortParm"] = sortOrder == "PName" ? "Pname_desc" : "PName";
            ViewData["PDateSortParm"] = sortOrder == "PDate" ? "Pdate_desc" : "PDate";
            ViewData["TNameSortParm"] = sortOrder == "TName" ? "Tname_desc" : "TName";
            ViewData["TDateSortParm"] = sortOrder == "TDate" ? "Tdate_desc" : "TDate";

            CreateQuickList();

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            var projectList = (from p in _context.Project
                               join ur in _context.UserProjects on p.ID equals ur.ProjectId
                               where ur.UserId == userId
                                        && p.IsComplete == false
                                        && p.IsArchived == false
                                        && p.ID != appUser.QuickProjectID
                               select p).OrderBy(p => p.RequiredDate);

            var taskList = (from t in _context.Tasks
                            join p in _context.Project on t.ProjectID equals p.ID
                            join ur in _context.UserProjects on p.ID equals ur.ProjectId
                            where ur.UserId == userId
                                && t.IsArchived == false
                                && t.IsQuickTask == false
                                && t.IsFavorite == false
                                && t.IsComplete == false
                                && t.ProjectID != appUser.QuickProjectID
                            select t).OrderBy(t => t.RequiredDate);

            switch (sortOrder)
            {
                case "Pname_desc":
                    projectList = projectList.OrderByDescending(t => t.Name);
                    break;

                case "PName":
                    projectList = projectList.OrderBy(t => t.Name);
                    break;

                case "Pdate_desc":
                    projectList = projectList.OrderByDescending(t => t.RequiredDate);
                    break;

                case "PDate":
                    projectList = projectList.OrderBy(t => t.RequiredDate);
                    break;

                case "Tname_desc":
                    taskList = taskList.OrderByDescending(t => t.Name);
                    break;

                case "TName":
                    taskList = taskList.OrderBy(t => t.Name);
                    break;

                case "Tdate_desc":
                    taskList = taskList.OrderByDescending(t => t.RequiredDate);
                    break;

                case "TDate":
                    taskList = taskList.OrderBy(t => t.RequiredDate);
                    break;
            }

            appUser.Projects = await projectList.Take(5).AsNoTracking().ToListAsync();
            appUser.Tasks = await taskList.Take(5).AsNoTracking().ToListAsync();
            ViewBag.UserQuickProject = appUser.QuickProjectID;

            ViewBag.Theme = appUser.Theme;

            PartialView("_DashboardProjectsPartial", appUser.Projects);
            PartialView("_DashboardTasksPartial", appUser.Tasks);

            return View();
        }

        /// <summary>
        /// Creates a quicklist for the user if this is the users first time at the dashboard.
        /// </summary>
        private void CreateQuickList()
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            var userProjects = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            // If the users QuickProject property = 0
            // Create a new Project then assigns the the project id to the
            // user QuickProjectID
            if (appUser.QuickProjectID == 0)
            {
                Project quickProject = new Project();
                quickProject.Name = "QuickList";
                quickProject.Description = "Users Default QuickList";

                _context.Add(quickProject);
                _context.SaveChanges();
                _context.Add(new UserProject { UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier), ProjectId = quickProject.ID });
                appUser.QuickProjectID = quickProject.ID;
                _context.Update(appUser);
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Adds a quick task to the quick task list.
        /// </summary>
        /// <param name="task">The task to be added.</param>
        /// <returns>The application index view (dashboard)</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreate([Bind("Description,Name,ProjectID")] Task task)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
                task.CreationDate = DateTime.Now;
                task.ProjectID = appUser.QuickProjectID;
                task.IsQuickTask = true;
                _context.Add(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Changes the theme to the Default theme.
        /// </summary>
        /// <returns>The Dashboard view.</returns>
        public ActionResult GreenTheme()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            appUser.Theme = "Green";

            _context.Update(appUser);

            _context.SaveChanges();

            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Changes the theme to the Dark theme.
        /// </summary>
        /// <returns>The Dashboard view.</returns>
        public ActionResult DarkTheme()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            appUser.Theme = "Dark";

            _context.Update(appUser);

            _context.SaveChanges();

            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Changes the theme to the Light theme.
        /// </summary>
        /// <returns>The Dashboard view.</returns>
        public ActionResult LightTheme()
        {
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            appUser.Theme = "Light";

            _context.Update(appUser);

            _context.SaveChanges();

            return RedirectToAction("index", "Home");
        }
    }
}