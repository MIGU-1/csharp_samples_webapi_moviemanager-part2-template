using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieManager.Core.Entities;
using Utils;

namespace MovieManager.Core
{
    public class ImportController
    {
        const string Filename = "movies.csv";

        /// <summary>
        /// Liefert die Movies mit den dazugehörigen Kategorien
        /// </summary>
        public static async Task<Movie[]> ReadFromCsvAsync()
        {
            string[][] csvMovies = await MyFile.ReadStringMatrixFromCsvAsync(Filename, true);

            var categories = csvMovies
                .GroupBy(line => line[2])
                .Select(grp => new Category() { CategoryName = grp.Key })
                .ToArray();
            Movie[] movies = csvMovies.Select(line =>
                new Movie()
                {
                    Category = categories.Single(cat => cat.CategoryName == line[2]),
                    Duration = int.Parse(line[3]),
                    Title = line[0],
                    Year = int.Parse(line[1]),
                }).ToArray();
            return movies;
        }

    }
}
