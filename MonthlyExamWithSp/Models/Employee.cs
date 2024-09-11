namespace MonthlyExamWithSp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; } = null!;

        public bool IsActive { get; set; }

        public DateOnly JoinDate { get; set; }

        public string? ImageName { get; set; }

        public string? ImageUrl { get; set; }

        public int Salary { get; set; }

        public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();
    }
}
