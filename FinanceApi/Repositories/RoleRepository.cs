using FinanceApi.Data;
using FinanceApi.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FinanceApi.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext dataContext;

        public RoleRepository(DataContext dataContext)
        {
            this.dataContext = dataContext;
        }

        public IdentityRole GetByName(string name, bool tracking)
        {
            if(tracking)
            {
                return dataContext.Roles.First(r => r.Name.ToLower().Equals(name.ToLower()));
            }
            return dataContext.Roles.AsNoTracking().First(r => r.Name.ToLower().Equals(name.ToLower()));
        }
    }
}
