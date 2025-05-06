using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanArchitecture.Infrastructure.Persistence.Configurations
{
    public class CompanyJoinRequestConfiguration : IEntityTypeConfiguration<CompanyJoinRequest>
    {
        public void Configure(EntityTypeBuilder<CompanyJoinRequest> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Company>()
                .WithMany()
                .HasForeignKey(r => r.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => new { r.UserId, r.CompanyId })
                .IsUnique()
                .HasName("IX_Unique_User_Company_Request");
        }
    }
}
