using Cases.Application.Common.Models;
using Cases.Domain.Entities;

namespace Cases.Application.Common.Interfaces.Authentication;

public interface ITokenService
{
    TokenResult GenerateToken(User user);
}
