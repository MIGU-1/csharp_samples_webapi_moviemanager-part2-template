using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.Core.Entities
{
    public class MovieWithCategoryDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public Category Category { get; set; }
        public int Duration { get; set; } //in Minuten
        public int Year { get; set; }
    }
}
