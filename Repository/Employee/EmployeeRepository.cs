
using Microsoft.EntityFrameworkCore;

namespace WebAPI6.Repository.Employee
{
    public class EmployeeRepository : Repository<Models.Employee>, IEmployeeRepository
    {
        private readonly DbContext _context;
        public EmployeeRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task BulkAddEmployee(List<Models.Employee> employees)
        {
            await _context.Set<Models.Employee>().AddRangeAsync(employees);
        }

        public async Task<List<Models.Employee>> GetEmployeesByNameAsync(string name)
        {
            return await _context.Set<Models.Employee>()
               .Where(e => e.Name != null && e.Name.Contains(name))
               .ToListAsync();
        }
    }
}
