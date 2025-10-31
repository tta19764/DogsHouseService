using DogsHouseService.Sevices.Enums;
using DogsHouseService.Sevices.Models;

namespace DogsHouseService.Sevices.Interfaces.Services
{
    public interface IDogService : ICrudService<DogModel>
    {
        public Task<IEnumerable<DogModel>> GetAllSortedAsync(SortBy attribute = SortBy.Name, string order = "asc", int? pageNumber = null, int? pageSize = null);
    }
}
