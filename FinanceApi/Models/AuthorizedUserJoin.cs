using Microsoft.AspNetCore.Identity;

namespace FinanceApi.Models
{
    public class AuthorizedUserJoin
    {
        public string OwnerId { get; set; }
        public string AuthorizedUserId { get; set; }
        public User Owner { get; set; }
        public User AuthorizedUser { get; set; }
        public bool CanEdit { get; set; }
    }
}
