using GondolinWeb.Areas.Application.Models;
using GondolinWeb.Areas.Identity.Data;

namespace GondolinWeb.Services
{
    public class ItemService : IItemService
    {
        /// <summary>
        /// DB context to be user for the query.
        /// </summary>
        private readonly GondolinWebIdentityDBContext _context;

        /// <summary>
        /// Instantiates a new instance of the ItemServices class.
        /// Houses methods for standard queries.
        /// </summary>
        /// <param name="context">Context which to be injected.</param>
        public ItemService(GondolinWebIdentityDBContext context)
        {
            _context = context;
        }

        public Project GetUserQuickList(string userId)
        {
            return null;
        }
    }
}