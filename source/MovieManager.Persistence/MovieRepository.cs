using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieManager.Persistence
{
    public class MovieRepository : IMovieRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public MovieRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Delete(Movie movie)
        {
            _dbContext.Movies.Remove(movie);
        }

        /// <summary>
        /// Liefert alle Filme zu einer übergebenen Kategorie sortiert nach Titel
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Movie[]> GetAllByCatIdAsync(int id)
        {
            return await _dbContext
                .Movies
                .Where(m => m.CategoryId == id)
                .OrderBy(m => m.Title)
                .ToArrayAsync();
        }

        /// <summary>
        /// Liefert den Film mit der übergebenen Id (null wenn nicht gefunden)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Movie> GetByIdAsync(int id)
        {
            return await _dbContext
                .Movies
                .SingleOrDefaultAsync(m => m.Id == id);
        }

        /// <summary>
        /// Liefert die Anzahl aller Filme in der Datenbank
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetCountAsync()
        {
            return await _dbContext.Movies.CountAsync();
        }

        /// <summary>
        /// Liefert den Film mit der längsten Dauer
        /// </summary>
        /// <returns></returns>
        public async Task<Movie> GetLongestMovieAsync()
        {
            return await _dbContext
                .Movies
                .OrderByDescending(m => m.Duration)
                .ThenBy(m=>m.Title)  //!
                .FirstAsync();
        }

        public async Task AddRangeAsync(Movie[] movies)
        {
            await _dbContext.Movies.AddRangeAsync(movies);
        }

        public async Task<Movie[]> GetAllAsync()
        {
            return await _dbContext
                .Movies
                .OrderBy(m => m.Title)
                .ToArrayAsync();
        }

        public async Task<bool> AnyOtherByTitleAndYearAsync(string movieTitle,  int movieYear, int movieId)
        {
            return await _dbContext
                .Movies
                .AnyAsync(m => m.Title == movieTitle && m.Year == movieYear && m.Id != movieId);
        }

        public void Update(Movie movie)
        {
            _dbContext.Entry(movie).State = EntityState.Modified;
        }

        public async Task AddAsync(Movie movie)
        {
            await  _dbContext
                .Movies.
                AddAsync(movie);
        }

    }
}