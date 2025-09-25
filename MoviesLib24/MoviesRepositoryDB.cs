namespace MoviesLib24
{
    public class MoviesRepositoryDB : IMoviesRepository
    {

        private readonly MoviesDbContext _context;

        public MoviesRepositoryDB(MoviesDbContext dbContext)
        {
            _context = dbContext;
        }

        public Movie Add(Movie movie)
        {
            movie.Id = 0;
            _context.Movies.Add(movie);
            _context.SaveChanges();
            return movie;
        }

        public IEnumerable<Movie> Get(int? yearAfter = null, string? titleIncludes = null, string? orderBy = null)
        {
            //List<Movie> result = _context.Movies.ToList();
            IQueryable<Movie> query = _context.Movies.ToList().AsQueryable();
            // Copy ToList()
            if (yearAfter != null)
            {
                query = query.Where(m => m.Year > yearAfter);
            }
            if (titleIncludes != null)
            {
                query = query.Where(m => m.Title.Contains(titleIncludes));
            }
            if (orderBy != null)
            {
                orderBy = orderBy.ToLower();
                switch (orderBy)
                {
                    case "title":
                    case "title_asc":
                        query = query.OrderBy(m => m.Title);
                        break;
                    case "title_desc":
                        query = query.OrderByDescending(m => m.Title);
                        break;
                    case "year":
                        query = query.OrderBy(m => m.Year);
                        break;
                    case "year_desc":
                        query = query.OrderByDescending(m => m.Year);
                        break;
                    default:
                        break; // do nothing
                        //throw new ArgumentException("Unknown sort order: " + orderBy);
                }
            }
            return query;
        }

        public Movie? GetById(int id)
        {
            return _context.Movies.FirstOrDefault(m => m.Id == id);
        }

        public Movie? Remove(int id)
        {
            Movie? movie = GetById(id);
            if (movie is null)
            {
                return null;
            }
            _context.Movies.Remove(movie);
            _context.SaveChanges();
            return movie;
        }

        public Movie? Update(int id, Movie movie)
        // https://www.learnentityframeworkcore.com/dbcontext/modifying-data
        {
            Movie? movieToUpdate = GetById(id);
            if (movieToUpdate == null) return null;
            movieToUpdate.Title = movie.Title;
            movieToUpdate.Year = movie.Year;
            _context.SaveChanges();
            return movieToUpdate;
        }
    }
}