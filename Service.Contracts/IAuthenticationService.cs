﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Shared.DataTransferObjects;

namespace Service.Contracts {
    public interface IAuthenticationService {
        Task<IdentityResult> RegisterUser(UserForRegistrationDto userForRegistration);
        Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
        Task<TokenDto> CreateToken(bool populateExp);
        Task<TokenDto> RefreshToken(TokenDto tokenDto);
        string GetCurrUserIdAsync(ClaimsPrincipal claimsPrincipal);
    }
}
