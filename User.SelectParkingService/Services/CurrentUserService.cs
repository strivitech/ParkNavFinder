﻿namespace User.SelectParkingService.Services;

internal class CurrentUserService(IUserSessionData userSessionData) : ICurrentUserService
{
    public IUserSessionData SessionData { get; } = userSessionData ?? throw new ArgumentNullException(nameof(userSessionData));
}