
namespace MoviesLib24
{
    public interface IMoviesRepository
    {
        Movie Add(Movie movie);
        IEnumerable<Movie> Get(int? yearAfter = null, string? titleIncludes = null, string? orderBy = null);
        Movie? GetById(int id);
        Movie? Remove(int id);
        Movie? Update(int id, Movie movie);
    }
}