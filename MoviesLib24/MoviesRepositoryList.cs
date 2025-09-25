using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoviesLib24
{
    public class MoviesRepositoryList : IMoviesRepository
    {
        private int _nextId = 1;
        private readonly List<Movie> _movies = new();

        public MoviesRepositoryList()
        {
            //_movies.Add(new Movie() { Id = _nextId++, Title = "The Matrix", Year = 1999 });
            //_movies.Add(new Movie() { Id = _nextId++, Title = "Snehvide", Year = 1937 });
        }

        public Movie? GetById(int id)
        {
            return _movies.Find(movie => movie.Id == id);
        }

        public Movie Add(Movie movie)
        {
            movie.Id = _nextId++;
            _movies.Add(movie);
            return movie;
        }

        public Movie? Remove(int id)
        {
            Movie? movie = GetById(id);
            if (movie == null)
            {
                return null;
            }
            _movies.Remove(movie);
            return movie;
        }

        public Movie? Update(int id, Movie movie)
        {
            Movie? existingMovie = GetById(id);
            if (existingMovie == null)
            {
                return null;
            }
            existingMovie.Title = movie.Title;
            existingMovie.Year = movie.Year;
            return existingMovie;
        }

        public IEnumerable<Movie> Get(int? yearAfter = null, string? titleIncludes = null, string? orderBy = null)
        {
            IEnumerable<Movie> result = new List<Movie>(_movies);
            // Filtering
            if (yearAfter != null)
            {
                result = result.Where(m => m.Year > yearAfter);
            }
            if (titleIncludes != null)
            {
                result = result.Where(m => m.Title.Contains(titleIncludes));
            }

            // Ordering aka. sorting
            if (orderBy != null)
            {
                orderBy = orderBy.ToLower();
                switch (orderBy)
                {
                    case "title": // fall through to next case
                    case "title_asc":
                        result = result.OrderBy(m => m.Title);
                        break;
                    case "title_desc":
                        result = result.OrderByDescending(m => m.Title);
                        break;
                    case "year":
                    case "year_asc":
                        result = result.OrderBy(m => m.Year);
                        break;
                    case "year_desc":
                        result = result.OrderByDescending(m => m.Year);
                        break;
                    default:
                        break; // do nothing
                        //throw new ArgumentException("Unknown sort order: " + orderBy);
                }
            }
            return result;
        }
    }
}

