using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Net.Cache;
using System.Text;

namespace MovieManager.Core.DataTransferObjects
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<MovieDto> Movies { get; set; }

        public CategoryDto()
        {
            Movies = new List<MovieDto>();
        }
    }
}
