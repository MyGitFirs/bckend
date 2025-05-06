using CleanArchitecture.Application.User._Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface IUsersBalancePdfService
    {
        byte[] Generate(List<UserWithBalanceDto> users, string companyName);
    }
}
