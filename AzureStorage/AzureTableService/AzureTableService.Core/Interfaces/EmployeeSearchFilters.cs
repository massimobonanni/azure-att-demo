namespace AzureTableService.Core.Interfaces
{
    public class EmployeeSearchFilters : SearchFilters
    {
        public string EmployeeId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}