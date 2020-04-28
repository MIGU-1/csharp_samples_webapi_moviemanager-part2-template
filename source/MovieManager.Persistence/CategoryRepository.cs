using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MovieManager.Core.DataTransferObjects;

namespace MovieManager.Persistence
{
    internal class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoryRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// Liefert eine Liste aller Kategorien sortiert nach dem CategoryName
        /// </summary>
        /// <returns></returns>
        public async Task<Category[]> GetAllAsync()
        {
            return await _dbContext
                .Categories
                .Include(c=>c.Movies)
                .OrderBy(c => c.CategoryName)
                .ToArrayAsync();
        }

        /// <summary>
        /// Liefert die Kategorie mit der übergebenen Id --> null wenn nicht gefunden
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _dbContext
                .Categories
                .SingleOrDefaultAsync(c => c.Id == id);
        }


        /// <summary>
        /// Liefert eine Liste mit der Anzahl und Gesamtdauer aller Filme je Kategorie
        /// Sortiert nach dem Namen der Kategorie (aufsteigend).
        /// </summary>
        public async Task<CategoryStatisticEntry[]> GetCategoryStatisticsAsync()
        {
            return await _dbContext
                .Movies.GroupBy(m => m.Category.CategoryName)
                .Select(grp =>
                new CategoryStatisticEntry()
                {
                    CategoryName = grp.Key,
                    NumberOfMovies = grp.Count(),
                    TotalDuration = grp.Sum(m => m.Duration)
                })
                .OrderBy(res => res.CategoryName)
                .ToArrayAsync();
        }

        /// <summary>
        /// Liefert die Kategorie mit den meisten Filmen
        /// </summary>
        public async Task<CategoryStatisticEntry> GetCategoryWithMostMoviesAsync()
        {
            var result = await _dbContext
                .Movies
                .GroupBy(m => m.Category.CategoryName)
                .Select(grp =>
                    new CategoryStatisticEntry()
                    {
                        CategoryName = grp.Key,
                        NumberOfMovies = grp.Count(),
                        TotalDuration = grp.Sum(m => m.Duration)
                    })
                .OrderByDescending(entry => entry.NumberOfMovies)
                .ThenBy(entry => entry.CategoryName)  //!
                .FirstOrDefaultAsync();
            return result;
        }

        /// <summary>
        /// Liefert die Kategorien mit der durchschnittlichen Länge der zugeordneten Filme.
        /// Absteigend sortiert nach der durchschnittlichen Dauer der Filme - bei gleicher
        /// Dauer dann nach dem Namen der Kategorie aufsteigend. 
        /// </summary>
        public async Task<(string CategoryName, double AverageLength)[]> GetCategoriesWithAverageLengthOfMoviesAsync()
        {
            var results = await _dbContext
                .Categories
                .Select(c => new
                {
                    CategoryName = c.CategoryName,
                    AverageDuration = c.Movies.Average(movie => movie.Duration)
                })
                .OrderByDescending(res => res.AverageDuration)
                .ThenBy(res => res.CategoryName)
                .ToArrayAsync();
            return  results.
                Select(cat => ValueTuple.Create(cat.CategoryName, cat.AverageDuration))
                .ToArray();
        }

        public async Task<int> GetYearWithMostPublicationsForCategoryAsync(string categoryName)
        {
            return (await _dbContext.Movies
                .Where(movie => movie.Category.CategoryName == categoryName)
                .GroupBy(movie => movie.Year)
                .Select(movieGroupByYear =>
                    new
                    {
                        Year = movieGroupByYear.Key,
                        CntOfMovies = movieGroupByYear.Count()
                    })
                .OrderByDescending(movieGroupByYear => movieGroupByYear.CntOfMovies)
                .FirstAsync()).Year;
        }

        public async Task AddRangeAsync(IEnumerable<Category> categories)
        {
            await _dbContext.Categories.AddRangeAsync(categories);
        }

        public void Delete(Category category)
        {
            _dbContext.Remove(category);
        }

        public async Task<bool> ExistsCategoryName(string categoryName, int? categoryId)
        {
            return await _dbContext
                .Categories
                .AnyAsync(c => c.CategoryName == categoryName && (categoryId != null || c.Id != categoryId));
        }

        /// <summary>
        /// Neue Kategorie wird in Datenbank eingefügt
        /// </summary>
        /// <param name="category"></param>
        public async Task AddAsync(Category category)
        {
            await _dbContext.Categories.AddAsync(category);
        }

    }
}