using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieManager.Core.Validations
{
    public class MovieYearRangeAttribute : ValidationAttribute
    {
        public int MinRange { get; }
        public int MaxRange { get; }

        public MovieYearRangeAttribute(int minRange, int maxRange)
        {
            MinRange = minRange;
            MaxRange = maxRange;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var movie = (Movie)validationContext.ObjectInstance;

            if (movie.Year < MinRange || movie.Year > MaxRange)
            {
                return new ValidationResult($"The year is out of range: {MinRange} - {MaxRange}",
                                              new List<string> { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
