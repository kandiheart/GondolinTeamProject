using System.ComponentModel.DataAnnotations;

namespace GondolinWeb.Areas.Application.Models
{
    /// <summary>
    /// Class which represents a project.
    /// </summary>
    public class Project
    {
        /// <summary>
        /// Gets or sets the ID value for the project.
        /// </summary>
        public int ID { get; set; }

        /// <summary>
        /// Gets the date the project was created.
        /// </summary>
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the project detailed description.
        /// </summary>
        [StringLength(1000)]
        [MaxLength(500, ErrorMessage = "The {0} must not be more than {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9-+()&%$#@!:.,-?\s][^;`']*$",
         ErrorMessage = "Some special characters are not allowed such as ; ' \\ <")]
        [DataType(DataType.MultilineText)]
        public string? Description { get; set; }

        /// <summary>
        /// Gets or sets the IsComplete status for the project.
        /// </summary>
        [Display(Name = "Is Complete?")]
        public bool IsComplete { get; set; }

        /// <summary>
        /// Gets or sets the IsArchived status for the project.
        /// </summary>
        public bool IsArchived { get; set; }

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        [Required]
        [DataType(DataType.Text)]
        [MinLength(2, ErrorMessage = "The {0} must be at least {1} characters long.")]
        [MaxLength(30, ErrorMessage = "The {0} must not be more than {1} characters long.")]
        [RegularExpression(@"^[a-zA-Z0-9-+()&%$#@!:.,-?\s][^;`']*$",
         ErrorMessage = "Some special characters are not allowed such as ; ' \\ <")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the date the project is to be completed by. Can be NULL.
        /// </summary>
        [DataType(DataType.DateTime)]
        [Display(Name = "Required Date")]
        public DateTime? RequiredDate { get; set; }
    }
}