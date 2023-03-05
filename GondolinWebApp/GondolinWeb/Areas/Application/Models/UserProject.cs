using GondolinWeb.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace GondolinWeb.Areas.Application.Models
{
    /// <summary>
    /// Class which represents the many to many linking table for the Users and Projects.
    /// </summary>
    public class UserProject
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public int ProjectId { get; set; }

        public Project Project { get; set; }

        public ApplicationUser ApplicationUser { get; set; }
    }
}