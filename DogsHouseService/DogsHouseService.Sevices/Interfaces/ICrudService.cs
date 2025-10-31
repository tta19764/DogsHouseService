namespace DogsHouseService.Sevices.Interfaces
{
    public interface ICrudService<TModel>
        where TModel : class
    {
        Task<IEnumerable<TModel>> GetAllAsync(int? pageNumber = null, int? pageSize = null);
        Task<TModel?> GetByIdAsync(object id);
        Task<TModel> AddAsync(TModel model);
        Task<TModel> UpdateAsync(TModel model);
        Task DeleteAsync(object id);
    }
}
