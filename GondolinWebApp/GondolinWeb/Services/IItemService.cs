using GondolinWeb.Areas.Application.Models;

namespace GondolinWeb.Services
{
    public interface IItemService
    {
        Project GetUserQuickList(string userID);
    }
}