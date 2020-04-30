using AutoMapper;
using WebApiCalculator.Core.Domain.Entities;
using WebApiCalculator.Models;

namespace WebApiCalculator.MapperProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CalculationModel, Calculation>();
            CreateMap<Calculation, CalculationModel>();
        }
    }
}