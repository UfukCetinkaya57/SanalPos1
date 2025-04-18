using System;

namespace SanalPos.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        Guid? UserId { get; }
        string UserName { get; }
        bool IsAuthenticated { get; }
    }
} 