using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieManager.Core.Entities
{
    public partial class Category : EntityObject
    {
        public String CategoryName { get; set; }

        public ICollection<Movie> Movies { get; set; }
    }
}
