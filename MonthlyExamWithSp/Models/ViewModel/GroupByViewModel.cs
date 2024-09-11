namespace MonthlyExamWithSp.Models.ViewModel
{
    public class GroupByViewModel
    {
        public int EmployeeId { get; set; }
        public int Count { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal SumValue { get; set; }
        public decimal AvgValue { get; set; }
    }
}
