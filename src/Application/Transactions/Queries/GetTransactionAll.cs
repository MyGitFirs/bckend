using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Transactions._Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Transactions.Queries.GetTransactionAll
{
    public class GetTransactionAllQuery : IRequest<List<TransactionReportDto>>
    {
        public Guid CompanyId { get; set; }
        public DateTime? FromDate { get; set; }  
        public DateTime? ToDate { get; set; }

        public class GetTransactionAllQueryHandler : IRequestHandler<GetTransactionAllQuery, List<TransactionReportDto>>
        {
            private readonly IApplicationDbContext _context;

            public GetTransactionAllQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<List<TransactionReportDto>> Handle(GetTransactionAllQuery request, CancellationToken cancellationToken)
            {
                var query = _context.Transactions
             .Include(t => t.Kiosk)
             .Where(t => t.Kiosk.CompanyId == request.CompanyId);

                if (request.FromDate.HasValue)
                {
                    query = query.Where(t => t.CreatedDate.Date >= request.FromDate.Value.Date);
                }

                if (request.ToDate.HasValue)
                {
                    query = query.Where(t => t.CreatedDate.Date <= request.ToDate.Value.Date);
                }

                var report = await query
                    .GroupBy(t => new
                    {
                        t.Kiosk.KioskName,
                        t.Kiosk.Category,
                        Date = t.CreatedDate.Date
                    })
                    .Select(g => new TransactionReportDto
                    {
                        KioskName = g.Key.KioskName,
                        Category = g.Key.Category,
                        CreatedDate = g.Key.Date,
                        Amount = g.Sum(x => x.Amount)
                    })
                    .ToListAsync(cancellationToken);

                return report;
            }
        }
    }
}
