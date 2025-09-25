using Microsoft.EntityFrameworkCore;
using MoviesLib24;

namespace MoviesLib24Tests
{
    
   [TestClass()]
    public class MoviesRepositoryTest2
    {

        private const bool useDatabase = true;
        private static IMoviesRepository _repo;
        // https://learn.microsoft.com/en-us/dotnet/core/testing/order-unit-tests?pivots=mstest

        [ClassInitialize]
        public static void InitOnce(TestContext context)
        {
            if (useDatabase)
            {
                var optionsBuilder = new DbContextOptionsBuilder<MoviesDbContext>();
                // https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets
                //optionsBuilder.UseSqlServer(DBSecrets.ConnectionStringSimply);

                // connection string structure
                //   "Data Source=mssql7.unoeuro.com;Initial Catalog=FROM simply.com;Persist Security Info=True;User ID=FROM simply.com;Password=DB PASSWORD FROM simply.com;TrustServerCertificate=True"
                optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MovieDb; Integrated Security=True; Connect Timeout=30; Encrypt=False");
                MoviesDbContext _dbContext = new(optionsBuilder.Options);
                // clean database table: remove all rows
                _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE dbo.Movies");
                _repo = new MoviesRepositoryDB(_dbContext);
            }
            else
            {
                _repo = new MoviesRepositoryList();
            }
        }

        // Test methods are execute in alphabetical order

        [TestMethod]
        public void AddTest()
        {
            _repo.Add(new Movie { Title = "Z", Year = 1895 });
            Movie snowWhite = _repo.Add(new Movie { Title = "Snehvide", Year = 1937 });
            Assert.IsTrue(snowWhite.Id >= 0);
            IEnumerable<Movie> all = _repo.Get();
            Assert.AreEqual(2, all.Count());

            Assert.ThrowsException<ArgumentNullException>(
                () => _repo.Add(new Movie { Title = null, Year = 1895 }));
            Assert.ThrowsException<ArgumentException>(
                () => _repo.Add(new Movie { Title = "", Year = 1895 }));
            Assert.ThrowsException<ArgumentOutOfRangeException>(
                () => _repo.Add(new Movie { Title = "B", Year = 1894 }));
        }

        [TestMethod()]
        public void GetTest()
        {
            IEnumerable<Movie> movies = _repo.Get(orderBy: "Title");

            Assert.AreEqual("Snehvide", movies.First().Title);

            movies = _repo.Get(orderBy: "Year");
            Assert.AreEqual(movies.First().Title, "Z");

            movies = _repo.Get(titleIncludes: "vide");
            Assert.AreEqual(1, movies.Count());
            Assert.AreEqual("Snehvide", movies.First().Title);
        }

        [TestMethod]
        public void GetByIdTest()
        {
            Movie m = _repo.Add(new Movie { Title = "Tarzan", Year = 1932 });
            Movie? movie = _repo.GetById(m.Id);
            Assert.IsNotNull(movie);
            Assert.AreEqual("Tarzan", movie.Title);
            Assert.AreEqual(1932, movie.Year);

            Assert.IsNull(_repo.GetById(-1));
        }

        [TestMethod]
        public void RemoveTest()
        {
            Movie m = _repo.Add(new Movie { Title = "Olsenbanden", Year = 1968 });
            Movie? movie = _repo.Remove(m.Id);
            Assert.IsNotNull(movie);
            Assert.AreEqual("Olsenbanden", movie.Title);

            Movie? movie2 = _repo.Remove(m.Id);
            Assert.IsNull(movie2);
        }

        [TestMethod]
        public void UpdateTest()
        {
            Movie m = _repo.Add(new Movie { Title = "Citizen Kane", Year = 1941 });
            Movie? movie = _repo.Update(m.Id, new Movie { Title = "Den Store Mand", Year = 1941 });
            Assert.IsNotNull(movie);
            Movie? movie2 = _repo.GetById(m.Id);
            Assert.AreEqual("Den Store Mand", movie.Title);

            Assert.IsNull(
                _repo.Update(-1, new Movie { Title = "Buh", Year = 1967 }));
            Assert.ThrowsException<ArgumentException>(
                () => _repo.Update(m.Id, new Movie { Title = "", Year = 1941 }));
        }
    }
}