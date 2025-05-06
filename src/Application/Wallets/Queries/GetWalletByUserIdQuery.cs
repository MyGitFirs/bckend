using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Wallets._Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Wallets.Queries.GetWallets
{
    public class GetWalletByUserIdQuery : IRequest<WalletDto>
    {
        public Guid UserId { get; set; }

        public class GetWalletByUserIdQueryHandler : IRequestHandler<GetWalletByUserIdQuery, WalletDto>
        {
            private readonly IApplicationDbContext _context;

            public GetWalletByUserIdQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<WalletDto> Handle(GetWalletByUserIdQuery request, CancellationToken cancellationToken)
            {
                var wallet = await _context.Wallets
                    .Where(w => w.UserID == request.UserId)
                    .Select(w => new WalletDto
                    {
                        Id = w.Id,
                        UserId = w.UserID,
                        Balance = w.Balance,
                        MonthlyLimit = w.MonthlyLimit,
                        LastResetDate = w.LastResetDate
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                return wallet ?? throw new KeyNotFoundException("Wallet not found for the given user ID");
            }
        }
    }
}
