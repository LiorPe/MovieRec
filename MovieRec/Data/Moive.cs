using System.Collections.Generic;

namespace MovieRec.Data
{
    public class Movie
    {
        public int ID { get;}
        public string Title { get; }
        public string Genres { get; }
        public Movie (int id, string title, string genres)
        {
            ID = id;
            Title = title;
            Genres = genres.Replace('|', ',');
        }
    }
}