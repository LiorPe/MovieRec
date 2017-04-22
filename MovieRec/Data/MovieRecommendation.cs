using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieRec.Data
{
    public class MovieRecommendation : Movie
    {
        public int Rank { get; }
        public double Score { get; }

        public MovieRecommendation(Movie movie, int rank, double score ): base(movie.ID,movie.Title, movie.Genres)
        {
            Rank = rank;
            Score = score;
        }
    }
}
