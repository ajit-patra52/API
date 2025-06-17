using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Concurrent;
using WebAPI6.Models;
using WebAPI6.Repository;
using WebAPI6.Repository.Employee;

namespace WebAPI6.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (typeof(T) == typeof(Employee))
            {
                if (!_repositories.ContainsKey(typeof(IEmployeeRepository)))
                {
                    var employeeRepository = new EmployeeRepository(_context);
                    _repositories.TryAdd(typeof(IEmployeeRepository), employeeRepository);
                }
                return (IRepository<T>)_repositories[typeof(IEmployeeRepository)];
            }

            if (!_repositories.ContainsKey(typeof(T)))
            {
                var repository = new Repository<T>(_context);
                _repositories.TryAdd(typeof(T), repository);
            }
            return (IRepository<T>)_repositories[typeof(T)];
        }

       

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }
}
