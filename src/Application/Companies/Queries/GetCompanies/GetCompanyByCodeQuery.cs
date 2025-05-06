using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Companies._Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Application.Company.Queries.GetCompanies
{
    public class GetCompanyByCodeQuery : IRequest<CompanyDto>
    {
        public string CompanyCode { get; set; }

        public class GetCompanyByCodeQueryHandler : IRequestHandler<GetCompanyByCodeQuery, CompanyDto>
        {
            private readonly IApplicationDbContext _context;

            public GetCompanyByCodeQueryHandler(IApplicationDbContext context)
            {
                _context = context;
            }

            public async Task<CompanyDto> Handle(GetCompanyByCodeQuery request, CancellationToken cancellationToken)
            {
                var company = await _context.Companies
                    .Where(c => c.CompanyCode == request.CompanyCode)
                    .Select(c => new CompanyDto
                    {
                        Id = c.Id,
                        CompanyName = c.CompanyName,
                        BusinessCategory = c.BusinessCategory,
                        CompanyCode = c.CompanyCode
                    })
                    .FirstOrDefaultAsync(cancellationToken);

                return company ?? throw new KeyNotFoundException("Company not found");
            }
        }
    }
}
