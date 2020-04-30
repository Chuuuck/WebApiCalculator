using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebApiCalculator.Core.Domain.Entities;
using WebApiCalculator.Core.Domain.Interfaces;

namespace WebApiCalculator.Infrastructure.Repositories
{
    public class CalculationRepository : ICalculationRepository
    {
        private readonly CalculatorDbContext _dbContext;
        private readonly DbSet<Calculation> _calculationDbSet;
        private readonly ILogger<CalculationRepository> _logger;

        public CalculationRepository(CalculatorDbContext dbContext, ILogger<CalculationRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _calculationDbSet = _dbContext?.Set<Calculation>();
        }

        /// <summary>
        /// Get all ordered calculation by created date.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Calculation>> GetAll()
        {
            _logger.LogInformation("Inside the repository about to call GetAll.");

            return await _calculationDbSet
                .OrderByDescending(c => c.CreateDate)
                .ToListAsync();
        }

        /// <summary>
        /// Find calculations by predicate.
        /// </summary>
        /// <param name="predicate"> Condition for search. </param>
        /// <returns></returns>
        public async Task<IEnumerable<Calculation>> FindByHistory(Expression<Func<Calculation, bool>> predicate)
        {
            _logger.LogInformation("Inside the repository about to call FindByHistory.");

            return await _calculationDbSet.Where(predicate)
                .ToListAsync();
        }

        /// <summary>
        /// Find calculation number by id.
        /// </summary>
        /// <param name="id">Calculation number</param>
        /// <returns></returns>
        public async Task<Calculation> FindByIdAsync(int id)
        {
            _logger.LogInformation("Inside the repository about to call FindByIdAsync.");
            return await _calculationDbSet.FindAsync(id);
        }

        /// <summary>
        /// Update calculation.
        /// </summary>
        /// <param name="calculation"> Calculation entity. </param>
        /// <returns></returns>
        public async Task UpdateAsync(Calculation calculation)
        {
            _logger.LogInformation("Inside the repository about to call UpdateAsync.");

            _dbContext.Entry(calculation).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>
        /// Create calculation.
        /// </summary>
        /// <param name="calculation"> Calculation entity. </param>
        /// <returns></returns>
        public async Task<Calculation> CreateAsync(Calculation calculation)
        {
            _logger.LogInformation("Inside the repository about to call CreateAsync.");

            await _dbContext.Calculations.AddAsync(calculation);
            await _dbContext.SaveChangesAsync();
            return calculation;
        }

        /// <summary>
        /// Delete calculation by id.
        /// </summary>
        /// <param name="id"> Calculation number. </param>
        /// <returns></returns>
        public async Task DeleteAsync(int? id)
        {
            _logger.LogInformation("Inside the repository about to call DeleteAsync.");
            var calculation = await _dbContext.Calculations.FindAsync(id);

            if (calculation is null) return;
            _dbContext.Calculations.Remove(calculation);
            await _dbContext.SaveChangesAsync();
        }
    }
}