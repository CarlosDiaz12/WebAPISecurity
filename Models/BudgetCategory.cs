namespace WebAPISecurity.Models
{
    public class BudgetCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }

    public class BudgetCategoryDto
    {
        public string EncryptId { get; set; }
        public string Name { get; set; }
        public decimal Amount { get; set; }
    }
}
