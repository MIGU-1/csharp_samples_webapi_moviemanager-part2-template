using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieManager.Core.Contracts;
using MovieManager.Core.DataTransferObjects;
using MovieManager.Core.Entities;
using MovieManager.WebApi.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MovieManager.WebApi.ApiControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private IUnitOfWork _unitOfWork;

        public MoviesController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Liefert alle Filme
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<List<MovieWithCategoryDto>> GetAllMovies()
        {
            return Ok(_unitOfWork
                .Movies
                .GetAllAsync()
                .Result
                .Select(m => new MovieWithCategoryDto()
                {
                    Id = m.Id,
                    Title = m.Title,
                    Category = m.Category,
                    Duration = m.Duration,
                    Year = m.Year
                })
                .ToList());
        }

        /// <summary>
        /// Erstellt einen neuen Film
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> CreateNewMovie(MovieWithCategoryNameDto movie)
        {
            Category category = await _unitOfWork.Categories.GetCategoryByNameAsync(movie.CategoryName);

            if (category != null)
            {
                Movie newMovie = new Movie()
                {
                    Title = movie.Title,
                    CategoryId = category.Id,
                    Category = category,
                    Duration = movie.Duration,
                    Year = movie.Year
                };

                try
                {
                    await _unitOfWork.Movies.AddAsync(newMovie);
                    await _unitOfWork.SaveChangesAsync();

                    return CreatedAtAction(nameof(GetMovieByIdAsync),
                                            new { id = newMovie.Id },
                                            newMovie);
                }
                catch (ValidationException ex)
                {
                    return BadRequest(ex.Message);
                }
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Liefert einen Film anhand der ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieWithCategoryDto>> GetMovieByIdAsync(int id)
        {
            Movie movie = await _unitOfWork
                                .Movies
                                .GetByIdAsync(id);

            if (movie != null)
            {
                MovieWithCategoryDto newMovie = new MovieWithCategoryDto()
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    Category = movie.Category,
                    Duration = movie.Duration,
                    Year = movie.Year
                };

                return Ok(newMovie);
            }
            else
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Ändert einen Film anhand der ID
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newMovie"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> ModifyMovie(int id, MovieWithCategoryNameDto newMovie)
        {
            Movie movie = await _unitOfWork.Movies.GetByIdAsync(id);
            Category newCategory = await _unitOfWork.Categories.GetCategoryByNameAsync(newMovie.CategoryName);

            if (movie != null && newCategory != null)
            {
                movie.Title = newMovie.Title;
                movie.Category = newCategory;
                movie.CategoryId = newCategory.Id;
                movie.Year = newMovie.Year;
                movie.Duration = newMovie.Duration;

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
                return NotFound("Die Kategorie oder der Film wurde nicht gefunde");
            }
        }

        /// <summary>
        /// Löscht einen vorhandenen Film
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteMovie(int id)
        {
            Movie tmp = await _unitOfWork.Movies.GetByIdAsync(id);

            if (tmp != null)
            {
                _unitOfWork.Movies.Delete(tmp);

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
                return NotFound($"Der Film mit der ID {id} wurde nicht gefunden");
            }
        }

        /// <summary>
        /// Liefert die Kategorie eines bestimmten Filmes
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}/category")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MovieWithCategoryDto>> GetMovieCategoryByIdAsync(int id)
        {
            Movie movie = await _unitOfWork
                                .Movies
                                .GetByIdAsync(id);


            if (movie != null)
            {
                Category category = await _unitOfWork.Categories.GetByIdAsync(movie.CategoryId);

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
                                     }).ToList()
                };

                return Ok(dto);
            }
            else
            {
                return NotFound();
            }
        }
    }
}
