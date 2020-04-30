using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApiCalculator.Core.Domain.Entities;

namespace WebApiCalculator.Infrastructure.Configurations
{
    public class CalculationConfiguration : IEntityTypeConfiguration<Calculation>
    {
        /// <summary>
        /// Configures the calculation entity.
        /// </summary>
        /// <param name="builder"> The builder to be used to configure the calculating entity type. </param>
        public void Configure(EntityTypeBuilder<Calculation> builder)
        {
            builder.Property(calculation => calculation.Id)
                .IsRequired();
            builder.Property(calculation => calculation.Type)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(calculation => calculation.Expression)
                .IsRequired()
                .HasMaxLength(100);
            builder.Property(calculation => calculation.CreateDate)
                .IsRequired();
            builder.Property(calculation => calculation.Result);
        }
    }
}
