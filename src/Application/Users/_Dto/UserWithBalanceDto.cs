using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Application.User._Dto
{
    public class UserWithBalanceDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
    }
}
