using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI6.Models;
using WebAPI6.Repository.Employee;
using WebAPI6.UnitOfWork;

namespace WebAPI6.Services
{
    public class EmployeeService : IEmployeeServices
    {
        //private readonly AppDbContext _context;
        private readonly ILogger<EmployeeService> _logger;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;

        // Update the constructor to ensure _employeeRepository is initialized correctly
        public EmployeeService(IUnitOfWork unitOfWork, ILogger<EmployeeService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _employeeRepository = _unitOfWork.Repository<Employee>() as IEmployeeRepository
                                  ?? throw new InvalidOperationException("Failed to initialize IEmployeeRepository.");
            _logger = logger;
        }

        public async Task<Employee?> AddEmployee(Employee employee)
        {
            if (employee == null) return null; // Invalid input

            try
            {
                var newEmployee = new Employee
                {
                    Name = employee.Name,
                    Address = employee.Address,
                    CompanyName = employee.CompanyName,
                    Designation = employee.Designation,
                    Gender = employee.Gender
                };
                //await _context.Employees.AddAsync(newEmployee); // Add to context
                //await _context.SaveChangesAsync(); // Save changes

                
                await _employeeRepository.AddAsync(newEmployee);
                await _unitOfWork.SaveChangesAsync();               
                return newEmployee; // Return the added employee
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding an employee.");
                return null; // Operation failed
            }
        }     


        public async Task<Employee?> GetById(int id)
        {
            try
            {
                // Use AsNoTracking for read-only operations to improve performance
                //return await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.EmployeeId == id) ?? new Employee();
                //return await _unitOfWork.Repository<Employee>().GetByIdAsync(id) ?? new Employee(); // Use repository method
                return await _employeeRepository.GetByIdAsync(id) ?? null; // Use repository method

            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return null; // Return null on failure
            }
        }

        public async Task<List<Employee>> GetEmployeeList(string? name)
        {
            try
            {
                // Ensure 'name' is not null before passing it to the repository method
                if (string.IsNullOrEmpty(name))
                {
                    return new List<Employee>(); // Return an empty list if 'name' is null or empty
                }

                return await _employeeRepository.GetEmployeesByNameAsync(name) ?? new List<Employee>();
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return new List<Employee>(); // Return an empty list on failure
            }
        }

        //public async Task<Employee?> PatchEmployee(Employee employee)
        //{
        //    if (employee == null) return null; // Invalid input

        //    try
        //    {
        //        var emp = await _context.Employees.FirstOrDefaultAsync(x => x.EmployeeId == employee.EmployeeId);
        //        if (emp != null)
        //        {
        //            // Update only the modified fields
        //            _context.Entry(emp).CurrentValues.SetValues(employee);
        //            await _context.SaveChangesAsync();
        //            return emp; // Return the updated employee
        //        }
        //        return null; // Employee not found
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (e.g., using a logging framework)
        //        return null; // Operation failed
        //    }
        //}

        public async Task<Employee?> PutEmployee(Employee employee)
        {
            if (employee == null) return null; // Invalid input

            try
            {
                // Use repository method to update the employee
                await _employeeRepository.UpdateAsync(employee);

                // Save changes to ensure the update is persisted
                await _unitOfWork.SaveChangesAsync();

                return employee; // Return the updated employee
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                _logger.LogError(ex, "An error occurred while updating an employee.");
                return null; // Operation failed
            }
        }

        public async Task<bool> DeleteEmployee(int id)
        {
            try
            {
                // Use repository method to delete the employee
              var res= await _employeeRepository.DeleteAsync(id);
                if (res)
                {
                    // Save changes to ensure the deletion is persisted
                    await _unitOfWork.SaveChangesAsync();

                    return res; // Return true if the deletion was successful
                }
                return false; // Return false if the employee was not found or deletion failed
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                _logger.LogError(ex, "An error occurred while deleting an employee By Id:{id}.", id);
                return false; // Return false on failure
            }
        }

        public async Task<List<Employee>> GetEmployeeList()
        {
            
            try
            {
                //var repository = _unitOfWork.Repository<Employee>();
                // Use the repository to get the list of employees
                // If you want to use the DbContext directly, you can uncomment the following line:
                // return await _context.Employees.AsNoTracking().ToListAsync();
                return await _employeeRepository.GetAllAsync() as List<Employee> ?? new List<Employee>(); // Use repository method
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return new List<Employee>(); // Return an empty list on failure
            }
        }

        private List<Models.Employee> GenerateDummyEmployees(int count)
        {
            var employees = new List<Models.Employee>();
            for (int i = 1; i <= count; i++)
            {
                employees.Add(new Models.Employee
                {
                    Name = $"Employee {i}",
                    Address = $"Address {i}",
                    CompanyName = $"Company {i % 5 + 1}", // Assign to one of 5 companies
                    Designation = $"Designation {i % 3 + 1}", // Assign to one of 3 designations
                    Gender = i % 2 == 0 ? "Male" : "Female"
                });
            }
            return employees;
        }

        public async Task GenerateAndAddDummyEmployees(int count)
        {
            var dummyEmployees = GenerateDummyEmployees(count);
            await _employeeRepository.BulkAddEmployee(dummyEmployees);
            await _unitOfWork.SaveChangesAsync(); // Ensure changes are saved to the database
        }
    }
}
