using DogsHouseService.Services.Database.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DogsHouseService.Services.Database.Repositories
{
    public abstract class AbstractRepository
    {
        protected AbstractRepository(DogsHouseServiceDbContext context) 
        {
            this.Context = context;
        }

        protected DogsHouseServiceDbContext Context { get; }
    }
}
