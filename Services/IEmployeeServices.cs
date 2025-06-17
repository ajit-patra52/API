using WebAPI6.Models;

namespace WebAPI6.Services
{
    public interface IEmployeeServices
    {
        Task<List<Employee>> GetEmployeeList();
        Task<Employee?> GetById(int id);

        Task<Employee?> AddEmployee(Employee employee);

        //Task<Employee?> PatchEmployee(Employee employee);
        Task<Employee?> PutEmployee(Employee employee);
        Task<bool> DeleteEmployee(int id);

        Task GenerateAndAddDummyEmployees(int count);
    }
}
