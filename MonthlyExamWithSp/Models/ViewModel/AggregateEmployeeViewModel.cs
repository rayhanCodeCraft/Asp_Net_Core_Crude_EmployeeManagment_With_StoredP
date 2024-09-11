namespace MonthlyExamWithSp.Models.ViewModel
{
    public class AggregateEmployeeViewModel
    {
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal SumValue { get; set; }
        public decimal AvgValue { get; set; }
        public List<GroupByViewModel> GroupByResult { get; set; }
        public List<EmployeeVM> Employees { get; set; }
    }
}
