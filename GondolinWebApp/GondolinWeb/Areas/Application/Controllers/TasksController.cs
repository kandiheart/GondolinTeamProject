using GondolinWeb.Areas.Application.Models;
using GondolinWeb.Areas.Identity.Data;
using GondolinWeb.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;
using System.Security.Claims;
using System.Threading.Tasks;
using Task = GondolinWeb.Areas.Application.Models.Task;

namespace GondolinWeb.Areas.Application.Controllers
{
    /// <summary>
    /// Class which represents the functionality required for handling tasks.
    /// "[Authorize]" added to protect behind authentication.
    /// </summary>
    [Authorize]
    [Area("Application")]
    public class TasksController : Controller
    {
        /// <summary>
        /// Database Context for application.
        /// </summary>
        private readonly GondolinWebIdentityDBContext _context;

        /// <summary>
        /// Instantiates new instance of the Controller class.
        /// </summary>
        /// <param name="context">DB Context injected.</param>
        public TasksController(GondolinWebIdentityDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Return the index view, creates a list from the UserTask table based on the user that is signed in.
        /// </summary>
        /// <returns>View with the users tasks model.</returns>
        /// GET: Application/Tasks
        public async Task<IActionResult> Index(string sortOrder, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = sortOrder == "Name" ? "name_desc" : "Name";
            ViewData["DateSortParm"] = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewData["CatSortParm"] = sortOrder == "CatName" ? "CatName_desc" : "CatName";
            ViewData["ProjSortParm"] = sortOrder == "ProjName" ? "ProjName_desc" : "ProjName";

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var taskList = from t in _context.Tasks
                           join p in _context.Project on t.ProjectID equals p.ID
                           join ur in _context.UserProjects on p.ID equals ur.ProjectId
                           where ur.UserId == userId
                               && t.IsArchived == false
                               && t.IsQuickTask == false
                               && t.IsFavorite == false
                           select t;

            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            ViewBag.UserQuickProject = appUser.QuickProjectID;
            ViewBag.UserCategories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Theme = appUser.Theme;

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            switch (sortOrder)
            {
                case "name_desc":
                    taskList = taskList.OrderByDescending(t => t.Name);
                    break;

                case "Name":
                    taskList = taskList.OrderBy(t => t.Name);
                    break;

                case "CatName_desc":
                    taskList = taskList.OrderByDescending(t => t.CategoryName);
                    break;

                case "CatName":
                    taskList = taskList.OrderBy(t => t.CategoryName);
                    break;

                case "ProjName_desc":
                    taskList = taskList.OrderByDescending(t => t.ProjectID);
                    break;

                case "ProjName":
                    taskList = taskList.OrderBy(t => t.ProjectID);
                    break;

                case "date_desc":
                    taskList = taskList.OrderByDescending(t => t.RequiredDate);
                    break;

                default:
                    taskList = taskList.OrderBy(t => t.RequiredDate);
                    break;
            }
            int pageSize = 5;

            var favTasks = from t in _context.Tasks
                           join p in _context.Project on t.ProjectID equals p.ID
                           join ur in _context.UserProjects on p.ID equals ur.ProjectId
                           where ur.UserId == userId
                               && t.IsArchived == false
                               && t.IsQuickTask == false
                               && t.IsFavorite == true
                           select t;

            var favs = await favTasks.AsNoTracking().ToListAsync();
            var pgTaskList = await PaginatedList<Task>.CreateAsync(taskList.AsNoTracking(), pageNumber ?? 1, pageSize);

            dynamic taskModel = new ExpandoObject();
            taskModel.FavTasks = favs;
            taskModel.TaskList = pgTaskList;

            return this.View(taskModel);
        }

        //TODO CC: The detail task was built by the template, may need to be removed at later time.
        // GET: Application/Tasks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == task.ProjectID);

            return View(task);
        }

        public async Task<IActionResult> Report(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            var task = await _context.Tasks
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            ViewBag.User = appUser.FirstName + " " + appUser.LastName;
            var project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID).FirstOrDefault(p => p.ID == task.ProjectID);

            ViewBag.Theme = appUser.Theme;

            if (project != null)
            {
                ViewBag.Project = project;
            }

            return View(task);
        }

        /// <summary>
        /// Action which returns the create view.
        /// The ViewBag.Project contains a list that populates the projects drop down.
        /// Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
        /// Return the filtered list of active projects.
        /// </summary>
        /// <returns>Create view.</returns>
        /// GET: Application/Tasks/Create
        public async Task<IActionResult> CreateAsync()
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);
            ViewBag.Category = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Theme = appUser.Theme;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            return View();
        }

        /// <summary>
        /// Processes user input from the form.
        /// Adds the task to the task table, adds userID and taskId to the UserTask table
        /// </summary>
        /// <param name="task">Task which the user created.</param>
        /// <returns></returns>
        // POST: Application/Tasks/Create
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryID,Description,IsComplete,Name,ProjectID,RequiredDate,CategoryName")] Task task)
        {
            if (ModelState.IsValid)
            {
                if (task.ProjectID == 0)
                {
                    ApplicationUser user = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
                    task.ProjectID = user.QuickProjectID;
                }
                task.CreationDate = DateTime.Now;
                _context.Add(task);

                // Below is functionality required to add custom categories.
                // Functionality could be moved to menthod and called for simplification.
                ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

                // Try to get category from TaskCategories table.
                var cat = (from tc in _context.TaskCategories
                           where tc.UserId == appUser.Id
                                   && tc.Name == task.CategoryName
                                   && tc.IsArchived == false
                           select tc).FirstOrDefault();

                // If category does not exist add new TaskCategory.
                if (task.CategoryName != "N/A" && task.CategoryName != "Home" && task.CategoryName != "Work" && task.CategoryName != "School" && task.CategoryName != "Gym")
                {
                    if (cat == null)
                    {
                        var taskCategory = new TaskCategory();
                        taskCategory.CreationDate = DateTime.Now;
                        taskCategory.UserId = appUser.Id;
                        taskCategory.ApplicationUser = appUser;
                        taskCategory.Name = task.CategoryName;
                        _context.Add(taskCategory);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        /// <summary>
        /// Action which returns the create view.
        /// The ViewBag.Project contains a list that populates the projects drop down.
        /// Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
        /// Return the filtered list of active projects.
        /// </summary>
        /// <returns>Create view.</returns>
        /// GET: Application/Tasks/Create
        public IActionResult CreateCategory()
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Theme = appUser.Theme;

            return View();
        }

        /// <summary>
        /// Processes user input from the form.
        /// Adds the the task to the task table, adds userID and taskId to the UserTask table
        /// </summary>
        /// <param name="task">Task which the user created.</param>
        /// <returns></returns>
        // POST: Application/Tasks/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory([Bind("Name")] TaskCategory taskCategory)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

                var currentCategories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

                foreach (var tc in currentCategories)
                {
                    if (tc.Name == taskCategory.Name && tc.IsArchived == false)
                        return RedirectToAction(nameof(Index));
                }

                taskCategory.CreationDate = DateTime.Now;
                taskCategory.UserId = appUser.Id;
                taskCategory.ApplicationUser = appUser;
                _context.Add(taskCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(taskCategory);
        }

        /// <summary>
        /// Action which returns the create view.
        /// The ViewBag.Project contains a list that populates the projects drop down.
        /// Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
        /// Return the filtered list of active projects.
        /// </summary>
        /// <returns>Create view.</returns>
        /// GET: Application/Tasks/Create
        public async Task<IActionResult> CreateFavoriteAsync()
        {
            //
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Categories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Theme = appUser.Theme;

            return View();
        }

        /// <summary>
        /// Processes user input from the form.
        /// Adds the task to the task table, adds userID and taskId to the UserTask table
        /// </summary>
        /// <param name="task">Task which the user created.</param>
        /// <returns></returns>
        // POST: Application/Tasks/Create
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFavorite([Bind("Id,CategoryName,Description,Name,ProjectID")] Task task)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();
                task.CreationDate = DateTime.Now;
                task.ProjectID = appUser.QuickProjectID;
                task.IsFavorite = true;
                //task.RequiredDate = DateTime.Now;
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(task);
        }

        /// <summary>
        /// Return partial QuickListPartial view.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> QuickList()
        {
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            var taskList = new ListService(_context).GetQuickListOfTasks(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            return PartialView("_QuickListPartial", taskList);
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
                //task.RequiredDate = DateTime.Now;
                _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction("index", "Home");
            }
            return RedirectToAction("index", "Home");
        }

        /// <summary>
        /// Action with toggles the IsComplete state for a task.
        /// </summary>
        /// <param name="id">Id for the task to toggle.</param>
        /// <returns></returns>
        public async Task<IActionResult> IsCompleteState(int id)
        {
            var result = _context.Tasks.FirstOrDefault(m => m.Id == id);
            if (result == null)
            {
                return NotFound();
            }
            else
            {
                bool b;
                bool state = result.IsComplete;
                if (state == true)
                {
                    b = false;
                }
                else
                {
                    b = true;
                }
                result.IsComplete = b;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("index");
        }

        /// <summary>
        /// Changes the IsComplete state on a task to complete.
        /// </summary>
        /// <param name="id">The id of the task in which is to change.</param>
        /// <returns>Either not found view, or redirects to index.</returns>
        public async Task<IActionResult> MarkComplete(int id)
        {
            using (_context)
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
            }
            return RedirectToAction("index", "Home");
        }

        //TODO CC The Edit action was built by the template, may need to be removed at later time.
        // GET: Application/Tasks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
            // Return the filtered list of active projects.
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            ViewBag.Categories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Theme = appUser.Theme;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            foreach (var p in ViewBag.Project)
            {
                if (p.ID == task.ProjectID)
                {
                    ViewBag.Max = p.RequiredDate?.ToString("yyyy-MM-dd");
                }
            }

            return View(task);
        }

        //TODO CC The edit action was built by the template, may need to be removed at later time.
        // POST: Application/Tasks/Edit/5
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Name,ProjectID,RequiredDate,CategoryName")] Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            return View(task);
        }

        public async Task<IActionResult> EditFavorite(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
            // Return the filtered list of active projects.
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Categories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Theme = appUser.Theme;

            return View(task);
        }

        //TODO CC The edit action was built by the template, may need to be removed at later time.
        // POST: Application/Tasks/Edit/5
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditFavorite(int id, [Bind("Id,Description,Name,CategoryName")] Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

                    task.ProjectID = appUser.QuickProjectID;

                    task.IsFavorite = true;

                    _context.Update(task);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
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
            return View(task);
        }

        //TODO CC: The delete action was built by the template, may need to be removed at later time.
        // GET: Application/Tasks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            ApplicationUser appUser = _context.Users.Where(u => u.Id == userId).First();

            ViewBag.UserQuickProject = appUser.QuickProjectID;

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            ViewBag.Theme = appUser.Theme;

            return View(task);
        }

        public async Task<IActionResult> DeleteCategory(int? id)
        {
            if (id == null || _context.TaskCategories == null)
            {
                return NotFound();
            }

            var task = await _context.TaskCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (task == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Theme = appUser.Theme;

            return View(task);
        }

        //TODO CC: The delelte action was built by the template, may need to be removed at later time.
        // POST: Application/Tasks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Tasks == null)
            {
                return Problem("Entity set 'GondolinWebIdentityDBContext.Tasks'  is null.");
            }
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                task.IsArchived = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //TODO CC: The delelte action was built by the template, may need to be removed at later time.
        // POST: Application/Tasks/Delete/5
        [HttpPost, ActionName("DeleteCategory")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            if (_context.TaskCategories == null)
            {
                return Problem("Entity set 'GondolinWebIdentityDBContext.TaskCategories'  is null.");
            }
            var tc = await _context.TaskCategories.FindAsync(id);
            if (tc != null)
            {
                tc.IsArchived = true;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }

        /// <summary>
        /// Action which returns the CreateProjectTask view.
        /// Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
        /// Return the filtered list of active projects.
        /// </summary>
        /// <returns>Create view.</returns>
        /// GET: Application/Tasks/Create
        public async Task<IActionResult> CreateProjectTaskAsync(int ProjectID)
        {
            ViewBag.ProjectID = ProjectID;

            ViewBag.Category = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Theme = appUser.Theme;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            foreach (var p in ViewBag.Project)
            {
                if (p.ID == ProjectID)
                {
                    ViewBag.Max = p.RequiredDate?.ToString("yyyy-MM-dd");
                }
            }            

            return View();
        }

        /// <summary>
        /// Processes user input from the form.
        /// Adds the current Project, adds userID and taskId to the UserTask table
        /// </summary>
        /// <param name="task">Task which the user created.</param>
        /// <returns></returns>
        // POST: Application/Tasks/Create
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProjectTask([Bind("ID, CategoryName, Description, IsComplete, Name, ProjectID, RequiredDate")] Task task)
        {
            task.CreationDate = DateTime.Now;

            // Below is functionality required to add custom categories.
            // Functionality could be moved to menthod and called for simplification.
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            // Try to get category from TaskCategories table.
            var cat = (from tc in _context.TaskCategories
                       where tc.UserId == appUser.Id
                               && tc.Name == task.CategoryName
                               && tc.IsArchived == false
                       select tc).FirstOrDefault();

            // If category does not exist add new TaskCategory.
            if (task.CategoryName != "N/A" && task.CategoryName != "Home" && task.CategoryName != "Work" && task.CategoryName != "School" && task.CategoryName != "Gym")
            {
                if (cat == null)
                {
                    var taskCategory = new TaskCategory();
                    taskCategory.CreationDate = DateTime.Now;
                    taskCategory.UserId = appUser.Id;
                    taskCategory.ApplicationUser = appUser;
                    taskCategory.Name = task.CategoryName;
                    _context.Add(taskCategory);
                }
            }

            _context.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Projects", new { id = task.ProjectID });
        }

        public async Task<IActionResult> AddFavoriteTask(int? taskID, int projectID)
        {
            if (taskID == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(taskID);
            if (task == null)
            {
                return NotFound();
            }

            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Theme = appUser.Theme;

            ViewBag.Categories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.ProjectID = projectID;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            foreach (var p in ViewBag.Project)
            {
                if (p.ID == projectID)
                {
                    ViewBag.Max = p.RequiredDate?.ToString("yyyy-MM-dd");
                }
            }

            return View(task);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFavoriteTask([Bind("Id,CategoryName,Description,Name,ProjectID,RequiredDate")] Task task)
        {
            task.CreationDate = DateTime.Now;
            task.IsFavorite = false;
            _context.Add(task);
            await _context.SaveChangesAsync();

            return RedirectToAction("Edit", "Projects", new { id = task.ProjectID });
        }

        //TODO CC The Edit action was built by the template, may need to be removed at later time.
        // GET: Application/Tasks/Edit/5
        public async Task<IActionResult> EditProjectTask(int? id)
        {
            if (id == null || _context.Tasks == null)
            {
                return NotFound();
            }

            var task = await _context.Tasks.FindAsync(id);
            if (task == null)
            {
                return NotFound();
            }

            // Gets the ApplicationUser, passed the application user ID and QuickProjectId into the list service.
            // Return the filtered list of active projects.
            ApplicationUser appUser = _context.Users.Where(u => u.Id == this.User.FindFirstValue(ClaimTypes.NameIdentifier)).First();

            ViewBag.Project = new ListService(_context).GetListOfProjects(this.User.FindFirstValue(ClaimTypes.NameIdentifier), appUser.QuickProjectID);

            ViewBag.Categories = new ListService(_context).GetListOfUserCategories(this.User.FindFirstValue(ClaimTypes.NameIdentifier));

            ViewBag.Theme = appUser.Theme;

            ViewBag.Today = DateTime.Now.ToString("yyyy-MM-dd");

            foreach (var p in ViewBag.Project)
            {
                if (p.ID == task.ProjectID)
                {
                    ViewBag.Max = p.RequiredDate?.ToString("yyyy-MM-dd");
                }
            }

            return View(task);
        }

        //TODO CC The edit action was built by the template, may need to be removed at later time.
        // POST: Application/Tasks/Edit/5
        // To protect from over posting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProjectTask(int id, [Bind("Id,CategoryName,Description,Name,ProjectID,RequiredDate")] Task task)
        {
            if (id != task.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskExists(task.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Edit", "Projects", new { id = task.ProjectID });
            }
            return RedirectToAction("Edit", "Projects", new { id = task.ProjectID });
        }
    }
}