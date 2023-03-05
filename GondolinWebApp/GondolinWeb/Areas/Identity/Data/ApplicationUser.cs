using GondolinWeb.Areas.Application.Models;
using Microsoft.AspNetCore.Identity;

namespace GondolinWeb.Areas.Identity.Data;

// Add profile data for application users by adding properties to the ApplicationUSer class
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public ICollection<Application.Models.Task> Tasks { get; set; }

    public ICollection<Project> Projects { get; set; }

    public int QuickProjectID { get; set; }

    public string Theme { get; set; }

    public ICollection<TaskCategory> TaskCategories { get; set; }
}