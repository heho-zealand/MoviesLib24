using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoviesRepositoryLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MoviesLib24;

namespace MoviesRepositoryLib.Tests
{
    [TestClass()]
    public class MoviesRepository3DBTest
    {
        private static MoviesDbContext? _dbContext;
        private static IMoviesRepository _repo;
        private static DbContextOptionsBuilder<MoviesDbContext> optionsBuilder;

        [ClassInitialize]

        // create table ...
        public static void InitOnce(TestContext context)
        {
            optionsBuilder = new DbContextOptionsBuilder<MoviesDbContext>();
            //Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MovieDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False
            optionsBuilder.UseSqlServer(@"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MovieDBV2; Integrated Security=True; Connect Timeout=30; Encrypt=False");           
           
            // clean database table: remove all rows
            //_dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE dbo.Movies");
            //_repo = new MoviesRepositoryDB(_dbContext);
        }

        [TestInitialize]
        public void TestSeup()
        {
            //Bemærk det er nødvendigt at lave en ny instans af MoviesDbContext, ellers vil DBSet ikke blive "nulstillet" 
            //og der bliver tracking problemer når DbSet bliver opdateret med nye objekter (da nye objekters Id vil være identiske med de gamle trackede objekters Id'er)
            //clean database table: remove all rows
            _dbContext = new MoviesDbContext(optionsBuilder.Options);
            _dbContext.Database.ExecuteSqlRaw("TRUNCATE TABLE dbo.Movies");
            _repo = new MoviesRepositoryDB(_dbContext);
            _repo.Add(new Movie() { Title = "Terminator 1", Year = 1984 });
            _repo.Add(new Movie() { Title = "The Matrix", Year = 1999 });
            _repo.Add(new Movie() { Title = "Snehvide", Year = 1937 });
            _repo.Add(new Movie() { Title = "Løvejagten", Year = 1907 });
            _repo.Add(new Movie() { Title = "Abekongen", Year = 1961 });
            _repo.Add(new Movie() { Title = "Terminator 2", Year = 1991 });
        }


        [TestMethod()]
        public void AddTest()
        {
            Movie xxx = _repo.Add(new Movie { Title = "xXx", Year = 2002  });
            Assert.IsTrue(xxx.Id >= 0);
            Assert.IsTrue(xxx.Title == "xXx");
            Assert.IsTrue(xxx.Year == 2002);
            IEnumerable<Movie> all = _repo.Get();
            Assert.AreEqual(7, all.Count());

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
            Assert.AreEqual(movies.First().Title, "Abekongen");

            movies = _repo.Get(orderBy: "Title_desc");
            Assert.AreEqual(movies.First().Title, "The Matrix");

            movies = _repo.Get(orderBy: "Title_asc");
            Assert.AreEqual(movies.First().Title, "Abekongen");


            movies = _repo.Get(orderBy: "Year");
            Assert.AreEqual(movies.First().Title, "Løvejagten");

            movies = _repo.Get(orderBy: "Year_desc");
            Assert.AreEqual(movies.First().Title, "The Matrix");

            movies = _repo.Get(titleIncludes: "vide");
            Assert.AreEqual(1, movies.Count());
            Assert.AreEqual(movies.First().Title, "Snehvide");

            movies = _repo.Get(titleIncludes: "minator");
            Assert.AreEqual(2, movies.Count());

            movies = _repo.Get(yearAfter: 1990);
            Assert.AreEqual(2, movies.Count());

            movies = _repo.Get(yearAfter: 1990, titleIncludes: "minator");
            Assert.AreEqual(1, movies.Count());
            Assert.AreEqual(movies.First().Title, "Terminator 2");
        }

        [TestMethod()]
        public void GetByIdTest()
        {
            Movie? movie = _repo.GetById(1);
            Assert.IsNotNull(movie);
            Assert.AreEqual("Terminator 1", movie.Title);
            Assert.AreEqual(1984, movie.Year);

            Movie m = _repo.Add(new Movie { Title = "Tarzan", Year = 1932 });
            movie = _repo.GetById(m.Id);
            Assert.IsNotNull(movie);
            Assert.AreEqual("Tarzan", movie.Title);
            Assert.AreEqual(1932, movie.Year);

            Assert.IsNull(_repo.GetById(-1));
        }

        [TestMethod()]
        public void RemoveTest()
        {
            Movie? movie = _repo.Remove(1);
            Assert.IsNotNull(movie);
            Assert.AreEqual("Terminator 1", movie.Title);

            Movie? movie2 = _repo.Remove(1);
            Assert.IsNull(movie2);
        }

        [TestMethod()]
        public void UpdateTest()
        { 
            Movie? movie = _repo.Update(6, new Movie { Title = "Terminator 2: Judgment Day", Year = 1941 });
            Assert.IsNotNull(movie);
            Movie? movie2 = _repo.GetById(6);
            Assert.AreEqual("Terminator 2: Judgment Day", movie.Title);

            Assert.IsNull(_repo.Update(-1, new Movie { Title = "Buh", Year = 1967 }));
            Assert.ThrowsException<ArgumentException>(() => _repo.Update(6, new Movie { Title = "", Year = 1941 }));
        }
    }
}
