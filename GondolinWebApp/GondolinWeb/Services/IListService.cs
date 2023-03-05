using GondolinWeb.Areas.Application.Models;

namespace GondolinWeb.Services
{
    public interface IListService
    {
        IEnumerable<Areas.Application.Models.Task> GetListOfTasks(string userId);

        IEnumerable<Project> GetListOfProjects(string userId, int QuickListId);

        IEnumerable<Areas.Application.Models.Task> GetQuickListOfTasks(string userId, int projectId);

        IEnumerable<Areas.Application.Models.Task> GetListOfProjectTasks(string userId, int? projectId);

        IEnumerable<TaskCategory> GetListOfUserCategories(string userId);
    }
}