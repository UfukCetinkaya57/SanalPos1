using System.Collections.Generic;
using SanalPos.Domain.Entities;

namespace SanalPos.Application.Common.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        bool ValidateToken(string token);
    }
} 