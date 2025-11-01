using DogsHouseService.Services.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Services.Database.Repositories
{
    /// <summary>
    /// Abstract base repository class
    /// </summary>
    /// <param name="context">Database context</param>
    public abstract class AbstractRepository(DogsHouseServiceDbContext context)
    {
        /// <summary>
        /// Database context
        /// </summary>
        protected DogsHouseServiceDbContext Context { get; } = context;
    }
}
