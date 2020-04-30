using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiCalculator.Core.Domain.Entities;
using WebApiCalculator.Core.Domain.Interfaces;
using WebApiCalculator.Infrastructure;
using WebApiCalculator.Infrastructure.Service;
using WebApiCalculator.Models;

namespace WebApiCalculator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculationsController : ControllerBase
    {
        private readonly ILogger<CalculationsController> _logger;
        private readonly ICalculationRepository _calculationRepository;
        private readonly ICalculator _calculator;
        private readonly CalculatorDbContext _dbContext;
        private readonly IMapper _mapper;

        public CalculationsController(
            ILogger<CalculationsController> logger, ICalculationRepository calculationRepository, ICalculator calculator, IMapper mapper, CalculatorDbContext dbContext)
        {
            _logger = logger;
            _calculationRepository = calculationRepository;
            _calculator = calculator;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Display calculation history 
        /// </summary>
        /// <returns>Calculation history</returns>
        [HttpGet("history")]
        public ActionResult<IEnumerable<CalculationModel>> History()
         {
             _logger.LogInformation("History calculations API call.");
            if (!_calculationRepository.GetAll().Result.Any())
            {
                _logger.LogWarning("History is empty.");
                return NotFound();
            }

            return _calculationRepository.GetAll().Result
                .Select(calculation => _mapper.Map<Calculation, CalculationModel>(calculation))
                .ToList();
        }

        /// <summary>
        /// Query to get calculation by id number
        /// </summary>
        /// <param name="id">calculation number</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CalculationModel>> GetById(int? id)
        {
            _logger.LogInformation($"Get {id} calculation API call.");

            if (!id.HasValue)
            {
                _logger.LogError("Missing id parameter");
                return BadRequest();
            }

            var calculation = await _calculationRepository.FindByIdAsync(id.Value);
            if (calculation is null)
            {
                _logger.LogWarning($"Calculation with {id} doesn't exist.");
                return NotFound();
            }

            return _mapper.Map<CalculationModel>(calculation);
        }
        
        /// <summary>
        /// Query to search calculations on history
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        [HttpGet("search/{predicate}")]
        public ActionResult<IEnumerable<CalculationModel>> FindByPredicate(string predicate)
        {
            _logger.LogInformation($"Attempt to find calculations by {predicate}.");
            var calculations = _calculationRepository
                .FindByHistory(c => c.Type.Contains(predicate) || c.Id.ToString().Contains(predicate)
                                    || c.Expression.Contains(predicate) || c.CreateDate.ToString().Contains(predicate)).Result
                .Select(calc => _mapper.Map<Calculation, CalculationModel>(calc))
                .ToList();

            if (calculations.Count == 0)
            {
                _logger.LogWarning($"Calculations with {predicate} don't exist.");
                return NotFound();
            }

            return calculations;
        }

        /// <summary>
        /// Query to create calculation
        /// </summary>
        /// <param name="calculationModel">Calculation dto model</param>
        /// <param name="expression"></param>
        /// <returns>Created at action response - success and Unprocessable entity - model errors</returns>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CalculationModel calculationModel, string expression)
        {
            _logger.LogInformation($"Create {calculationModel} calculation API call.");

            calculationModel.CreateDate = DateTime.Now;
            calculationModel.Type = _calculator.OperationType(expression);
            calculationModel.Expression = expression;
            calculationModel.Result = _calculator.Calculate(expression);

            // Validation check model
            if (!ModelState.IsValid)
            {
                _logger.LogError("Calculation model is not valid.");
                return UnprocessableEntity();
            }

            await _calculationRepository.CreateAsync(_mapper.Map<Calculation>(calculationModel));
            return CreatedAtAction(nameof(GetById), new {id = calculationModel.Id}, calculationModel);
        }

        /// <summary>
        /// Query to update calculation
        /// </summary>
        /// <param name="id">Calculation number</param>
        /// <param name="calculationModel">Calculation dto model</param>
        /// <returns>Ok response - success and Bad request - invalid request</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CalculationModel calculationModel)
        {
            _logger.LogInformation($"Attempt to UPDATE {id} calculation API call.");
            if (id != calculationModel.Id)
            {
                _logger.LogWarning($"Calculation with {id} doesn't exist.");
                return NotFound();
            }

            // Validation check model
            if (!ModelState.IsValid)
            {
                _logger.LogError("Calculation model is not valid.");
                return UnprocessableEntity();
            }

            await _calculationRepository.UpdateAsync(_mapper.Map<Calculation>(calculationModel));

            return Ok(calculationModel);
        }

        /// <summary>
        /// Query to update calculation
        /// </summary>
        /// <param name="id">Calculation number</param>
        /// <returns>No content response - success and Bad request - invalid request or Not found - current calculation doesn't exist</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            _logger.LogInformation($"Attempt to DELETE {id} calculation API call.");

            if (!id.HasValue)
            {
                _logger.LogError("Missing id parameter");
                return BadRequest();
            }
            
            var calculation = await _calculationRepository.FindByIdAsync(id.Value);
            if (calculation is null)
            {
                _logger.LogWarning($"Calculation with {id} doesn't exist");
                return NotFound();
            }
            await _calculationRepository.DeleteAsync(_mapper.Map<Calculation>(calculation).Id);

            return NoContent();
        }
    }
}
