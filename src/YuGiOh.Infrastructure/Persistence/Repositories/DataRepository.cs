using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;

namespace YuGiOh.Infrastructure.Persistence.Repositories
{
    public class DataRepository<T> : RepositoryBase<T>, IRepositoryBase<T>, IReadRepositoryBase<T> where T : class
    {
        public DataRepository(YuGiOhDbContext dbContext) : base(dbContext) { }
    }
}