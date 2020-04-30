namespace WebApiCalculator.Infrastructure.Service
{
    public interface ICalculator
    {
        double Calculate(string expression);
        string OperationType(string expression);
    }
}