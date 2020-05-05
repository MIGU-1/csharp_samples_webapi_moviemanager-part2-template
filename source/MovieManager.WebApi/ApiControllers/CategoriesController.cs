using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using MovieManager.Core.Contracts;
using MovieManager.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using MovieManager.Core.DataTransferObjects;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace MovieManager.WebApi.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle Kategorien
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CategoryDto>>> GetAllCategories()
        {
            var categoriesDb = await _unitOfWork
                        .Categories
                        .GetAllAsync();

            List<CategoryDto> categories = categoriesDb
                                            .Select(c => new CategoryDto
                                            {
                                                Id = c.Id,
                                                Name = c.CategoryName,
                                                Movies = c.Movies
                                                            .Select(m => new MovieDto()
                                                            {
                                                                Id = m.Id,
                                                                Title = m.Title,
                                                                CategoryId = m.CategoryId,
                                                                Year = m.Year,
                                                                Duration = m.Duration
                                                            })
                                                            .ToList()
                                            })
                                            .ToList();

            if (categories == null)
            {
                return NotFound(categories);
            }
            else
            {
                return Ok(categories);
            }
        }

        /// <summary>
        /// Erstellt eine neue Kategorie
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddNewCategoryAsync(string categoryName)
        {
            var result = _unitOfWork.Categories.ExistsCategoryName(categoryName, null);

            if (result.Result)
            {
                return BadRequest();
            }
            else
            {
                Category newCategory = new Category() { CategoryName = categoryName };
                await _unitOfWork.Categories.AddAsync(newCategory);

                try
                {
                    await _unitOfWork.SaveChangesAsync();
                }
                catch (ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }

                return CreatedAtAction(nameof(GetByIdAsync),
                                        new { id = newCategory.Id },
                                        new CategoryDto() { Name = newCategory.CategoryName });
            }
        }

        /// <summary>
        /// Ermittelt die Karegorie anhand der ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDto>> GetByIdAsync(int id)
        {
            Category category = await _unitOfWork
                                .Categories
                                .GetByIdAsync(id);

            if (category != null)
            {
                CategoryDto dto = new CategoryDto()
                {
                    Id = category.Id,
                    Name = category.CategoryName,
                    Movies = category.Movies
                                         .Select(m => new MovieDto()
                                         {
                                             Id = m.Id,
                                             Title = m.Title,
                                             CategoryId = m.CategoryId,
                                             Duration = m.Duration,
                                             Year = m.Year
                                         })
                                         .ToList()
                };

                return Ok(dto);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Ändert eine Kategorie anhand ihrer ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ModifyCategory(int id, string newName)
        {
            Category category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (!_unitOfWork.Categories.ExistsCategoryName(newName, null).Result && category != null)
            {
                category.CategoryName = newName;

                try
                {
                    return Ok(await _unitOfWork.SaveChangesAsync());
                }
                catch (ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound("Die Kategorie wurde nicht gefunden oder der Name existiert bereits");
            }
        }

        /// <summary>
        /// Löscht eine vorhandene Kategorie
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCategory(int id)
        {
            Category tmp = await _unitOfWork.Categories.GetByIdAsync(id);

            if (tmp != null)
            {
                _unitOfWork.Categories.Delete(tmp);
                try
                {
                    return Ok(await _unitOfWork.SaveChangesAsync());
                }
                catch (ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound($"Die Kategorie mit der ID {id} wurde nicht gefunden");
            }
        }

        /// <summary>
        /// Liefert alle Filme einer Kategorie
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/movies")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<CategoryDto>>> GetMovieFromCategory(int id)
        {
            Category category = await _unitOfWork.Categories.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(category);
            }
            else
            {
                return Ok(category.Movies.Select(m => new MovieDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    CategoryId = m.CategoryId,
                    Duration = m.Duration,
                    Year = m.Year
                }));
            }
        }
    }
}
