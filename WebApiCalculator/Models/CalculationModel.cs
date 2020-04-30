using System;
using System.ComponentModel.DataAnnotations;

namespace WebApiCalculator.Models
{
    public class CalculationModel
    {
        public int Id { get; set; }

        [Required]
        public string Type { get; set; }
        
        [Required]
        public string Expression { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        public double Result { get; set; }
    }
}
