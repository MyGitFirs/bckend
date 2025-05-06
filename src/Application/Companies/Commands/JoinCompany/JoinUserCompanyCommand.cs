using System;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Companies.Commands.JoinCompany
{
    public class JoinUserCompanyCommand : IRequest<string>
    {
        public Guid UserId { get; set; }
        public Guid CompanyId { get; set; }

        public class JoinUserCompanyCmdHandler : IRequestHandler<JoinUserCompanyCommand, string>
        {
            private readonly IApplicationDbContext _context;

            public JoinUserCompanyCmdHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<string> Handle(JoinUserCompanyCommand request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);
                if (user == null)
                {
                    throw new Exception("User not found.");
                }

                var company = await _context.Companies.FirstOrDefaultAsync(c => c.Id == request.CompanyId, cancellationToken);
                if (company == null)
                {
                    throw new Exception("Company not found.");
                }

                var existingRequest = await _context.CompanyJoinRequests
                    .FirstOrDefaultAsync(r => r.UserId == request.UserId && r.CompanyId == request.CompanyId && r.Status == "Pending", cancellationToken);

                if (existingRequest != null)
                {
                    return "You have already requested to join this company.";
                }

                var joinRequest = new CompanyJoinRequest
                {
                    UserId = request.UserId,
                    CompanyId = request.CompanyId,
                    Status = "Pending",
                    CreatedDate = DateTime.Now,
                    UpdatedDate = DateTime.Now
                };

                await _context.CompanyJoinRequests.AddAsync(joinRequest, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return $"Join request submitted to {company.CompanyName}. Waiting for admin approval.";
            }

        }
    }
}
