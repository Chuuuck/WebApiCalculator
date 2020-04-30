using Microsoft.EntityFrameworkCore;
using WebApiCalculator.Core.Domain.Entities;
using WebApiCalculator.Infrastructure.Configurations;

namespace WebApiCalculator.Infrastructure
{
    public class CalculatorDbContext : DbContext
    {
        public CalculatorDbContext(DbContextOptions<CalculatorDbContext> options) 
            : base(options)
        {
        }

        public virtual DbSet<Calculation> Calculations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new CalculationConfiguration());
        }
    }
}