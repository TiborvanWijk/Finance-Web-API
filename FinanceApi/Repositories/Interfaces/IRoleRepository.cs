using Microsoft.AspNetCore.Identity;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        IdentityRole GetByName(string name, bool tracking);
    }
}
