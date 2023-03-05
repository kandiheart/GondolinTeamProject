using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GondolinWeb.Areas.Application.Models
{
    /// <summary>
    /// Class which represents a Task.
    /// </summary>
    public class Task
    {
        /// <summary>
        /// Gets or sets the ID value for the task.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the Category ID value for the task.
        /// </summary>
        [Display(Name = "Category")]
        public int CategoryID { get; set; }

        /// <summary>
        /// Gets or sets the Category name value assigned to the task.
        /// </summary>
        [Required]
        [Display(Name = "Category")]
        [RegularExpression(@"^[a-zA-Z0-9-+()&%$#@!:.,-?\s][^;`']*$",
         ErrorMessage = "Some special characters are not allowed such as ; ' \\ <")]
        public string CategoryName { get; set; } = "N/A";

        /// <summary>
        /// Gets the date the task was created.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the Task detailed description.
        /// </summary>
        [StringLength(1000)]
        [MaxLength(500, ErrorMessage = "The {0} must not be more than {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9-+()&%$#@!:.,-?\s][^;`']*$",
         ErrorMessage = "Some special characters are not allowed such as ; ' \\ <")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or set the Archived status for the task.
        /// If IsArchieve, will be removed from the list visibility.
        /// </summary>
        [Display(Name = "Archived")]
        [Column("IsArchived")]
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the IsComplete status for the task.
        /// </summary>
        [Display(Name = "Is Complete?")]
        public bool IsComplete { get; set; }

        /// <summary>
        /// Gets or sets the IsFavorite status for the task.
        /// If a task is favorite, allows the user to select add create new from a list.
        /// </summary>
        [Display(Name = "Favorited")]
        public bool IsFavorite { get; set; }

        /// <summary>
        /// Gets or sets the IsQuickTask status for the task.
        /// If is quick will be shown on the quick list on dashboard.
        /// </summary>
        [Display(Name = "Quick Task")]
        public bool IsQuickTask { get; set; }

        /// <summary>
        /// Gets or sets the name of the task.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(30, ErrorMessage = "The {0} must not be more than {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9-+()&%$#@!:.,-?\s][^;`']*$",
         ErrorMessage = "Some special characters are not allowed such as ; ' \\ <")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the Project ID if the task is associated with a project. Can be NULL.
        /// </summary>
        [Display(Name = "Project")]
        public int ProjectID { get; set; }

        /// <summary>
        /// Gets or sets the date the task is to be completed by. Can be NULL.
        /// </summary>
        // [Required] causes Quick & Favorite lists to not show content.
        [DataType(DataType.DateTime)]
        [Display(Name = "Required Date")]
        public DateTime? RequiredDate { get; set; }
    }
}