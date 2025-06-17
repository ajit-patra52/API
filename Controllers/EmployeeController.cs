using Microsoft.AspNetCore.Mvc;
using WebAPI6.Models;
using WebAPI6.Services;

namespace WebAPI6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeServices _employeeService;
        private readonly ILogger<EmployeeController> _logger;


        // Constructor
        public EmployeeController(IEmployeeServices employeeService, ILogger<EmployeeController> logger)
        {
            _employeeService = employeeService ?? throw new ArgumentNullException(nameof(employeeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // GET: api/employee
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var employees = await _employeeService.GetEmployeeList(); 
            if (employees == null || !employees.Any())
            {
                return NotFound("No employees found.");
            }
            return Ok(employees);
        }

        // GET: api/employee/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var employees = await _employeeService.GetById(id); // Await the async method
            if (employees == null)
            {
                return NotFound("No employees found."); // Return 404 if no employees are found
            }
            return Ok(employees); // Return 200 with the list of employees
        }

        // POST: api/employee
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            if (employee == null)
            {
                return BadRequest("Invalid employee data."); // Return 400 if the input is null
            }

            try
            {
                var createdEmployee = await _employeeService.AddEmployee(employee);
                if (createdEmployee == null)
                {
                    return StatusCode(500, "An error occurred while adding the employee."); // Return 500 if the addition failed
                }

                return CreatedAtAction(nameof(Get), new { id = createdEmployee.EmployeeId }, createdEmployee); // Return 201 with the created employee
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding the employee."); // Log the exception
                return StatusCode(500, "An error occurred while adding the employee."); // Return 500 on failure
            }
        }

        // PUT: api/employee/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            if (employee == null || employee.EmployeeId != id)
            {
                return BadRequest("Employee ID mismatch or invalid data.");
            }

            try
            {
                var updatedEmployee = await _employeeService.PutEmployee(employee);
                if (updatedEmployee == null)
                {
                    return NotFound($"Employee with ID {id} not found.");
                }

                return Ok(updatedEmployee); // Return 200 with the updated employee
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., using a logging framework)
                return StatusCode(500, "An error occurred while updating the employee."); // Return 500 on failure
            }
        }

        [HttpPost("generate-dummy-employees")]
        public async Task<IActionResult> GenerateDummyEmployees([FromQuery] int count = 100)
        {
            await _employeeService.GenerateAndAddDummyEmployees(count);
            return Ok($"{count} dummy employees have been added to the database.");
        }

        // DELETE: api/employee/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var isDeleted = await _employeeService.DeleteEmployee(id);
                if (isDeleted)
                {
                    return NoContent(); // Return 204 if the deletion was successful
                }
                return NotFound($"Employee with ID {id} not found."); // Return 404 if the employee does not exist
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while deleting the employee with ID {id}."); // Log the exception
                return StatusCode(500, "An error occurred while deleting the employee."); // Return 500 on failure
            }
        }
    }
}
