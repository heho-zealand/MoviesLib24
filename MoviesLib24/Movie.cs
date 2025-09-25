namespace MoviesLib24
{
    public class Movie
    {
        private string _title;
        private int _year;

        public int Id { get; set; }
        public string Title
        {
            get => _title;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Title cannot be null");
                }
                if (value.Length < 1)
                {
                    throw new ArgumentException("Title must be at least 1 character");
                }
                _title = value;
            }
        }
        public int Year
        {
            get => _year; set
            {
                if (value < 1895)
                {
                    throw new ArgumentOutOfRangeException("Year must be at least 1895: " + value);
                }
                _year = value;
            }
        }

        public Movie(int id, string title, int year)
        {
            Id = id;
            Title = title;
            Year = year;
        }

        public Movie() : this(0, "Unknown", 1900) { }

        public override string ToString()
        {
            return $"{Id} {Title} {Year}";
        }
    }
}
