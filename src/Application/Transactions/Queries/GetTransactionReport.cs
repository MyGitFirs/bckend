using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Transactions._Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Transactions.Queries.GetTransactionDetailedReport
{
    public class GetTransactionDetailedReportQuery : IRequest<List<TransactionDto>>
    {
        public Guid CompanyId { get; set; }
        public string FullName { get; set; } 
        public DateTime? Date { get; set; }  
    }

    public class GetTransactionDetailedReportQueryHandler : IRequestHandler<GetTransactionDetailedReportQuery, List<TransactionDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetTransactionDetailedReportQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TransactionDto>> Handle(GetTransactionDetailedReportQuery request, CancellationToken cancellationToken)
        {
            var query = _context.Transactions
        .Include(t => t.Kiosk)
        .Include(t => t.User)
        .Where(t => t.Kiosk.CompanyId == request.CompanyId);

            if (!string.IsNullOrEmpty(request.FullName))
            {
                query = query.Where(t =>
                    (t.User.FirstName + " " + t.User.LastName).Contains(request.FullName));
            }

            // Apply the filter for the exact date if provided
            if (request.Date.HasValue)
            {
                var date = request.Date.Value.Date; 
                query = query.Where(t => t.CreatedDate.Date == date);
            }

            var result = await query
                .Select(t => new TransactionDto
                {
                    Id = t.Id,
                    UserId = t.User.Id,
                    FirstName = t.User.FirstName,
                    LastName = t.User.LastName,
                    KioskId = t.Kiosk.Id,
                    KioskName = t.Kiosk.KioskName,
                    Category = t.Kiosk.Category,
                    Amount = t.Amount,
                    CreatedDate = t.CreatedDate
                })
                .OrderBy(t => t.CreatedDate)
                .ToListAsync(cancellationToken);

            return result;
           
        }
    }
}

//var result = await _context.Transactions
//    .Include(t => t.Kiosk)
//    .Include(t => t.User)
//    .Where(t => t.Kiosk.CompanyId == request.CompanyId)
//    .Select(t => new TransactionDto
//    {
//        Id = t.Id,
//        UserId = t.User.Id,
//        FirstName = t.User.FirstName,
//        LastName = t.User.LastName,
//        KioskId = t.Kiosk.Id,
//        KioskName = t.Kiosk.KioskName,
//        Category = t.Kiosk.Category,
//        Amount = t.Amount,
//        CreatedDate = t.CreatedDate
//    })
//    .OrderBy(t => t.CreatedDate)
//    .ToListAsync(cancellationToken);

//return result;