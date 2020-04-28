using MovieManager.Core.Entities;

namespace MovieManager.Core.DataTransferObjects
{
    public class CategoryStatisticEntry
    {
        public string CategoryName { get; set; }
        public int NumberOfMovies { get; set; }
        public int TotalDuration { get; set; }
    }
}
