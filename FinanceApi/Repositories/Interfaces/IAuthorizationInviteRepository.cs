﻿using FinanceApi.Models;

namespace FinanceApi.Repositories.Interfaces
{
    public interface IAuthorizationInviteRepository
    {
        ICollection<AuthorizeUserInvite> GetAllSentRequest(string ownerId);
        ICollection<AuthorizeUserInvite> GetPendingRequestsForUser(string userId);
        bool RequestExists(string ownerId, string authorizedUserId);
        bool Create(AuthorizeUserInvite authorizationRequest);
        bool Delete(string ownerId, string authorizedUserId);
        bool Save();
    }
}
