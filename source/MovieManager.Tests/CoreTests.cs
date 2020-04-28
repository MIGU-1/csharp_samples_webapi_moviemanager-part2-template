using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text.RegularExpressions;
using MovieManager.Core.Entities;

namespace MovieManager.Tests
{
    [TestClass]
    public class CoreTests
    {
        [TestMethod]
        public void CategoryValidation_MissingCategoryName_ShouldIndicateMissingCategoryName()
        {
            // Arrange
            Category category = new Category();

            // Act
            var (isValid, validationResults) = ValidateObject(category);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, validationResults.Count(_ => _.ErrorMessage == $"The {nameof(Category.CategoryName)} field is required."));
        }

        [TestMethod]
        public void MovieValidation_TooShortTitle_ShouldThrowValidationMessage()
        {
            // Arrange
            Movie movie = new Movie
            {
                Title = new String('x', 1)
            };

            // Act
            var (isValid, validationResults) = ValidateObject(movie);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1,
                    validationResults.Count(_ =>
                        Regex.IsMatch(
                            _.ErrorMessage,
                            WildCardToRegular($"The length of {nameof(Movie.Title)} must be between * and *."))));
        }

        [TestMethod]
        public void MovieValidation_TooLongTitle_ShouldThrowValidationMessage()
        {
            // Arrange
            Movie movie = new Movie()
            {
                Title = new String('x', 101)
            };

            // Act
            var (isValid, validationResults) = ValidateObject(movie);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1,
                validationResults.Count(_ =>
                    Regex.IsMatch(
                        _.ErrorMessage,
                        WildCardToRegular($"The length of {nameof(Movie.Title)} must be between * and *."))));
        }

        [TestMethod]
        public void MovieValidation_MissingTitle_ShouldIndicateMissingTitle()
        {
            // Arrange
            Movie movie = new Movie();

            // Act
            var (isValid, validationResults) = ValidateObject(movie);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, validationResults.Count(_ => _.ErrorMessage == $"The {nameof(Movie.Title)} field is required."));
        }

        [TestMethod]
        [DataRow(1899)]
        [DataRow(2100)]
        public void MovieValidation_YearNotInRange_ShouldThrowValidationMessage(int year)
        {
            // Arrange
            Movie movie = new Movie
            {
                Year = year
            };

            // Act

            var (isValid, validationResults) = ValidateObject(movie);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1,
                validationResults.Count(_ =>
                    Regex.IsMatch(
                        _.ErrorMessage,
                        WildCardToRegular($"The field {nameof(Movie.Year)} must be between * and *."))));
        }



        [TestMethod]
        public void MovieValidation_MissingCategoryId_ShouldIndicateMissingCategoryId()
        {
            // Arrange
            Movie movie = new Movie{Title = "Dummy", Year = 1990};

            // Act
            var (isValid, validationResults) = ValidateObject(movie);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, validationResults.Count(_ => _.ErrorMessage == $"CategoryId must be greater than zero"));
        }


        [TestMethod]
        public void MovieValidation_ClassicMovieRule_ShouldThrowAValidationEror()
        {
            // Arrange
            Movie movie = new Movie()
            {
                CategoryId = 1,
                Title = "A typical classic Movie",
                Duration = 240,
                Year = 1937
            };

            // Act
            var (isValid, validationResults) = ValidateObject(movie);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1,
                validationResults.Count(_ =>
                    Regex.IsMatch(
                        _.ErrorMessage,
                        WildCardToRegular("Classical Movies (until year '*') may not last longer than * minutes!"))));
        }


        #region Helper Methods

        private static (bool IsValid, IEnumerable<ValidationResult> ValidationResults) ValidateObject(object objectToValidate)
        {
            var context = new ValidationContext(objectToValidate, serviceProvider: null, items: null);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(objectToValidate, context, results, true);

            return (isValid, results);
        }

        /// <summary>
        /// Builds a Regular Expression out of a search string with * and ? wildcards.
        /// </summary>
        private static string WildCardToRegular(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            return "^" + Regex.Escape(value).Replace("\\?", ".").Replace("\\*", ".*") + "$";
        }


        #endregion

    }
}
