using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace MovieManager.Core.DataTransferObjects
{
    public class MovieDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int CategoryId { get; set; }
        public int Duration { get; set; } //in Minuten
        public int Year { get; set; }
    }
}
