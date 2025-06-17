using WebAPI6.Repository.Employee;
using WebAPI6.Services;
using WebAPI6.UnitOfWork;

namespace WebAPI6.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddScopedServices(this IServiceCollection services)
        {
            // Register the UnitOfWork with the DI container
            services.AddScoped<IUnitOfWork, UnitOfWork.UnitOfWork>(); // Fully qualify the UnitOfWork class to avoid namespace conflict
            // Register the EmployeeRepository with the DI container
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            // Register the EmployeeService with the DI container
            services.AddScoped<IEmployeeServices, EmployeeService>();

           
            return services;
        }
    }
}
