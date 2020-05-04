using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieManager.WebApi.DataTransferObjects
{
    public class MovieWithCategoryNameDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public int Duration { get; set; } //in Minuten
        public int Year { get; set; }
    }
}
