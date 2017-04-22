using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieRec.Data;

namespace MovieRec.Model
{
    public class MyMovieModel : IModel
    {
        List<Movie> _allMovies; 
        public MyMovieModel ()
        {
            _allMovies = new List<Movie>();
            _allMovies.Add(new Movie(1, "Toy Story(1995)", "genere"));
            _allMovies.Add(new Movie(2, "Saving Santa (2013)", "genere"));
            _allMovies.Add(new Movie(3, "Ants in the Pants (2000)", "genere"));
            _allMovies.Add(new Movie(3, "Wings of Courage (1995)", "genere"));
        }

        public List<Movie> GetAllMoviesExist()
        {

            return _allMovies;

        }

        public List<MovieRecommendation> GetRecommendationsForMovie(Movie _selectedMovie)
        {
            List<MovieRecommendation> results = new List<MovieRecommendation>();
            Movie movie;
            for (int i = 0; i < _allMovies.Count; i++)
            {
                movie = _allMovies.ElementAt(i);
                results.Add(new MovieRecommendation(movie, i, i));
            }
            return results;
        }
    }
}
