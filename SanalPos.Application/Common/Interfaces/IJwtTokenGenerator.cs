using SanalPos.Domain.Entities;

namespace SanalPos.Application.Common.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
} 