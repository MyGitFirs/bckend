using CleanArchitecture.Application.User._Dto;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;
using CleanArchitecture.Application.Common.Interfaces;

namespace CleanArchitecture.Infrastructure.Report
{
    public class UsersBalancePdfGenerator : IUsersBalancePdfService
    {
        public byte[] Generate(List<UserWithBalanceDto> users, string companyName)
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

                        column.Item().AlignCenter().Text("User Balance Report")
                            .SemiBold().FontSize(18).FontColor(Colors.Black);

                        column.Item().PaddingBottom(15);
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(); 
                            columns.RelativeColumn(); 
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("Name");
                            header.Cell().Element(HeaderCellStyle).Text("Balance");
                        });

                        foreach (var user in users)
                        {
                            table.Cell().Element(CellStyle).Text($"{user.FirstName} {user.LastName}");
                            table.Cell().Element(CellStyle).Text($"{user.Balance:C}");
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
