using System.ComponentModel.DataAnnotations;

namespace MonthlyExamWithSp.Models.ViewModel
{
    public class ExperienceViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public int Duration { get; set; }
    }
}
