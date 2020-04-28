using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MovieManager.Core.Validations;

namespace MovieManager.Core.Entities
{
    public partial class Movie : EntityObject
    {
        public string Title { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; }

        public int CategoryId { get; set; }

        public int Duration { get; set; } //in Minuten

        public int Year { get; set; }
    }
}
