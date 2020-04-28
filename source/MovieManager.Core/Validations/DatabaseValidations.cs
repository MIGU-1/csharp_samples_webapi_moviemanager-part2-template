using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;

namespace MovieManager.Core.Validations
{
    public class DatabaseValidations
    {
        public static async Task<ValidationResult> ExistsMovieTwiceThisYearAsync(Movie movie, IUnitOfWork unitOfWork)
        {
            if (await unitOfWork.Movies.AnyOtherByTitleAndYearAsync(movie.Title, movie.Year, movie.Id))
            {
                return new ValidationResult($"Movie {movie.Title} existiert bereits im Jahr {movie.Year}!", 
                    new List<string> { nameof(movie.Title), nameof(movie.Year)});
            }

            return ValidationResult.Success;
        }

        public static async Task<ValidationResult> ExistsCategoryNameTwice(string categoryName, int? categoryId, IUnitOfWork unitOfWork)
        {
            if (await unitOfWork.Categories.ExistsCategoryName(categoryName, categoryId))
            {
                return new ValidationResult($"Category {categoryName} existiert bereits!",
                    new List<string> { "CategoryName" });
            }

            return ValidationResult.Success;
        }




    }
}
