//using CleanArchitecture.Application.Common.Interfaces;
//using CleanArchitecture.Application.Transactions._Dto;
//using QuestPDF.Helpers;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using QuestPDF.Fluent;
//using QuestPDF.Infrastructure;


//namespace CleanArchitecture.Infrastructure.Report
//{
//    public class TransactionReportPdfGenerator : ITransactionReportPdfService
//    {
//        public byte[] Generate(List<TransactionReportDto> reports)
//        {
//            var document = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Size(PageSizes.A4);
//                    page.Margin(20);
//                    page.DefaultTextStyle(x => x.FontSize(12));

//                    page.Header()
//                        .Text("Transaction Summary Report")
//                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

//                    page.Content().Table(table =>
//                    {
//                        table.ColumnsDefinition(columns =>
//                        {
//                            columns.RelativeColumn();
//                            columns.RelativeColumn();
//                            columns.RelativeColumn();
//                            columns.RelativeColumn();
//                        });

//                        table.Header(header =>
//                        {
//                            header.Cell().Element(CellStyle).Text("Kiosk");
//                            header.Cell().Element(CellStyle).Text("Category");
//                            header.Cell().Element(CellStyle).Text("Date");
//                            header.Cell().Element(CellStyle).Text("Total Amount");
//                        });

//                        foreach (var item in reports)
//                        {
//                            table.Cell().Text(item.KioskName);
//                            table.Cell().Text(item.Category);
//                            table.Cell().Text(item.CreatedDate.ToShortDateString());
//                            table.Cell().Text(item.Amount.ToString("C"));
//                        }
//                    });
//                });
//            });

//            return document.GeneratePdf();
//        }

//        static IContainer CellStyle(IContainer container)
//        {
//            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten3);
//        }
//    }
//}
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using CleanArchitecture.Application.Transactions._Dto;
using System.Collections.Generic;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Report
{
    public class TransactionAllPdfGenerator : ITransactionAllPdfService
    {
        public byte[] Generate(List<TransactionDto> transactions, string companyName)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header().Column(column =>
                    {
                        column.Item().AlignCenter().Text(companyName) 
                            .Bold().FontSize(20).FontColor(Colors.Black);

                        column.Item().AlignCenter().Text("Transaction Detailed Report By Specific User")
                            .SemiBold().FontSize(18).FontColor(Colors.Black);

                        column.Item().PaddingBottom(15); 
                    });
                    // Content Table
                    page.Content().Table(table =>
                    {
   
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); 
                            columns.RelativeColumn();
                            columns.RelativeColumn(); 
                            columns.RelativeColumn();
                            columns.RelativeColumn(); 
                        });

                        // Table Header
                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text(t => t.Span("Name").Style(TextStyle.Default.Bold()));
                            header.Cell().Element(HeaderCellStyle).Text(t => t.Span("Kiosk").Style(TextStyle.Default.Bold()));
                            header.Cell().Element(HeaderCellStyle).Text(t => t.Span("Category").Style(TextStyle.Default.Bold()));
                            header.Cell().Element(HeaderCellStyle).Text(t => t.Span("Amount").Style(TextStyle.Default.Bold()));
                            header.Cell().Element(HeaderCellStyle).Text(t => t.Span("Date").Style(TextStyle.Default.Bold()));
                        });

                        // Table Rows
                        foreach (var item in transactions)
                        {
                            table.Cell().Element(CellStyle).Text($"{item.FirstName} {item.LastName}");
                            table.Cell().Element(CellStyle).Text(item.KioskName);
                            table.Cell().Element(CellStyle).Text(item.Category);
                            table.Cell().Element(CellStyle).Text(item.Amount.ToString("C"));
                            table.Cell().Element(CellStyle).Text(item.CreatedDate.ToShortDateString());
                        }
                    });
                });
            });

            return document.GeneratePdf();
        }

        static IContainer HeaderCellStyle(IContainer container)
        {
            return container
                 .Background(Colors.Grey.Lighten2) 
                 .Border(1)
                 .BorderColor(Colors.Black)
                 .PaddingVertical(5)
                 .PaddingHorizontal(5)
                 .AlignCenter()
                 .AlignMiddle();
        }
        static IContainer CellStyle(IContainer container)
        {
            return container
                .Border(1) 
                .BorderColor(Colors.Black) 
                .PaddingVertical(5)
                .PaddingHorizontal(5)
                .AlignCenter() 
                .AlignMiddle()
                .DefaultTextStyle(x => x.SemiBold()); 
        }
    }
}
