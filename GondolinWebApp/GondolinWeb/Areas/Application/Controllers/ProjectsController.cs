using GondolinWeb.Areas.Application.Models;
using GondolinWeb.Areas.Identity.Data;
using GondolinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Project = GondolinWeb.Areas.Application.Models.Project;

namespace GondolinWeb.Areas.Application.Controllers
{
    [Authorize]
    [Area("Application")]
    public class ProjectsController : Controller
    {
        private readonly GondolinWebIdentityDBContext _context;

        public ProjectsController(GondolinWebIdentityDBContext context)
        {
            _context = context;
        }

        // GET: Application/Projects
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            var projectList = (from p in _context.Project
                               join ur in _context.UserProjects on p.ID equals ur.ProjectId
                               where ur.UserId == userId
                                        && p.IsComplete == false
                                        && p.IsArchived == false
                                        && p.ID != appUser.QuickProjectID
                               select p);

            switch (sortOrder)
            {
                case "name_desc":
                    projectList = projectList.OrderByDescending(t => t.Name);
                    break;

                case "Name":
                    projectList = projectList.OrderBy(t => t.Name);
                    break;

                case "date_desc":
                    projectList = projectList.OrderByDescending(t => t.RequiredDate);
                    break;

                default:
                    projectList = projectList.OrderBy(t => t.RequiredDate);
                    break;
            }

            ViewBag.Theme = appUser.Theme;

            int pageSize = 10;

            return this.View(await PaginatedList<Project>.CreateAsync(projectList.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Application/Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // GET: Application/Projects/Create
        public async Task<IActionResult> Create()
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Theme = appUser.Theme;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
        }

        // POST: Application/Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Description,IsComplete,Name,RequiredDate")] Project project, string submitbutton)
        {
            if (ModelState.IsValid)
            {
                project.CreationDate = DateTime.Now;
                _context.Add(project);
                _context.SaveChanges();
                _context.UserProjects.Add(new UserProject { UserId = this.User.FindFirstValue(ClaimTypes.NameIdentifier), ProjectId = project.ID });
                await _context.SaveChangesAsync();
            }
            else
            {
                submitbutton = string.Empty;
            }

            switch (submitbutton)
            {
                case "IndexCreate":
                    // I'm done button
                    return RedirectToAction(nameof(Index));

                case "AddTasksCreate":
                    // Add Task button
                    return RedirectToAction(nameof(Edit), new { id = project.ID });

                default:
                    // this is not called or used but will return the current view
                    return View(nameof(Create));
            }
        }

        // GET: Application/Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == id);

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.Task = new ListService(_context).GetListOfTasks(this.User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewBag.QuickProjectID = appUser.QuickProjectID;

            ViewBag.Theme = appUser.Theme;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            return View(project);
        }

        // POST: Application/Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Description,IsComplete,Name,RequiredDate")] Project project)
        {
            if (id != project.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProjectExists(project.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(project);
        }

        public async Task<IActionResult> Report(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == id);

            ViewBag.Theme = appUser.Theme;

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.Progress = Convert.ToDouble(Progress(project));
            ViewBag.User = appUser.FirstName + " " + appUser.LastName;
            ViewBag.Tasks = new ListService(_context).GetListOfTasks(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            return View(project);
        }

        public double Progress(Project project)
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var projectTasks = new ListService(_context).GetListOfProjectTasks(appUser.Id, project.ID);

            double done = 0;

            foreach (Models.Task t in projectTasks)
            {
                if (t.IsComplete == true)
                {
                    done++;
                }
            }

            if (projectTasks.Count() <= 0)
            {
                return 1;
            }

            double progress = done / projectTasks.Count();
            progress = Math.Round(progress, 2);

            return progress;
        }

        // GET: Application/Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Project == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == id);

            ViewBag.Theme = appUser.Theme;

            if (project == null)
            {
                return NotFound();
            }

            ViewBag.IsTasksComplete = CheckTaskCompletion(project);

            return View(project);
        }

        // POST: Application/Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Project == null)
            {
                return Problem("Entity set 'GondolinWebIdentityDBContext.Project'  is null.");
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == id);

            if (project != null)
            {
                var projectTasks = new ListService(_context).GetListOfProjectTasks(appUser.Id, project.ID);

                foreach (Models.Task t in projectTasks)
                {
                    if (t.ProjectID == project.ID)
                    {
                        t.IsArchived = true;
                        _context.Update(t);
                        await _context.SaveChangesAsync();
                    }
                }

                project.IsArchived = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public bool CheckTaskCompletion(Project project)
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var projectTasks = new ListService(_context).GetListOfProjectTasks(appUser.Id, project.ID);

            foreach (Models.Task t in projectTasks)
            {
                if (t.ProjectID == project.ID)
                {
                    if (t.IsComplete == false)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public async Task<RedirectToActionResult> AddTaskAsync(int taskId, int projectID)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                task.ProjectID = projectID;
                _context.Update(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = projectID });
        }

        public async Task<RedirectToActionResult> RemoveTaskAsync(int taskId, int projectID)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task != null)
            {
                ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
                task.ProjectID = appUser.QuickProjectID;
                _context.Update(task);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Edit), new { id = projectID });
        }

        private bool ProjectExists(int id)
        {
            return _context.Project.Any(e => e.ID == id);
        }

        /// <summary>
        /// Changes the IsComplete state on a task to true in the Project Edit view.
        /// </summary>
        /// <param name="id">The id of the task in which is to change.</param>
        /// <returns>Either not found view, or redirects to index.</returns>
        public async Task<IActionResult> MarkProjectTaskComplete(int id)
        {
            var result = _context.Tasks.FirstOrDefault(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                result.IsComplete = true;
                await _context.SaveChangesAsync();
            }
            var projectId = result.ProjectID;

            return RedirectToAction("Edit", "Projects", new { id = projectId });
        }

        /// <summary>
        /// Changes the IsComplete state on a task to false in the Project Edit view.
        /// </summary>
        /// <param name="id">The id of the task in which is to change.</param>
        /// <returns>Either not found view, or redirects to index.</returns>
        public async Task<IActionResult> MarkProjectTaskUnComplete(int id)
        {
            var result = _context.Tasks.FirstOrDefault(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                result.IsComplete = false;
                await _context.SaveChangesAsync();
            }
            var projectId = result.ProjectID;

            return RedirectToAction("Edit", "Projects", new { id = projectId });
        }
    }
}