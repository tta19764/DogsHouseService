namespace DogsHouseService.Services.Database.Interfaces
{
    public interface IRepository<TEntity> 
        where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<IEnumerable<TEntity>> GetAllAsync(int pageNumber, int pageSize);
        Task<TEntity?> GetByIdAsync(object id);
        Task<TEntity> AddAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task DeleteAsync(object id);
    }
}
