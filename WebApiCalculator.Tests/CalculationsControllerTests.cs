using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable.Moq;
using Moq;
using WebApiCalculator.Controllers;
using WebApiCalculator.Core.Domain.Entities;
using WebApiCalculator.Core.Domain.Interfaces;
using WebApiCalculator.Infrastructure;
using WebApiCalculator.Infrastructure.Service;
using WebApiCalculator.Models;
using Xunit;

namespace WebApiCalculator.Tests
{
    public class CalculationsControllerTests
    {
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ICalculationRepository> _mockRepository;
        private readonly Mock<ICalculator> _mockCalculator;
        private readonly Mock<ILogger<CalculationsController>> _mockLogger;
        private readonly Mock<CalculatorDbContext> _mockDbContext;
        private List<CalculationModel> _listOfCalculations;
        private CalculationsController _calculationsController;
        private readonly DbContextOptions _options;

        public CalculationsControllerTests()
        { 
            _options = new DbContextOptions<CalculatorDbContext>();
            _mockMapper = new Mock<IMapper>();
            _mockCalculator = new Mock<ICalculator>();
            _mockDbContext = new Mock<CalculatorDbContext>(_options);
            _mockRepository = new Mock<ICalculationRepository>();
            _mockLogger = new Mock<ILogger<CalculationsController>>();
            _listOfCalculations = new List<CalculationModel>();
            _calculationsController = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);
        }

        private IEnumerable<CalculationModel> GetTestCalculations()
        {
            _listOfCalculations.Add(new CalculationModel{ Id = 1, Type = "Add", Expression = "3 + 5", CreateDate = DateTime.Now, Result = 8});
            _listOfCalculations.Add(new CalculationModel{ Id = 2, Type = "Subtract", Expression = "12 - 9", CreateDate = DateTime.Now, Result = 3 });
            _listOfCalculations.Add(new CalculationModel{ Id = 3, Type = "Multiply", Expression = "7 * 4", CreateDate = DateTime.Now, Result = 28 });
            _listOfCalculations.Add(new CalculationModel{ Id = 4, Type = "Divide", Expression = "8 / 2", CreateDate = DateTime.Now, Result = 4 });
            
            return _listOfCalculations;
        }

        [Fact]
        public void HistoryReturnsResultWithListOfCalculations()
        {
            // Arrange
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            // Act
            var result = controller.History().Result;

            // Assert
            var actionResult = Assert.IsType<ActionResult<IEnumerable<CalculationModel>>>(result);
            Assert.Equal(GetTestCalculations().Count(), actionResult.Value.Count());
        }

        [Fact]
        public void HistoryReturnsBadRequestWhenListIsNull()
        {
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result = controller.History().Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetByIdReturnsResultWithCalculation()
        {
            var testUserId = 1;
            var calculation = GetTestCalculations().FirstOrDefault(c => c.Id == testUserId);
            _mockRepository.Setup(rep => rep.FindByIdAsync(testUserId).Result)
                .Returns(_mockMapper.Object.Map<Calculation>(calculation));
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result =  controller.GetById(testUserId);
            
            var actionResult = Assert.IsType<Task<ActionResult<CalculationModel>>>(result);
            var model = Assert.IsType<Task<Calculation>>(actionResult.Result.Value);
            Assert.Equal(testUserId, model.Result.Id);
            Assert.Equal("Add", model.Result.Type);
            Assert.Equal("3 + 5", model.Result.Expression);
            Assert.Equal(8, model.Result.Result);
        }

        [Fact]
        public void GetByIdReturnsBadRequestResultWhenIdIsNull()
        {
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result = controller.GetById(null).Result;

            Assert.IsType<ActionResult<CalculationModel>>(result);
        }

        [Fact]
        public void GetByIdReturnsNotFoundResultWhenCalculationNotFound()
        {
            int testCalcId = 6555;
            _mockRepository.Setup(rep => rep.FindByIdAsync(testCalcId))
                .Returns(null as Task<Calculation>);
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result = controller.GetById(testCalcId);

            Assert.IsType<Task<ActionResult<CalculationModel>>>(result);
        }

        [Fact]
        public void CreateReturnsCreatedAtActionAndAddsCalculation()
        {
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);
            var newCalculation = new CalculationModel()
            {
                Type = "Add", 
                Expression = "88 + 11", 
                CreateDate = DateTime.Now, 
                Result = 99
            };

            var result = controller.Create(newCalculation, newCalculation.Expression).Result;

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Null(createdAtActionResult.ControllerName);
            Assert.Equal("GetById", createdAtActionResult.ActionName);
            _mockRepository.Verify(r => r
                .CreateAsync(_mockMapper.Object.Map<Calculation>(newCalculation)));
        }

        [Fact]
        public void CreateReturnsResultWithInvalidCalculationModel()
        {
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);
            controller.ModelState.AddModelError("Expression", "Required");
            var newCalculation = new CalculationModel();

            var result = controller.Create(newCalculation, newCalculation.Expression).Result;

            Assert.IsType<UnprocessableEntityResult>(result);
        }

        [Fact]
        public void UpdateReturnsResultWithCalculation()
        {
            var testUserId = 1;
            var calculation = GetTestCalculations().FirstOrDefault(c => c.Id == testUserId);

            _mockRepository.Setup(rep => rep.FindByIdAsync(testUserId).Result)
                .Returns(_mockMapper.Object.Map<Calculation>(calculation));
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);
            var modifiedCalculation = new CalculationModel();

            var result = controller.Update(testUserId, modifiedCalculation).Result;

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void UpdateCalculationReturnsNotFound()
        {
            int testUserId = 65555;
            _mockRepository.Setup(rep => rep.FindByIdAsync(testUserId))
                .Returns(null as Task<Calculation>);
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);
            var modifiedCalculation = new CalculationModel();

            var result = controller.Update(testUserId, modifiedCalculation).Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void UpdateReturnsResultWithInvalidCalculationModel()
        {
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);
            controller.ModelState.AddModelError("Type", "Required");
            var modifiedCalculation = new CalculationModel();
            
            var result = controller.Update(modifiedCalculation.Id, modifiedCalculation).Result;

            Assert.IsType<UnprocessableEntityResult>(result);
        }

        [Fact]
        public void DeleteReturnsBadRequestResultWhenIdIsNull()
        {
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result = controller.Delete(null).Result;

            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public void DeleteReturnsNotFoundResultWhenCalculationNotFound()
        {
            int testCalcId = 8;
            _mockRepository.Setup(rep => rep.DeleteAsync(testCalcId))
                .Returns(null as Task<Calculation>);
            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result = controller.Delete(testCalcId).Result;

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteReturnsResultWithCalculation()
        {
            var testUserId = 1;
            var calculation = GetTestCalculations().FirstOrDefault(c => c.Id == testUserId);

            _mockRepository.Setup(rep => rep.FindByIdAsync(testUserId).Result)
                .Returns(_mockMapper.Object.Map<Calculation>(calculation));

            var controller = new CalculationsController(_mockLogger.Object, _mockRepository.Object, _mockCalculator.Object, _mockMapper.Object, _mockDbContext.Object);

            var result = controller.Delete(testUserId).Result;

            Assert.IsType<NoContentResult>(result);
        }
    }
}
