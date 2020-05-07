using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MovieManager.Core.Validations
{
    public class LengthRangeAttribute : ValidationAttribute
    {
        public int MinLength { get; set; }
        public int MaxLength { get; set; }

        public LengthRangeAttribute(int minLength, int maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var movie = (Movie)validationContext.ObjectInstance;

            if (movie.Duration < MinLength || movie.Year > MaxLength)
            {
                return new ValidationResult($"The length of {nameof(Movie.Title)} must be between * and *.",
                                              new List<string> { validationContext.MemberName });
            }

            return ValidationResult.Success;
        }
    }
}
