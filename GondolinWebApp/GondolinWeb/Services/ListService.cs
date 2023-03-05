using GondolinWeb.Areas.Application.Models;
using GondolinWeb.Areas.Identity.Data;
using System.Data;

namespace GondolinWeb.Services
{
    /// <summary>
    /// Class which represents the ListService.
    /// </summary>
    public class ListService : IListService
    {
        /// <summary>
        /// DB context to be user for the query.
        /// </summary>
        private readonly GondolinWebIdentityDBContext _context;

        /// <summary>
        /// Instantiates a new instance of the ListServices class.
        /// Houses methods for standard queries.
        /// </summary>
        /// <param name="context">Context which to be injected.</param>
        public ListService(GondolinWebIdentityDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Query the database, returns a list of projects based on the userID.
        /// Return project is has not been completed.
        /// </summary>
        /// <param name="userId">The user's id which projects are needed.</param>
        /// <returns>List of project objects.</returns>
        public IEnumerable<Project> GetListOfProjects(string userId, int quickListId)
        {
            var projects = (from p in _context.Project
                            join ur in _context.UserProjects on p.ID equals ur.ProjectId
                            where ur.UserId == userId
                                     && p.IsComplete == false
                                     && p.IsArchived == false
                                     && p.ID != quickListId
                            select p).Distinct().ToList();

            return projects;
        }

        /// <summary>
        /// Query the database, returns a list of tasks based on the userID.
        /// This list does not include items on the QuickList or items which have been archived.
        /// </summary>
        /// <param name="userId">The user's id which tasks are needed.</param>
        /// <returns>List of task objects.</returns>
        public IEnumerable<Areas.Application.Models.Task> GetListOfTasks(string userId)
        {
            var tasks = (from t in _context.Tasks
                         join p in _context.Project on t.ProjectID equals p.ID
                         join ur in _context.UserProjects on p.ID equals ur.ProjectId
                         where ur.UserId == userId
                             && t.IsArchived == false
                             && t.IsQuickTask == false
                             && t.IsComplete == false
                         orderby t.RequiredDate ascending
                         select t).ToList();

            var completetasks = (from t in _context.Tasks
                                 join p in _context.Project on t.ProjectID equals p.ID
                                 join ur in _context.UserProjects on p.ID equals ur.ProjectId
                                 where ur.UserId == userId
                                     && t.IsArchived == false
                                     && t.IsQuickTask == false
                                     && t.IsComplete == true
                                 orderby t.RequiredDate ascending
                                 select t).ToList();

            tasks.AddRange(completetasks);

            return tasks;
        }

        /// <summary>
        /// Query the database, returns a list of tasks based on the userID.
        /// This list does not include items on the QuickList or items which have been archived.
        /// </summary>
        /// <param name="userId">The user's id which tasks are needed.</param>
        /// <returns>List of task objects.</returns>
        public IEnumerable<Areas.Application.Models.Task> GetListOfProjectTasks(string userId, int? projectId)
        {
            var tasks = (from t in _context.Tasks
                         join p in _context.Project on t.ProjectID equals p.ID
                         join ur in _context.UserProjects on p.ID equals ur.ProjectId
                         where ur.UserId == userId
                                && p.ID == projectId
                             && t.IsArchived == false
                             && t.IsQuickTask == false
                         orderby t.ProjectID descending
                         select t).Distinct().ToList();

            return tasks;
        }

        /// <summary>
        /// Query the database, returns a list of tasks that falls into the quick list.
        /// Tasks are shown if the project id = -1, is not archived, and is not complete.
        /// </summary>
        /// <param name="userId">The user's id which quick tasks are needed.</param>
        /// <returns>List of quick tasks.</returns>
        public IEnumerable<Areas.Application.Models.Task> GetQuickListOfTasks(string userId, int projectId)
        {
            var tasks = (from t in _context.Tasks
                         join p in _context.Project on t.ProjectID equals projectId
                         join ur in _context.UserProjects on p.ID equals ur.ProjectId
                         where ur.UserId == userId
                             && t.IsQuickTask == true
                             && t.IsArchived == false
                             && t.IsComplete == false
                         select t).Distinct().ToList();

            return tasks;
        }

        /// <summary>
        /// Query the database, returns list of category both default an custom to the user.
        /// </summary>
        /// <param name="userId">The user's id which categories are needed.</param>
        /// <returns>List of categories.</returns>
        public IEnumerable<TaskCategory> GetListOfUserCategories(string userId)
        {
            // Creates default Categories.
            var categories = new List<TaskCategory>();
            var cat = new TaskCategory { Name = "Home" };
            categories.Add(cat);
            cat = new TaskCategory { Name = "School" };
            categories.Add(cat);
            cat = new TaskCategory { Name = "Work" };
            categories.Add(cat);
            cat = new TaskCategory { Name = "Gym" };
            categories.Add(cat);

            // Get list of custom categories the user has created.
            var userCategories = (from tc in _context.TaskCategories
                                  where tc.UserId == userId
                                  && tc.IsArchived == false
                                  select tc).ToList();

            // Add the custom categories to the default list.
            foreach (var category in userCategories)
            {
                categories.Add(category);
            }

            return categories;
        }
    }
}