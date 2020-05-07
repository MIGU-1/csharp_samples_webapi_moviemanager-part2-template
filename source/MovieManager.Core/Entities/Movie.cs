using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieManager.Core.Validations;

namespace MovieManager.Core.Entities
{
    public partial class Movie : EntityObject
    {
        [Required]
        [LengthRange(minLength:3, maxLength:100)]
        public string Title { get; set; }

        [Required(ErrorMessage = "CategoryId must be greater than zero")]
        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }
        
        [Required]
        public int CategoryId { get; set; }
        
        [ClassicMovieMaxDuration(isClassicMovieUntilYear: 1950, maxDurationForClassicMovie: 200)]
        public int Duration { get; set; } //in Minuten

        [MovieYearRange(minRange: 1900, maxRange: 2099)]
        public int Year { get; set; }

        public Movie()
        {
            Duration = 0;
            Year = 1900;
        }
    }
}
