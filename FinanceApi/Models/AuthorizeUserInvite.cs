using Microsoft.AspNetCore.Identity;

namespace FinanceApi.Models
{
    public class AuthorizeUserInvite
    {
        public string OwnerId { get; set; }
        public string AuthorizedUserId { get; set; }
        public User Owner { get; set; }
        public User AuthorizedUser{ get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }
}
