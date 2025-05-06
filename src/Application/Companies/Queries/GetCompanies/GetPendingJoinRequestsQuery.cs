using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Companies._Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Company.Queries.GetJoinRequests
{
    public class GetPendingJoinRequestsQuery : IRequest<List<CompanyJoinRequest>>
    {
        public Guid CompanyId { get; set; }

        public class Handler : IRequestHandler<GetPendingJoinRequestsQuery, List<CompanyJoinRequest>>
        {
            private readonly IApplicationDbContext _context;

            public Handler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<CompanyJoinRequest>> Handle(GetPendingJoinRequestsQuery request, CancellationToken cancellationToken)
            {
                var pendingRequests = await _context.CompanyJoinRequests
                    .Where(r => r.CompanyId == request.CompanyId && r.Status == "Pending")
                    .Select(r => new CompanyJoinRequest
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        CompanyId = r.CompanyId,
                        Status = r.Status,
                        CreatedDate = r.CreatedDate
                    })
                    .ToListAsync(cancellationToken);

                return pendingRequests;
            }
        }
    }
}
