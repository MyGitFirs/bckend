using CleanArchitecture.Application.Transactions._Dto;
using CleanArchitecture.Application.Transactions.Queries.GetTransactionAll;
using CleanArchitecture.Application.Transactions.Queries.GetTransactionDetailedReport;
using CleanArchitecture.Application.User.Queries.GetUserBalance;
using CleanArchitecture.Application.User._Dto;
using CleanArchitecture.Application.Wallets._Dto;
using CleanArchitecture.Application.Wallets.Queries.GetAllWallet;
using CleanArchitecture.Infrastructure.Report;
using CleanArchitecture.Application.Common.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.WebUI.Controllers
{
    public class RecordController : ApiController
    {
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;
        private readonly ITransactionAllPdfService _pdfService;
        private readonly IUsersBalancePdfService _usersBalancePdfService;
        private readonly ITransactionReportPdfService _transactionReportPdfService;

        public RecordController(IApplicationDbContext context, IMediator mediator, ITransactionAllPdfService pdfService, IUsersBalancePdfService usersBalancePdfService, ITransactionReportPdfService transactionReportPdfService)
        {
            _context = context;
            _mediator = mediator;
            _pdfService = pdfService;
            _usersBalancePdfService = usersBalancePdfService;
            _transactionReportPdfService = transactionReportPdfService;
        }

        //[HttpGet("user/balance")]
        //public async Task<ActionResult<List<UserWithBalanceDto>>> GetUsersWithBalance([FromQuery] Guid companyId)
        //{
        //    if (companyId == Guid.Empty)
        //        return BadRequest("CompanyId is required.");

        //    var result = await _mediator.Send(new GetUserBalanceQuery { CompanyId = companyId });

        //    return Ok(result);
        //}

        [HttpGet("user/balance/pdf")]
        public async Task<IActionResult> DownloadUsersBalancePdf([FromQuery] Guid companyId)
        {
            try
            {
                if (companyId == Guid.Empty)
                    return BadRequest("CompanyId is required.");


                var company = await _context.Companies
                    .Where(c => c.Id == companyId)
                    .Select(c => c.CompanyName)  
                    .FirstOrDefaultAsync();

                if (company == null)
                    return NotFound("Company not found.");

                var data = await _mediator.Send(new GetUserBalanceQuery { CompanyId = companyId });

                if (data == null || !data.Any())
                    return NotFound("No user balance data found for this company.");

                var pdf = _usersBalancePdfService.Generate(data, company);

                return File(pdf, "application/pdf", "UsersBalanceReport.pdf");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        //[HttpGet("report")]
        //public async Task<ActionResult<List<TransactionReportDto>>> GetTransactionReport([FromQuery] Guid companyId)
        //{
        //    if (companyId == Guid.Empty)
        //        return BadRequest("CompanyId is required.");

        //    var result = await _mediator.Send(new GetTransactionAllQuery { CompanyId = companyId });

        //    return Ok(result);
        //}

        //[HttpGet("report/kiosk/pdf")]
        //public async Task<IActionResult> DownloadTransactionReportPdf([FromQuery] Guid companyId)
        //{
        //    if (companyId == Guid.Empty)
        //        return BadRequest("CompanyId is required.");

        //    var result = await _mediator.Send(new GetTransactionAllQuery { CompanyId = companyId });

        //    if (!result.Any())
        //        return NotFound("No data found.");

        //    var pdf = _transactionReportPdfService.Generate(result);

        //    return File(pdf, "application/pdf", "TransactionSummaryReport.pdf");
        //}

        [HttpGet("report/kiosk/pdf")]
        public async Task<IActionResult> DownloadTransactionReportPdf([FromQuery] Guid companyId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            if (companyId == Guid.Empty)
                return BadRequest("CompanyId is required.");

            var company = await _context.Companies.FindAsync(companyId);
            if (company == null)
                return NotFound("Company not found.");

            string companyName = company.CompanyName; 

            var result = await _mediator.Send(new GetTransactionAllQuery
            {
                CompanyId = companyId,
                FromDate = fromDate,
                ToDate = toDate
            });

            if (!result.Any())
                return NotFound("No data found.");

            var pdf = _transactionReportPdfService.Generate(result, companyName);

            return File(pdf, "application/pdf", "TransactionSummaryReport.pdf");
        }


        //[HttpGet("report/detailed")]
        //public async Task<ActionResult<List<TransactionDto>>> GetTransactionDetails([FromQuery] Guid companyId)
        //{
        //    if (companyId == Guid.Empty)
        //        return BadRequest("CompanyId is required.");

        //    var result = await _mediator.Send(new GetTransactionDetailedReportQuery { CompanyId = companyId });

        //    return Ok(result);
        //}

        [HttpGet("report/user/pdf")]
        public async Task<IActionResult> DownloadTransactionPdf([FromQuery] Guid companyId, [FromQuery] string fullName, [FromQuery] DateTime? date)
        {
            if (companyId == Guid.Empty)
                return BadRequest("CompanyId is required.");

            var data = await _mediator.Send(new GetTransactionDetailedReportQuery
            {
                CompanyId = companyId,
                FullName = fullName,
                Date = date
            });

            if (data == null || !data.Any())
                return NotFound("No transaction data found for this company.");

            var companyName = await _context.Companies
           .Where(c => c.Id == companyId)
           .Select(c => c.CompanyName)
           .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(companyName))
                companyName = "Unknown Company";

            var pdf = _pdfService.Generate(data, companyName);

            return File(pdf, "application/pdf", "TransactionReport.pdf");
        }

    }
}
