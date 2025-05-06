using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.User._Dto;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.User.Queries.GetUserBalance
{
    public class GetUserBalanceQuery : IRequest<List<UserWithBalanceDto>>
    {
        public Guid CompanyId { get; set; }

        public class GetUserBalanceQueryHandler : IRequestHandler<GetUserBalanceQuery, List<UserWithBalanceDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetUserBalanceQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<UserWithBalanceDto>> Handle(GetUserBalanceQuery request, CancellationToken cancellationToken)
            {
                var result = await _context.Users
                    .Where(u => u.CompanyID == request.CompanyId)
                    .Join(
                        _context.Wallets,
                        user => user.Id,
                        wallet => wallet.UserID, 
                        (user, wallet) => new UserWithBalanceDto
                        {
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Balance = wallet.Balance
                        }
                    )
                    .ToListAsync(cancellationToken);

                return result;
            }
        }
    }
}