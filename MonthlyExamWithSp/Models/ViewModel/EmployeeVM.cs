using System.ComponentModel.DataAnnotations;

namespace MonthlyExamWithSp.Models.ViewModel
{
    public class EmployeeVM
    {
        public EmployeeVM()
        {
            Experiences = new List<ExperienceViewModel>();
        }

        public int? EmployeeId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsActive { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateOnly JoinDate { get; set; }

        [Display(Name = "Upload Image")]
        public IFormFile ImageFile { get; set; }

        [Required]
        public string ImageName { get; set; }

        [Required]
        public string ImageUrl { get; set; }

        [Required]
        public int Salary { get; set; }

        public List<ExperienceViewModel> Experiences { get; set; }
    }
}
