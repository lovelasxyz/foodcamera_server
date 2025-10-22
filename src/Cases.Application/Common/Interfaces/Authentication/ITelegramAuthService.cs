using Cases.Application.Common.Models;

namespace Cases.Application.Common.Interfaces.Authentication;

public interface ITelegramAuthService
{
    TelegramUserData ValidateAndParse(string initData);
}
