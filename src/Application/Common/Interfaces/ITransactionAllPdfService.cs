using CleanArchitecture.Application.Transactions._Dto;
using System.Collections.Generic;


namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ITransactionAllPdfService
    {
        byte[] Generate(List<TransactionDto> transactions, string companyName);
    }
}
