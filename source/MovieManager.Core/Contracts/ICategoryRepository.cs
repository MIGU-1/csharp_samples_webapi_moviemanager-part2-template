using System.Collections.Generic;
using System.Threading.Tasks;
using MovieManager.Core.DataTransferObjects;
using MovieManager.Core.Entities;


namespace MovieManager.Core.Contracts
{
    public interface ICategoryRepository
    {
        Task<Category[]> GetAllAsync();
        Task AddAsync(Category category);
        Task<Category> GetByIdAsync(int id);
        Task<CategoryStatisticEntry[]> GetCategoryStatisticsAsync();
        Task<CategoryStatisticEntry> GetCategoryWithMostMoviesAsync();
        Task<(string CategoryName, double AverageLength)[]> GetCategoriesWithAverageLengthOfMoviesAsync();
        Task<int> GetYearWithMostPublicationsForCategoryAsync(string categoryName);
        Task AddRangeAsync(IEnumerable<Category> categories);
        void Delete(Category category);
        Task<bool> ExistsCategoryName(string categoryName, int? categoryId);
    }
}
