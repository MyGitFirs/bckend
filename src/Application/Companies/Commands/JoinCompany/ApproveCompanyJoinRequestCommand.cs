using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Companies.Commands.ApproveJoinRequest
{
    public class ApproveCompanyJoinRequestCommand : IRequest<string>
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }

        public class Handler : IRequestHandler<ApproveCompanyJoinRequestCommand, string>
        {
            private readonly IApplicationDbContext _context;

            public Handler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(ApproveCompanyJoinRequestCommand request, CancellationToken cancellationToken)
            {
                var joinRequest = await _context.CompanyJoinRequests
                    .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.CompanyId == request.CompanyId && r.Status == "Pending", cancellationToken);

                if (joinRequest == null)
                {
                    throw new Exception("Pending join request not found.");
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                // Approve the request
                joinRequest.Status = "Approved";
                joinRequest.UpdatedDate = DateTime.Now;

                // Assign company to user
                user.CompanyID = request.CompanyId;
                user.UpdatedDate = DateTime.Now;

                await _context.SaveChangesAsync(cancellationToken);

                return $"User {user.FirstName} {user.LastName} has been approved to join the company.";
            }
        }
    }
}
