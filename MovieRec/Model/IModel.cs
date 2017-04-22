using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MovieRec.Data;

namespace MovieRec
{
    interface IModel
    {
        List<Movie> GetAllMoviesExist();
        List<MovieRecommendation> GetRecommendationsForMovie(Movie _selectedMovie);
    }
}
