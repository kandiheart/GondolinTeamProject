using GondolinWeb.Areas.Identity.Data;
using System.ComponentModel.DataAnnotations;

namespace GondolinWeb.Areas.Application.Models
{
    public class TaskCategory
    {
        /// <summary>
        /// Gets or sets the ID value for the category.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the date which the category was created.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the is archived status for the category.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the name to be displayed for the category.
        /// </summary>
        [Required]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(30, ErrorMessage = "The {0} must not be more than {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z''-'\s]{1,30}$",
         ErrorMessage = "Numbers and special characters are not allowed.")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the User id FK
        /// </summary>
        public string? UserId { get; set; }

        /// <summary>
        /// Gets or set the user associated with the category.
        /// </summary>
        public ApplicationUser? ApplicationUser { get; set; }
    }
}