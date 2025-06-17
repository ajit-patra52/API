namespace WebAPI6.Repository.Employee
{
    public interface IEmployeeRepository : IRepository<Models.Employee>
    {
        Task<List<Models.Employee>> GetEmployeesByNameAsync(string name);
        Task BulkAddEmployee(List<Models.Employee> employees);
    }
}
