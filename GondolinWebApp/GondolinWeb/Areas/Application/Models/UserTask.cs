using System.ComponentModel.DataAnnotations;

namespace GondolinWeb.Areas.Application.Models
{
    public class UserTask
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string? UserId { get; set; }

        [Required]
        public int TaskId { get; set; }

        public Task? Task { get; set; }

        //public User User { get; set; }
    }
}