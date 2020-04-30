using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApiCalculator.Core.Domain.Entities;

namespace WebApiCalculator.Core.Domain.Interfaces
{
    public interface ICalculationRepository
    {
        Task<IEnumerable<Calculation>> GetAll();
        
        Task<IEnumerable<Calculation>> FindByHistory(Expression<Func<Calculation, bool>> predicate);
        
        Task<Calculation> FindByIdAsync(int id);

        Task UpdateAsync(Calculation calculation);

        Task<Calculation> CreateAsync(Calculation calculation);
        
        Task DeleteAsync(int? id);
    }
}