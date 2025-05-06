using CleanArchitecture.Application.Transactions._Dto;
using System;
using System.Collections.Generic;
using System.Text;


namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ITransactionReportPdfService
    {
        byte[] Generate(List<TransactionReportDto> reports, string companyName);
    }
}
