using WebAPI6.Repository;

namespace WebAPI6.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
        IRepository<T> Repository<T>() where T : class; // Generic repository access


    }

}
