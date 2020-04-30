using System;

namespace WebApiCalculator.Core.Domain.Entities
{
    public class Calculation
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public string Expression { get; set; }

        public DateTime CreateDate { get; set; }

        public double Result { get; set; }
    }
}