//using QuestPDF.Fluent;
//using QuestPDF.Helpers;
//using QuestPDF.Infrastructure;
//using CleanArchitecture.Application.Transactions._Dto;
//using System.Collections.Generic;
//using CleanArchitecture.Application.Common.Interfaces;

//namespace CleanArchitecture.Infrastructure.Report
//{
//    public class TransactionAllPdfGenerator : ITransactionAllPdfService
//    {

//        public byte[] Generate(List<TransactionDto> transactions)
//        {
//            // create ng pdf file
//            var document = Document.Create(container =>
//            {
//                container.Page(page =>
//                {
//                    page.Size(PageSizes.A4);
//                    page.Margin(20);
//                    page.DefaultTextStyle(x => x.FontSize(12));

//                    // header adjust nalang para maganda tingnan
//                    page.Header()
//                        .Text("Transaction Detailed Report")
//                        .SemiBold().FontSize(18).FontColor(Colors.Blue.Medium);

//                    // Content ng table
//                    page.Content().Table(table =>
//                    {
//                        // table columns
//                        table.ColumnsDefinition(columns =>
//                        {
//                            //columns.RelativeColumn(); // UserId
//                            columns.RelativeColumn(); // Name
//                            columns.RelativeColumn(); // Kiosk
//                            columns.RelativeColumn(); // Category
//                            columns.RelativeColumn(); // Amount
//                            columns.RelativeColumn(); // Date
//                        });

//                        // Table Header
//                        table.Header(header =>
//                        {
//                           // header.Cell().Element(CellStyle).Text("UserId");
//                            header.Cell().Element(CellStyle).Text("Name");
//                            header.Cell().Element(CellStyle).Text("Kiosk");
//                            header.Cell().Element(CellStyle).Text("Category");
//                            header.Cell().Element(CellStyle).Text("Amount");
//                            header.Cell().Element(CellStyle).Text("Date");
//                        });

//                        // Table Rows iteration mangyayare
//                        foreach (var item in transactions)
//                        {
//                           // table.Cell().Text(item.UserId.ToString());
//                            table.Cell().Text($"{item.FirstName} {item.LastName}");
//                            table.Cell().Text(item.KioskName);
//                            table.Cell().Text(item.Category);
//                            table.Cell().Text(item.Amount.ToString("C"));
//                            table.Cell().Text(item.CreatedDate.ToShortDateString());
//                        }
//                    });
//                });
//            });

//            // Return generated PDF
//            return document.GeneratePdf();
//        }

//        // Optional: Style table header cells
//        static IContainer CellStyle(IContainer container)
//        {
//            return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).Background(Colors.Grey.Lighten3);
//        }
//    }
//}

using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Transactions._Dto;
using QuestPDF.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CleanArchitecture.Infrastructure.Report
{
    public class TransactionReportPdfGenerator : ITransactionReportPdfService
    {
        public byte[] Generate(List<TransactionReportDto> reports, string companyName)
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
                           .FontSize(20).FontColor(Colors.Black);

                        column.Item().AlignCenter().Text("Kiosk Summary Report")
                            .SemiBold().FontSize(18).FontColor(Colors.Black);

                        column.Item().PaddingBottom(15);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); // Kiosk
                            columns.RelativeColumn(); // Category
                            columns.RelativeColumn(); // Date
                            columns.RelativeColumn(); // Total Amount
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("Kiosk");
                            header.Cell().Element(HeaderCellStyle).Text("Category");
                            header.Cell().Element(HeaderCellStyle).Text("Date");
                            header.Cell().Element(HeaderCellStyle).Text("Total Amount");
                        });

                        foreach (var item in reports)
                        {
                            table.Cell().Element(CellStyle).Text(item.KioskName);
                            table.Cell().Element(CellStyle).Text(item.Category);
                            table.Cell().Element(CellStyle).Text(item.CreatedDate.ToShortDateString());
                            table.Cell().Element(CellStyle).Text(item.Amount.ToString("C"));
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
                .AlignMiddle()
                .DefaultTextStyle(x => x.Bold());
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


